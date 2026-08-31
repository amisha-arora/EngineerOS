import { useNavigate } from "react-router-dom";

import type { RepositoryListItem } from "../../types/repository";

type RepositoryCardProps = {
    repository: RepositoryListItem;
    onDelete: (repositoryId: string) => void;
};

export default function RepositoryCard({
    repository,
    onDelete,
}: RepositoryCardProps) {
    const navigate = useNavigate();

    const createdDate =
        new Date(
            repository.createdAtUtc
        ).toLocaleDateString();

    return (
        <article className="repository-card">
            <div className="repository-card-main">

                <div>
                    <h2 className="repository-name">
                        {repository.name}
                    </h2>

                    <a
                        href={repository.url}
                        target="_blank"
                        rel="noreferrer"
                        className="repository-url"
                    >
                        {repository.url}
                    </a>
                </div>

                <span className="repository-status">
                    Added
                </span>

            </div>

            <div className="repository-meta">

                <div>
                    <span className="meta-label">
                        Created
                    </span>

                    <span className="meta-value">
                        {createdDate}
                    </span>
                </div>

                <div>
                    <span className="meta-label">
                        Last analyzed
                    </span>

                    <span className="meta-value">
                        Not analyzed yet
                    </span>
                </div>

            </div>

            <div className="repository-actions">

                <button
                    type="button"
                    className="repository-secondary-button"
                    onClick={() =>
                        navigate(
                            `/repositories/${repository.id}`
                        )
                    }
                >
                    Open repository
                </button>

                <button
                    type="button"
                    className="repository-delete-button"
                    onClick={() =>
                        onDelete(repository.id)
                    }
                >
                    Delete
                </button>

            </div>
        </article>
    );
}