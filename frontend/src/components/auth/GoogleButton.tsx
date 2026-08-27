type GoogleButtonProps = {
    text: string;
};

export default function GoogleButton({ text }: GoogleButtonProps) {
    const handleGoogleClick = () => {
        console.log("Mock Google authentication clicked");
    };

    return (
        <button
            type="button"
            className="google-button"
            onClick={handleGoogleClick}
        >
            <svg
                width="20"
                height="20"
                viewBox="0 0 24 24"
                aria-hidden="true"
            >
                <path
                    fill="#4285F4"
                    d="M21.6 12.23c0-.71-.06-1.4-.18-2.07H12v3.91h5.38a4.6 4.6 0 0 1-2 3.02v2.51h3.24c1.9-1.75 2.98-4.33 2.98-7.37Z"
                />
                <path
                    fill="#34A853"
                    d="M12 22c2.7 0 4.97-.89 6.62-2.4l-3.24-2.51c-.9.6-2.05.96-3.38.96-2.6 0-4.81-1.76-5.6-4.13H3.06v2.6A10 10 0 0 0 12 22Z"
                />
                <path
                    fill="#FBBC05"
                    d="M6.4 13.92a6.03 6.03 0 0 1 0-3.84v-2.6H3.06a10 10 0 0 0 0 9.04l3.34-2.6Z"
                />
                <path
                    fill="#EA4335"
                    d="M12 5.95c1.47 0 2.8.51 3.84 1.5l2.86-2.87A9.62 9.62 0 0 0 12 2a10 10 0 0 0-8.94 5.48l3.34 2.6c.79-2.37 3-4.13 5.6-4.13Z"
                />
            </svg>

            <span>{text}</span>
        </button>
    );
}