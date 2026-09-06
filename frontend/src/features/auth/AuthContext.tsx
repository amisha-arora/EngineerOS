import {
    createContext,
    useContext,
    useEffect,
    useState,
    type ReactNode,
} from "react";

import {
    loginUser,
    logoutUser,
} from "../../api/authApi";

import {
    getCurrentUser,
} from "../../api/userApi";

import {
    tokenStorage,
} from "../../utils/tokenStorage";

import type {
    CurrentUser,
    LoginRequest,
} from "../../types/auth";

type AuthContextValue = {
    user: CurrentUser | null;

    isAuthenticated: boolean;

    isInitializing: boolean;

    login: (
        request: LoginRequest
    ) => Promise<void>;

    logout: () => Promise<void>;
};

const AuthContext =
    createContext<AuthContextValue | undefined>(
        undefined
    );

type AuthProviderProps = {
    children: ReactNode;
};

export function AuthProvider({
    children,
}: AuthProviderProps) {
    const [
        user,
        setUser,
    ] = useState<CurrentUser | null>(
        null
    );

    const [
        isInitializing,
        setIsInitializing,
    ] = useState(true);

    /*
     * Runs once when the application starts.
     *
     * If an access token already exists,
     * try to restore the authenticated user.
     */
    useEffect(() => {
        const restoreSession = async () => {
            const accessToken =
                tokenStorage.getAccessToken();

            /*
             * No token means the user
             * is not currently logged in.
             */
            if (!accessToken) {
                setIsInitializing(false);

                return;
            }

            try {
                /*
                 * The token is sent by apiClient
                 * because /users/me is an
                 * authenticated endpoint.
                 */
                const currentUser =
                    await getCurrentUser();

                setUser(currentUser);
            } catch {
                /*
                 * The token may be expired,
                 * invalid, or otherwise unusable.
                 *
                 * Remove authentication data
                 * instead of keeping a broken
                 * session in the browser.
                 */
                tokenStorage.clear();

                setUser(null);
            } finally {
                setIsInitializing(false);
            }
        };

        restoreSession();
    }, []);

    /*
     * Login flow:
     *
     * credentials
     *      ↓
     * /auth/login
     *      ↓
     * access + refresh tokens
     *      ↓
     * save tokens
     *      ↓
     * /users/me
     *      ↓
     * authenticated user state
     */
    const login = async (
        request: LoginRequest
    ) => {
        const response =
            await loginUser(request);

        tokenStorage.setTokens(
            response.accessToken,
            response.refreshToken
        );

        try {
            const currentUser =
                await getCurrentUser();

            setUser(currentUser);
        } catch (error) {
            /*
             * If we receive tokens but cannot
             * retrieve the current user, do not
             * leave an incomplete session behind.
             */
            tokenStorage.clear();

            setUser(null);

            throw error;
        }
    };

    /*
     * Logout flow:
     *
     * refresh token
     *      ↓
     * backend logout/revocation
     *      ↓
     * always clear local tokens
     *      ↓
     * clear authenticated user state
     */
    const logout = async () => {
        const refreshToken =
            tokenStorage.getRefreshToken();

        try {
            if (refreshToken) {
                await logoutUser(
                    refreshToken
                );
            }
        } finally {
            /*
             * Even if the backend logout request
             * fails because the server is offline,
             * the local user should still be
             * logged out.
             */
            tokenStorage.clear();

            setUser(null);
        }
    };

    return (
        <AuthContext.Provider
            value={{
                user,

                isAuthenticated:
                    user !== null,

                isInitializing,

                login,

                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

/*
 * We intentionally keep this hook in the same
 * file as AuthProvider for a simpler project
 * structure.
 *
 * React Fast Refresh prefers files that only
 * export components, so this one lint warning
 * is intentionally suppressed.
 */

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
    const context =
        useContext(AuthContext);

    if (!context) {
        throw new Error(
            "useAuth must be used inside AuthProvider."
        );
    }

    return context;
}