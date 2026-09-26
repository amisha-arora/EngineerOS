
import { apiRequest } from "./apiClient";

import type {
    RepositoryDetails,
    RepositoryListItem,
    AnalyzeRepositoryResponse,
    RepositoryDependencies,
    RepositoryStructure,
} from "../types/repository";

export type CreateRepositoryRequest = {
    name: string;
    url: string;
};

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

export function createRepository(
    request: CreateRepositoryRequest
) {
    return apiRequest<RepositoryDetails>(
        "/api/v1/repositories",
        {
            method: "POST",
            authenticated: true,
            body: JSON.stringify(request),
        }
    );
}

export function analyzeRepository(
    repositoryId: string
) {
    return apiRequest<AnalyzeRepositoryResponse>(
        `/api/v1/repositories/${repositoryId}/analyze`,
        {
            method: "POST",
            authenticated: true,
        }
    );
}

export function getRepositoryStructure(
    repositoryId: string
) {
    return apiRequest<RepositoryStructure>(
        `/api/v1/repositories/${repositoryId}/structure`,
        {
            method: "GET",
            authenticated: true,
        }
    );
}

export function getRepositoryDependencies(
    repositoryId: string
) {
    return apiRequest<RepositoryDependencies>(
        `/api/v1/repositories/${repositoryId}/dependencies`,
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