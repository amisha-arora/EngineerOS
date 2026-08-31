import {
    useEffect,
    useState,
} from "react";

import {
    useNavigate,
    useParams,
} from "react-router-dom";

import AppLayout from "../layouts/AppLayout";

import {
    deleteRepository,
    getRepositoryById,
} from "../api/repositoryApi";

import type { RepositoryDetails } from "../types/repository";

import "../styles/repositories.css";

export default function RepositoryDetailsPage() {
    const { repositoryId } = useParams();

    const navigate = useNavigate();

    const [
        repository,
        setRepository,
    ] = useState<RepositoryDetails | null>(null);

    const [
        isLoading,
        setIsLoading,
    ] = useState(true);

    const [
        error,
        setError,
    ] = useState("");

    useEffect(() => {
        const loadRepository = async () => {
            if (!repositoryId) {
                setError("Repository id is missing.");
                setIsLoading(false);
                return;
            }

            try {
                const response =
                    await getRepositoryById(
                        repositoryId
                    );

                setRepository(response);
            } catch (error) {
                setError(
                    error instanceof Error
                        ? error.message
                        : "Unable to load repository."
                );
            } finally {
                setIsLoading(false);
            }
        };

        loadRepository();
    }, [repositoryId]);

    const handleDelete = async () => {
        if (!repositoryId) {
            return;
        }

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

            navigate(
                "/repositories",
                {
                    replace: true,
                }
            );
        } catch (error) {
            setError(
                error instanceof Error
                    ? error.message
                    : "Unable to delete repository."
            );
        }
    };

    if (isLoading) {
        return (
            <AppLayout>
                <div className="repository-loading">
                    Loading repository...
                </div>
            </AppLayout>
        );
    }

    if (error) {
        return (
            <AppLayout>
                <div className="repository-details-page">

                    <button
                        type="button"
                        className="repository-back-button"
                        onClick={() =>
                            navigate("/repositories")
                        }
                    >
                        ← Back to repositories
                    </button>

                    <div className="repository-error">
                        {error}
                    </div>

                </div>
            </AppLayout>
        );
    }

    if (!repository) {
        return (
            <AppLayout>
                <div className="repository-details-page">
                    Repository not found.
                </div>
            </AppLayout>
        );
    }

    const createdDate =
        new Date(
            repository.createdAtUtc
        ).toLocaleString();

    return (
        <AppLayout>
            <div className="repository-details-page">

                <button
                    type="button"
                    className="repository-back-button"
                    onClick={() =>
                        navigate("/repositories")
                    }
                >
                    ← Back to repositories
                </button>

                <div className="repository-details-header">

                    <div>
                        <h1>
                            {repository.name}
                        </h1>

                        <a
                            href={repository.url}
                            target="_blank"
                            rel="noreferrer"
                            className="repository-url"
                        >
                            {repository.url}
                        </a>
                    </div>

                    <button
                        type="button"
                        className="repository-delete-button"
                        onClick={handleDelete}
                    >
                        Delete Repository
                    </button>

                </div>

                <div className="repository-details-grid">

                    <section className="repository-details-card">
                        <span className="details-label">
                            Status
                        </span>

                        <strong>
                            Added
                        </strong>

                        <span className="details-note">
                            Temporary status
                        </span>
                    </section>

                    <section className="repository-details-card">
                        <span className="details-label">
                            Created
                        </span>

                        <strong>
                            {createdDate}
                        </strong>
                    </section>

                    <section className="repository-details-card">
                        <span className="details-label">
                            Last analyzed
                        </span>

                        <strong>
                            Not analyzed yet
                        </strong>

                        <span className="details-note">
                            Temporary value
                        </span>
                    </section>

                </div>

                <section className="repository-info-panel">

                    <div className="panel-header">
                        <div>
                            <h2>
                                Repository Information
                            </h2>

                            <p>
                                Data currently available
                                from the EngineerOS backend.
                            </p>
                        </div>
                    </div>

                    <div className="repository-info-list">

                        <div>
                            <span>
                                Repository ID
                            </span>

                            <strong>
                                {repository.id}
                            </strong>
                        </div>

                        <div>
                            <span>
                                Name
                            </span>

                            <strong>
                                {repository.name}
                            </strong>
                        </div>

                        <div>
                            <span>
                                URL
                            </span>

                            <strong>
                                {repository.url}
                            </strong>
                        </div>

                        <div>
                            <span>
                                Created
                            </span>

                            <strong>
                                {createdDate}
                            </strong>
                        </div>

                    </div>

                </section>

            </div>
        </AppLayout>
    );
}