import {
    useEffect,
    useState,
} from "react";

import {
    useNavigate,
    useParams,
} from "react-router-dom";

import AppLayout from "../layouts/AppLayout";

import Button from "../components/common/Button";
import Card from "../components/common/Card";
import LoadingSpinner from "../components/common/LoadingSpinner";
import ErrorMessage from "../components/common/ErrorMessage";
import PageHeader from "../components/common/PageHeader";
import Modal from "../components/common/Modal";

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

    const [
        isDeleteModalOpen,
        setIsDeleteModalOpen,
    ] = useState(false);

    const [
        isDeleting,
        setIsDeleting,
    ] = useState(false);

    useEffect(() => {
        const loadRepository = async () => {
            if (!repositoryId) {
                setError(
                    "Repository id is missing."
                );

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

        setIsDeleting(true);
        setError("");

        try {
            await deleteRepository(
                repositoryId
            );

            setIsDeleteModalOpen(false);

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
        } finally {
            setIsDeleting(false);
        }
    };

    if (isLoading) {
        return (
            <AppLayout>
                <LoadingSpinner
                    message="Loading repository..."
                />
            </AppLayout>
        );
    }

    if (error) {
        return (
            <AppLayout>
                <div className="repository-details-page">

                    <Button
                        type="button"
                        variant="secondary"
                        onClick={() =>
                            navigate(
                                "/repositories"
                            )
                        }
                    >
                        ← Back to repositories
                    </Button>

                    <ErrorMessage
                        message={error}
                    />

                </div>
            </AppLayout>
        );
    }

    if (!repository) {
        return (
            <AppLayout>
                <div className="repository-details-page">

                    <Button
                        type="button"
                        variant="secondary"
                        onClick={() =>
                            navigate(
                                "/repositories"
                            )
                        }
                    >
                        ← Back to repositories
                    </Button>

                    <ErrorMessage
                        message="Repository not found."
                    />

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

                <Button
                    type="button"
                    variant="secondary"
                    onClick={() =>
                        navigate(
                            "/repositories"
                        )
                    }
                >
                    ← Back to repositories
                </Button>

                <PageHeader
                    title={repository.name}
                    description="Repository details and workspace information."
                    action={
                        <Button
                            type="button"
                            variant="danger"
                            onClick={() =>
                                setIsDeleteModalOpen(true)
                            }
                        >
                            Delete Repository
                        </Button>
                    }
                />

                <a
                    href={repository.url}
                    target="_blank"
                    rel="noreferrer"
                    className="repository-url"
                >
                    {repository.url}
                </a>

                <div className="repository-details-grid">

                    <Card className="repository-details-card">
                        <span className="details-label">
                            Status
                        </span>

                        <strong>
                            Added
                        </strong>

                        <span className="details-note">
                            Temporary status
                        </span>
                    </Card>

                    <Card className="repository-details-card">
                        <span className="details-label">
                            Created
                        </span>

                        <strong>
                            {createdDate}
                        </strong>
                    </Card>

                    <Card className="repository-details-card">
                        <span className="details-label">
                            Last analyzed
                        </span>

                        <strong>
                            Not analyzed yet
                        </strong>

                        <span className="details-note">
                            Temporary value
                        </span>
                    </Card>

                </div>

                <Card className="repository-info-panel">

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

                </Card>

                <Modal
                    isOpen={isDeleteModalOpen}
                    title="Delete Repository"
                    onClose={() => {
                        if (!isDeleting) {
                            setIsDeleteModalOpen(false);
                        }
                    }}
                >
                    <div className="delete-modal-content">

                        <p>
                            Are you sure you want to delete{" "}
                            <strong>
                                {repository.name}
                            </strong>
                            ?
                        </p>

                        <p className="delete-modal-warning">
                            This action cannot be undone.
                        </p>

                        <div className="delete-modal-actions">

                            <Button
                                type="button"
                                variant="secondary"
                                disabled={isDeleting}
                                onClick={() =>
                                    setIsDeleteModalOpen(false)
                                }
                            >
                                Cancel
                            </Button>

                            <Button
                                type="button"
                                variant="danger"
                                isLoading={isDeleting}
                                onClick={handleDelete}
                            >
                                Delete Repository
                            </Button>

                        </div>

                    </div>
                </Modal>

            </div>
        </AppLayout>
    );
}