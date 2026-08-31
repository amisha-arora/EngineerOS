import { apiRequest } from "./apiClient";

import type {
    CurrentUser,
    LoginRequest,
    LoginResponse,
    RegisterRequest,
} from "../types/auth";

export function registerUser(
    request: RegisterRequest
) {
    return apiRequest(
        "/api/v1/auth/register",
        {
            method: "POST",
            body: JSON.stringify(request),
        }
    );
}

export function loginUser(
    request: LoginRequest
) {
    return apiRequest<LoginResponse>(
        "/api/v1/auth/login",
        {
            method: "POST",
            body: JSON.stringify(request),
        }
    );
}

export function getCurrentUser() {
    return apiRequest<CurrentUser>(
        "/api/v1/users/me",
        {
            method: "GET",
            authenticated: true,
        }
    );
}

export function logoutUser(
    refreshToken: string
) {
    return apiRequest<void>(
        "/api/v1/auth/logout",
        {
            method: "POST",
            body: JSON.stringify({
                refreshToken,
            }),
        }
    );
}