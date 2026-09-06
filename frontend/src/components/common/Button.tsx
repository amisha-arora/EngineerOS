import type {
    ButtonHTMLAttributes,
    ReactNode,
} from "react";

type ButtonVariant =
    | "primary"
    | "secondary"
    | "danger";

type ButtonProps =
    ButtonHTMLAttributes<HTMLButtonElement> & {
        children: ReactNode;
        variant?: ButtonVariant;
        isLoading?: boolean;
    };

export default function Button({
    children,
    variant = "primary",
    isLoading = false,
    disabled,
    className = "",
    ...props
}: ButtonProps) {
    return (
        <button
            className={`app-button app-button-${variant} ${className}`}
            disabled={disabled || isLoading}
            {...props}
        >
            {isLoading
                ? "Please wait..."
                : children}
        </button>
    );
}