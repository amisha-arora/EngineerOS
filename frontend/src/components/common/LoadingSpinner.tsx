type LoadingSpinnerProps = {
    message?: string;
};

export default function LoadingSpinner({
    message = "Loading...",
}: LoadingSpinnerProps) {
    return (
        <div
            className="loading-state"
            role="status"
        >
            <div className="loading-spinner" />

            <span>{message}</span>
        </div>
    );
}