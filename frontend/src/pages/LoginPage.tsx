// src/pages/LoginPage.tsx

import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";

import AuthLayout from "../components/auth/AuthLayout";
import GoogleButton from "../components/auth/GoogleButton";

import { useAuth } from "../features/auth/AuthContext";

import "../styles/auth.css";

type LoginErrors = {
    email?: string;
    password?: string;
};

export default function LoginPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

    const [showPassword, setShowPassword] =
        useState(false);

    const [isLoading, setIsLoading] =
        useState(false);

    const [errors, setErrors] =
        useState<LoginErrors>({});

    const [apiError, setApiError] =
        useState("");

    const navigate = useNavigate();

    const { login } = useAuth();

    const validate = () => {
        const newErrors: LoginErrors = {};

        if (!email.trim()) {
            newErrors.email =
                "Email is required.";
        } else if (
            !/^\S+@\S+\.\S+$/.test(email)
        ) {
            newErrors.email =
                "Enter a valid email address.";
        }

        if (!password) {
            newErrors.password =
                "Password is required.";
        } else if (password.length < 8) {
            newErrors.password =
                "Password must contain at least 8 characters.";
        }

        setErrors(newErrors);

        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        setApiError("");

        if (!validate()) {
            return;
        }

        setIsLoading(true);

        try {
            await login({
                email,
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
            <div className="auth-content">
                <div className="auth-heading">
                    <h1>
                        Sign in to EngineerOS
                    </h1>

                    <p>
                        Continue to your engineering
                        workspace.
                    </p>
                </div>

                <form
                    className="auth-form"
                    onSubmit={handleSubmit}
                    noValidate
                >
                    <div className="form-group">
                        <label htmlFor="email">
                            Email address
                        </label>

                        <input
                            id="email"
                            type="email"
                            placeholder="you@example.com"
                            value={email}
                            className={
                                errors.email
                                    ? "input-error"
                                    : ""
                            }
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

                        {errors.email && (
                            <span className="field-error">
                                {errors.email}
                            </span>
                        )}
                    </div>

                    <div className="form-group">
                        <div className="label-row">
                            <label htmlFor="password">
                                Password
                            </label>

                            <button
                                type="button"
                                className="text-button"
                                onClick={() =>
                                    console.log(
                                        "Forgot password clicked"
                                    )
                                }
                            >
                                Forgot password?
                            </button>
                        </div>

                        <div className="password-wrapper">
                            <input
                                id="password"
                                type={
                                    showPassword
                                        ? "text"
                                        : "password"
                                }
                                placeholder="Enter your password"
                                value={password}
                                className={
                                    errors.password
                                        ? "input-error"
                                        : ""
                                }
                                onChange={(event) => {
                                    setPassword(
                                        event.target.value
                                    );

                                    if (
                                        errors.password
                                    ) {
                                        setErrors(
                                            (current) => ({
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

                        {errors.password && (
                            <span className="field-error">
                                {errors.password}
                            </span>
                        )}
                    </div>

                    {apiError && (
                        <div className="api-error">
                            {apiError}
                        </div>
                    )}

                    <button
                        type="submit"
                        className="primary-button"
                        disabled={isLoading}
                    >
                        {isLoading
                            ? "Signing in..."
                            : "Continue"}
                    </button>
                </form>

                <div className="auth-divider">
                    <span>OR</span>
                </div>

                <GoogleButton text="Continue with Google" />

                <p className="auth-switch">
                    Don't have an account?{" "}
                    <Link to="/register">
                        Create an account
                    </Link>
                </p>
            </div>
        </AuthLayout>
    );
}