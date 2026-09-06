import { useNavigate } from "react-router-dom";

import {
    useEffect,
    useState,
} from "react";

import AppLayout from "../layouts/AppLayout";

import RepositoryCard from "../components/repositories/RepositoryCard";
import RepositoryEmptyState from "../components/repositories/RepositoryEmptyState";

import Button from "../components/common/Button";
import PageHeader from "../components/common/PageHeader";
import LoadingSpinner from "../components/common/LoadingSpinner";
import ErrorMessage from "../components/common/ErrorMessage";

import {
    deleteRepository,
    getRepositories,
} from "../api/repositoryApi";

import type { RepositoryListItem } from "../types/repository";

import "../styles/repositories.css";

export default function RepositoriesPage() {
    const navigate = useNavigate();
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

                <PageHeader
                    title="Repositories"
                    description="Browse and manage the repositories in your workspace."
                    action={
                        <Button
                            type="button"
                            onClick={() =>
                                navigate("/repositories/upload")
                            }
                        >
                            Upload Repository
                        </Button>
                    }
                />

                <ErrorMessage message={error} />

                {isLoading ? (
                    <LoadingSpinner
                        message="Loading repositories..."
                    />
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