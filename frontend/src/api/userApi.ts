import { apiRequest } from "./apiClient";

import type {
    CurrentUser,
} from "../types/auth";

export function getCurrentUser() {
    return apiRequest<CurrentUser>(
        "/api/v1/users/me",
        {
            method: "GET",
            authenticated: true,
        }
    );
}