import type {
    InputHTMLAttributes,
} from "react";

type InputProps =
    InputHTMLAttributes<HTMLInputElement> & {
        label?: string;
        error?: string;
        hint?: string;
    };

export default function Input({
    label,
    error,
    hint,
    id,
    className = "",
    ...props
}: InputProps) {
    return (
        <div className="app-input-group">

            {label && (
                <label htmlFor={id}>
                    {label}
                </label>
            )}

            <input
                id={id}
                className={`app-input ${error
                        ? "app-input-error"
                        : ""
                    } ${className}`}
                {...props}
            />

            {hint && !error && (
                <span className="app-input-hint">
                    {hint}
                </span>
            )}

            {error && (
                <span className="app-input-error-message">
                    {error}
                </span>
            )}

        </div>
    );
}