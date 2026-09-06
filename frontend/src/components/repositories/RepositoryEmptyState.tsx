import { useNavigate } from "react-router-dom";

import EmptyState from "../common/EmptyState";
import Button from "../common/Button";

export default function RepositoryEmptyState() {
    const navigate = useNavigate();

    return (
        <EmptyState
            title="No repositories yet"
            description="Upload your first repository to start managing and analyzing your codebase with EngineerOS."
            action={
                <Button
                    type="button"
                    onClick={() =>
                        navigate(
                            "/repositories/upload"
                        )
                    }
                >
                    Upload Repository
                </Button>
            }
        />
    );
}  