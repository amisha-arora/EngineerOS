import {
    useState,
    type FormEvent,
} from "react";

import {
    Link,
    useNavigate,
} from "react-router-dom";

import AuthLayout from "../components/auth/AuthLayout";
import GoogleButton from "../components/auth/GoogleButton";

import Button from "../components/common/Button";
import Input from "../components/common/Input";
import ErrorMessage from "../components/common/ErrorMessage";

import {
    useAuth,
} from "../features/auth/AuthContext";

import "../styles/auth.css";

type LoginErrors = {
    email?: string;
    password?: string;
};

export default function LoginPage() {
    const navigate = useNavigate();

    const { login } = useAuth();

    const [
        email,
        setEmail,
    ] = useState("");

    const [
        password,
        setPassword,
    ] = useState("");

    const [
        showPassword,
        setShowPassword,
    ] = useState(false);

    const [
        errors,
        setErrors,
    ] = useState<LoginErrors>({});

    const [
        apiError,
        setApiError,
    ] = useState("");

    const [
        isLoading,
        setIsLoading,
    ] = useState(false);

    const validateForm = () => {
        const newErrors: LoginErrors = {};

        if (!email.trim()) {
            newErrors.email =
                "Email is required.";
        }

        if (!password) {
            newErrors.password =
                "Password is required.";
        }

        setErrors(newErrors);

        return (
            Object.keys(newErrors).length === 0
        );
    };

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        setApiError("");

        if (!validateForm()) {
            return;
        }

        setIsLoading(true);

        try {
            await login({
                email: email.trim(),
                password,
            });

            navigate(
                "/dashboard",
                {
                    replace: true,
                }
            );
        } catch (error) {
            setApiError(
                error instanceof Error
                    ? error.message
                    : "Unable to sign in."
            );
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <AuthLayout>

            <div className="auth-heading">
                <h1>
                    Welcome back
                </h1>

                <p>
                    Sign in to continue to EngineerOS.
                </p>
            </div>

            <form
                className="auth-form"
                onSubmit={handleSubmit}
            >

                <Input
                    id="email"
                    type="email"
                    label="Email address"
                    placeholder="you@example.com"
                    autoComplete="email"
                    value={email}
                    error={errors.email}
                    onChange={(event) => {
                        setEmail(
                            event.target.value
                        );

                        if (errors.email) {
                            setErrors(
                                (current) => ({
                                    ...current,
                                    email: undefined,
                                })
                            );
                        }

                        if (apiError) {
                            setApiError("");
                        }
                    }}
                />

                <div className="form-group">

                    <div className="label-row">
                        <label htmlFor="password">
                            Password
                        </label>

                        <button
                            type="button"
                            className="text-button"
                            disabled
                            title="Password recovery is not implemented yet."
                        >
                            Forgot password?
                        </button>
                    </div>

                    <div className="password-wrapper">

                        <Input
                            id="password"
                            type={
                                showPassword
                                    ? "text"
                                    : "password"
                            }
                            placeholder="Enter your password"
                            autoComplete="current-password"
                            value={password}
                            error={errors.password}
                            onChange={(event) => {
                                setPassword(
                                    event.target.value
                                );

                                if (
                                    errors.password
                                ) {
                                    setErrors(
                                        (
                                            current
                                        ) => ({
                                            ...current,
                                            password:
                                                undefined,
                                        })
                                    );
                                }

                                if (apiError) {
                                    setApiError("");
                                }
                            }}
                        />

                        <button
                            type="button"
                            className="password-toggle"
                            onClick={() =>
                                setShowPassword(
                                    (current) =>
                                        !current
                                )
                            }
                        >
                            {showPassword
                                ? "Hide"
                                : "Show"}
                        </button>

                    </div>

                </div>

                <ErrorMessage
                    message={apiError}
                />

                <Button
                    type="submit"
                    isLoading={isLoading}
                    className="primary-button"
                >
                    Continue
                </Button>

            </form>

            <div className="auth-divider">
                <span>OR</span>
            </div>

            <GoogleButton
                text="Continue with Google"
            />

            <p className="auth-switch-text">
                Don&apos;t have an account?{" "}
                <Link to="/register">
                    Create one
                </Link>
            </p>

        </AuthLayout>
    );
}