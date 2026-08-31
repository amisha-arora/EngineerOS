import {
    createContext,
    useContext,
    useEffect,
    useState,
    type ReactNode,
} from "react";

import {
    getCurrentUser,
    loginUser,
    logoutUser,
} from "../../api/authApi";

import { tokenStorage } from "../../utils/tokenStorage";

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
    const [user, setUser] =
        useState<CurrentUser | null>(null);

    const [isInitializing, setIsInitializing] =
        useState(true);

    useEffect(() => {
        const restoreSession = async () => {
            const token =
                tokenStorage.getAccessToken();

            if (!token) {
                setIsInitializing(false);
                return;
            }

            try {
                const currentUser =
                    await getCurrentUser();

                setUser(currentUser);
            } catch {
                tokenStorage.clear();
                setUser(null);
            } finally {
                setIsInitializing(false);
            }
        };

        restoreSession();
    }, []);

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
            tokenStorage.clear();
            throw error;
        }
    };

    const logout = async () => {
        const refreshToken =
            tokenStorage.getRefreshToken();

        try {
            if (refreshToken) {
                await logoutUser(refreshToken);
            }
        } finally {
            tokenStorage.clear();
            setUser(null);
        }
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                isAuthenticated: user !== null,
                isInitializing,
                login,
                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
}

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