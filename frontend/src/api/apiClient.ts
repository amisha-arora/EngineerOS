import { tokenStorage } from "../utils/tokenStorage";
import { ApiError } from "./ApiError";

const API_BASE_URL =
    import.meta.env.VITE_API_BASE_URL;

type ApiOptions = RequestInit & {
    authenticated?: boolean;
};

export async function apiRequest<T>(
    endpoint: string,
    options: ApiOptions = {}
): Promise<T> {
    const {
        authenticated = false,
        headers,
        ...requestOptions
    } = options;

    const requestHeaders =
        new Headers(headers);

    /*
     * Only set JSON content type when
     * a request actually contains a body.
     *
     * This also keeps this client ready
     * for FormData/file uploads later.
     */
    if (
        requestOptions.body &&
        !(requestOptions.body instanceof FormData)
    ) {
        requestHeaders.set(
            "Content-Type",
            "application/json"
        );
    }

    if (authenticated) {
        const accessToken =
            tokenStorage.getAccessToken();

        if (!accessToken) {
            throw new ApiError(
                "Your session has expired. Please sign in again.",
                401
            );
        }

        requestHeaders.set(
            "Authorization",
            `Bearer ${accessToken}`
        );
    }

    let response: Response;

    try {
        response = await fetch(
            `${API_BASE_URL}${endpoint}`,
            {
                ...requestOptions,
                headers: requestHeaders,
            }
        );
    } catch {
        throw new ApiError(
            "Unable to connect to the EngineerOS server.",
            0
        );
    }

    if (!response.ok) {
        let message =
            getDefaultErrorMessage(
                response.status
            );

        try {
            const errorBody =
                await response.json();

            message =
                errorBody.message ??
                errorBody.title ??
                message;
        } catch {
            // Response may not contain JSON.
        }

        if (response.status === 401) {
            tokenStorage.clear();
        }

        throw new ApiError(
            message,
            response.status
        );
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json() as Promise<T>;
}

function getDefaultErrorMessage(
    status: number
) {
    switch (status) {
        case 400:
            return "The request is invalid.";

        case 401:
            return "Your session has expired. Please sign in again.";

        case 403:
            return "You do not have permission to perform this action.";

        case 404:
            return "The requested resource could not be found.";

        case 409:
            return "This resource already exists.";

        case 500:
            return "The server encountered an unexpected error.";

        default:
            return "Something went wrong.";
    }
}