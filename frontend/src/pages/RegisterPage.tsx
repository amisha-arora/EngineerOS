
import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";

import AuthLayout from "../components/auth/AuthLayout";
import GoogleButton from "../components/auth/GoogleButton";

import { registerUser } from "../api/authApi";

import "../styles/auth.css";

type RegisterErrors = {
    firstName?: string;
    lastName?: string;
    email?: string;
    password?: string;
    confirmPassword?: string;
};

export default function RegisterPage() {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] =
        useState("");

    const [showPassword, setShowPassword] =
        useState(false);

    const [isLoading, setIsLoading] =
        useState(false);

    const [errors, setErrors] =
        useState<RegisterErrors>({});

    const [apiError, setApiError] =
        useState("");

    const navigate = useNavigate();

    const validate = () => {
        const newErrors: RegisterErrors = {};

        if (!firstName.trim()) {
            newErrors.firstName =
                "First name is required.";
        }

        if (!lastName.trim()) {
            newErrors.lastName =
                "Last name is required.";
        }

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
                "Use at least 8 characters.";
        }

        if (!confirmPassword) {
            newErrors.confirmPassword =
                "Confirm your password.";
        } else if (
            password !== confirmPassword
        ) {
            newErrors.confirmPassword =
                "Passwords do not match.";
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
            await registerUser({
                firstName,
                lastName,
                email,
                password,
            });

            navigate(
                "/login",
                {
                    replace: true,
                    state: {
                        registered: true,
                    },
                }
            );
        } catch (error) {
            setApiError(
                error instanceof Error
                    ? error.message
                    : "Unable to create account."
            );
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <AuthLayout>
            <div className="auth-content register-content">
                <div className="auth-heading">
                    <h1>Create your account</h1>

                    <p>
                        Get started with EngineerOS.
                    </p>
                </div>

                <form
                    className="auth-form"
                    onSubmit={handleSubmit}
                    noValidate
                >
                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="firstName">
                                First name
                            </label>

                            <input
                                id="firstName"
                                type="text"
                                placeholder="First name"
                                value={firstName}
                                className={
                                    errors.firstName
                                        ? "input-error"
                                        : ""
                                }
                                onChange={(event) => {
                                    setFirstName(
                                        event.target.value
                                    );

                                    if (errors.firstName) {
                                        setErrors((current) => ({
                                            ...current,
                                            firstName: undefined,
                                        }));
                                    }

                                    if (apiError) {
                                        setApiError("");
                                    }
                                }}
                            />

                            {errors.firstName && (
                                <span className="field-error">
                                    {errors.firstName}
                                </span>
                            )}
                        </div>

                        <div className="form-group">
                            <label htmlFor="lastName">
                                Last name
                            </label>

                            <input
                                id="lastName"
                                type="text"
                                placeholder="Last name"
                                value={lastName}
                                className={
                                    errors.lastName
                                        ? "input-error"
                                        : ""
                                }
                                onChange={(event) => {
                                    setLastName(
                                        event.target.value
                                    );

                                    if (errors.lastName) {
                                        setErrors((current) => ({
                                            ...current,
                                            lastName: undefined,
                                        }));
                                    }

                                    if (apiError) {
                                        setApiError("");
                                    }
                                }}
                            />

                            {errors.lastName && (
                                <span className="field-error">
                                    {errors.lastName}
                                </span>
                            )}
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="registerEmail">
                            Email address
                        </label>

                        <input
                            id="registerEmail"
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
                                    setErrors((current) => ({
                                        ...current,
                                        email: undefined,
                                    }));
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
                        <label htmlFor="registerPassword">
                            Password
                        </label>

                        <div className="password-wrapper">
                            <input
                                id="registerPassword"
                                type={
                                    showPassword
                                        ? "text"
                                        : "password"
                                }
                                placeholder="Create a password"
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

                                    if (errors.password) {
                                        setErrors((current) => ({
                                            ...current,
                                            password: undefined,
                                        }));
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
                                        (current) => !current
                                    )
                                }
                            >
                                {showPassword
                                    ? "Hide"
                                    : "Show"}
                            </button>
                        </div>

                        <span className="field-hint">
                            Minimum 8 characters.
                        </span>

                        {errors.password && (
                            <span className="field-error">
                                {errors.password}
                            </span>
                        )}
                    </div>

                    <div className="form-group">
                        <label htmlFor="confirmPassword">
                            Confirm password
                        </label>

                        <input
                            id="confirmPassword"
                            type={
                                showPassword
                                    ? "text"
                                    : "password"
                            }
                            placeholder="Repeat your password"
                            value={confirmPassword}
                            className={
                                errors.confirmPassword
                                    ? "input-error"
                                    : ""
                            }
                            onChange={(event) => {
                                setConfirmPassword(
                                    event.target.value
                                );

                                if (
                                    errors.confirmPassword
                                ) {
                                    setErrors((current) => ({
                                        ...current,
                                        confirmPassword:
                                            undefined,
                                    }));
                                }

                                if (apiError) {
                                    setApiError("");
                                }
                            }}
                        />

                        {errors.confirmPassword && (
                            <span className="field-error">
                                {errors.confirmPassword}
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
                            ? "Creating account..."
                            : "Create account"}
                    </button>
                </form>

                <div className="auth-divider">
                    <span>OR</span>
                </div>

                <GoogleButton text="Continue with Google" />

                <p className="auth-switch">
                    Already have an account?{" "}
                    <Link to="/login">
                        Sign in
                    </Link>
                </p>
            </div>
        </AuthLayout>
    );
}