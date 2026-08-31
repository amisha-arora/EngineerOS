
import { apiRequest } from "./apiClient";

import type {
    RepositoryDetails,
    RepositoryListItem,
} from "../types/repository";

export function getRepositories() {
    return apiRequest<RepositoryListItem[]>(
        "/api/v1/repositories",
        {
            method: "GET",
            authenticated: true,
        }
    );
}

export function getRepositoryById(
    repositoryId: string
) {
    return apiRequest<RepositoryDetails>(
        `/api/v1/repositories/${repositoryId}`,
        {
            method: "GET",
            authenticated: true,
        }
    );
}

export function deleteRepository(
    repositoryId: string
) {
    return apiRequest<void>(
        `/api/v1/repositories/${repositoryId}`,
        {
            method: "DELETE",
            authenticated: true,
        }
    );
}