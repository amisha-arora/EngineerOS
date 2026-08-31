import { tokenStorage } from "../utils/tokenStorage";

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

    requestHeaders.set(
        "Content-Type",
        "application/json"
    );

    if (authenticated) {
        const accessToken =
            tokenStorage.getAccessToken();

        if (accessToken) {
            requestHeaders.set(
                "Authorization",
                `Bearer ${accessToken}`
            );
        }
    }

    const response = await fetch(
        `${API_BASE_URL}${endpoint}`,
        {
            ...requestOptions,
            headers: requestHeaders,
        }
    );

    if (!response.ok) {
        let message =
            "Something went wrong.";

        try {
            const errorBody =
                await response.json();

            message =
                errorBody.message ??
                errorBody.title ??
                message;
        } catch {
            // Response may have no JSON body.
        }

        throw new Error(message);
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json() as Promise<T>;
}