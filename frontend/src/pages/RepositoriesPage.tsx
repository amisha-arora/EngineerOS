// src/pages/RepositoriesPage.tsx

import {
    useEffect,
    useState,
} from "react";

import AppLayout from "../layouts/AppLayout";

import RepositoryCard from "../components/repositories/RepositoryCard";
import RepositoryEmptyState from "../components/repositories/RepositoryEmptyState";

import {
    deleteRepository,
    getRepositories,
} from "../api/repositoryApi";

import type { RepositoryListItem } from "../types/repository";

import "../styles/repositories.css";

export default function RepositoriesPage() {
    const [
        repositories,
        setRepositories,
    ] = useState<RepositoryListItem[]>([]);

    const [
        isLoading,
        setIsLoading,
    ] = useState(true);

    const [
        error,
        setError,
    ] = useState("");

    useEffect(() => {
        const loadRepositories = async () => {
            try {
                const response =
                    await getRepositories();

                setRepositories(response);
            } catch (error) {
                setError(
                    error instanceof Error
                        ? error.message
                        : "Unable to load repositories."
                );
            } finally {
                setIsLoading(false);
            }
        };

        loadRepositories();
    }, []);

    const handleDelete = async (
        repositoryId: string
    ) => {
        const confirmed =
            window.confirm(
                "Are you sure you want to delete this repository?"
            );

        if (!confirmed) {
            return;
        }

        try {
            await deleteRepository(
                repositoryId
            );

            setRepositories(
                (currentRepositories) =>
                    currentRepositories.filter(
                        (repository) =>
                            repository.id !== repositoryId
                    )
            );
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : "Unable to delete repository."
            );
        }
    };

    return (
        <AppLayout>
            <div className="repositories-page">

                <div className="repositories-header">

                    <div>
                        <h1>Repositories</h1>

                        <p>
                            Browse and manage the
                            repositories in your workspace.
                        </p>
                    </div>

                    <button
                        type="button"
                        className="repository-primary-button"
                    >
                        Upload Repository
                    </button>

                </div>

                {error && (
                    <div className="repository-error">
                        {error}
                    </div>
                )}

                {isLoading ? (
                    <div className="repository-loading">
                        Loading repositories...
                    </div>
                ) : repositories.length === 0 ? (
                    <RepositoryEmptyState />
                ) : (
                    <div className="repository-list">
                        {repositories.map(
                            (repository) => (
                                <RepositoryCard
                                    key={repository.id}
                                    repository={repository}
                                    onDelete={handleDelete}
                                />
                            )
                        )}
                    </div>
                )}

            </div>
        </AppLayout>
    );
}