import {
    useRef,
    useState,
    type ChangeEvent,
    type DragEvent,
    type FormEvent,
} from "react";

import {
    useNavigate,
} from "react-router-dom";

import AppLayout from "../layouts/AppLayout";

import Button from "../components/common/Button";
import Input from "../components/common/Input";
import PageHeader from "../components/common/PageHeader";
import ErrorMessage from "../components/common/ErrorMessage";

import {
    createRepository,
} from "../api/repositoryApi";

import "../styles/repositories.css";

export default function RepositoryUploadPage() {
    const navigate = useNavigate();

    const fileInputRef =
        useRef<HTMLInputElement | null>(null);

    const [
        repositoryName,
        setRepositoryName,
    ] = useState("");

    const [
        repositoryUrl,
        setRepositoryUrl,
    ] = useState("");

    const [
        selectedFile,
        setSelectedFile,
    ] = useState<File | null>(null);

    const [
        fileError,
        setFileError,
    ] = useState("");

    const [
        formError,
        setFormError,
    ] = useState("");

    const [
        isSubmitting,
        setIsSubmitting,
    ] = useState(false);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState("");

    const validateFile = (
        file: File
    ) => {
        setFileError("");

        const isZip =
            file.name
                .toLowerCase()
                .endsWith(".zip");

        if (!isZip) {
            setSelectedFile(null);

            setFileError(
                "Only ZIP files are currently supported."
            );

            return;
        }

        setSelectedFile(file);
    };

    const handleFileChange = (
        event: ChangeEvent<HTMLInputElement>
    ) => {
        const file =
            event.target.files?.[0];

        if (file) {
            validateFile(file);
        }
    };

    const handleDrop = (
        event: DragEvent<HTMLDivElement>
    ) => {
        event.preventDefault();

        const file =
            event.dataTransfer.files?.[0];

        if (file) {
            validateFile(file);
        }
    };

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        setFormError("");
        setSuccessMessage("");

        if (!repositoryName.trim()) {
            setFormError(
                "Repository name is required."
            );

            return;
        }

        if (!repositoryUrl.trim()) {
            setFormError(
                "Repository URL is required."
            );

            return;
        }

        setIsSubmitting(true);

        try {
            const repository =
                await createRepository({
                    name: repositoryName.trim(),
                    url: repositoryUrl.trim(),
                });

            setSuccessMessage(
                "Repository created successfully."
            );

            /*
             * selectedFile is intentionally
             * not sent to the backend.
             *
             * ZIP upload/storage is not
             * implemented yet.
             */

            navigate(
                `/repositories/${repository.id}`,
                {
                    replace: true,
                }
            );
        } catch (error) {
            setFormError(
                error instanceof Error
                    ? error.message
                    : "Unable to create repository."
            );
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <AppLayout>
            <div className="repository-upload-page">

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
                    title="Upload Repository"
                    description="Add a repository to your EngineerOS workspace."
                />

                <form
                    className="repository-upload-form"
                    onSubmit={handleSubmit}
                >

                    <Input
                        id="repositoryName"
                        type="text"
                        label="Repository name"
                        placeholder="e.g. EngineerOS"
                        value={repositoryName}
                        onChange={(event) =>
                            setRepositoryName(
                                event.target.value
                            )
                        }
                    />

                    <Input
                        id="repositoryUrl"
                        type="url"
                        label="Repository URL"
                        placeholder="https://github.com/username/repository"
                        value={repositoryUrl}
                        hint="This URL is currently used by EngineerOS to create the repository record."
                        onChange={(event) =>
                            setRepositoryUrl(
                                event.target.value
                            )
                        }
                    />

                    <div className="upload-form-group">

                        <label>
                            Repository archive
                        </label>

                        <div
                            className="repository-drop-zone"
                            onDragOver={(event) =>
                                event.preventDefault()
                            }
                            onDrop={handleDrop}
                        >

                            <div className="upload-icon">
                                ↑
                            </div>

                            <h2>
                                Drag and drop your repository
                            </h2>

                            <p>
                                Drop a ZIP archive here or
                                browse from your computer.
                            </p>

                            <Button
                                type="button"
                                variant="secondary"
                                className="browse-file-button"
                                onClick={() =>
                                    fileInputRef.current?.click()
                                }
                            >
                                Browse file
                            </Button>

                            <input
                                ref={fileInputRef}
                                type="file"
                                accept=".zip,application/zip"
                                hidden
                                onChange={
                                    handleFileChange
                                }
                            />

                            <span className="supported-format">
                                Supported format: .zip
                            </span>

                        </div>

                        {selectedFile && (
                            <div className="selected-file">

                                <div>
                                    <strong>
                                        {selectedFile.name}
                                    </strong>

                                    <span>
                                        {(
                                            selectedFile.size /
                                            1024 /
                                            1024
                                        ).toFixed(2)}{" "}
                                        MB
                                    </span>
                                </div>

                                <Button
                                    type="button"
                                    variant="secondary"
                                    onClick={() =>
                                        setSelectedFile(
                                            null
                                        )
                                    }
                                >
                                    Remove
                                </Button>

                            </div>
                        )}

                        <ErrorMessage
                            message={fileError}
                        />

                        <div className="upload-notice">
                            ZIP upload is not yet supported by
                            the backend. Selecting a file here
                            does not upload or store it.
                        </div>

                    </div>

                    <ErrorMessage
                        message={formError}
                    />

                    {successMessage && (
                        <div className="upload-success">
                            {successMessage}
                        </div>
                    )}

                    <div className="upload-actions">

                        <Button
                            type="button"
                            variant="secondary"
                            onClick={() =>
                                navigate(
                                    "/repositories"
                                )
                            }
                        >
                            Cancel
                        </Button>

                        <Button
                            type="submit"
                            isLoading={isSubmitting}
                        >
                            Add Repository
                        </Button>

                    </div>

                </form>

            </div>
        </AppLayout>
    );
}