import { useNavigate } from "react-router-dom";

export default function RepositoryEmptyState() {
    const navigate = useNavigate();

    return (
        <div className="repository-empty-state">
            <div className="empty-state-icon">
                +
            </div>

            <h2>No repositories yet</h2>

            <p>
                Upload your first repository to start
                exploring and understanding your codebase.
            </p>

            <button
                type="button"
                className="repository-primary-button"
                onClick={() =>
                    navigate("/repositories/upload")
                }
            >
                Upload your first repository
            </button>
        </div>
    );
}