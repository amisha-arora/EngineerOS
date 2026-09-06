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
    registerUser,
} from "../api/authApi";

import {
    useAuth,
} from "../features/auth/AuthContext";

import "../styles/auth.css";

type RegisterErrors = {
    firstName?: string;
    lastName?: string;
    email?: string;
    password?: string;
    confirmPassword?: string;
};

export default function RegisterPage() {
    const navigate = useNavigate();

    const { login } = useAuth();

    const [
        firstName,
        setFirstName,
    ] = useState("");

    const [
        lastName,
        setLastName,
    ] = useState("");

    const [
        email,
        setEmail,
    ] = useState("");

    const [
        password,
        setPassword,
    ] = useState("");

    const [
        confirmPassword,
        setConfirmPassword,
    ] = useState("");

    const [
        showPassword,
        setShowPassword,
    ] = useState(false);

    const [
        showConfirmPassword,
        setShowConfirmPassword,
    ] = useState(false);

    const [
        errors,
        setErrors,
    ] = useState<RegisterErrors>({});

    const [
        apiError,
        setApiError,
    ] = useState("");

    const [
        isLoading,
        setIsLoading,
    ] = useState(false);

    const clearFieldError = (
        field: keyof RegisterErrors
    ) => {
        setErrors(
            (current) => ({
                ...current,
                [field]: undefined,
            })
        );

        if (apiError) {
            setApiError("");
        }
    };

    const validateForm = () => {
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
            !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
                email
            )
        ) {
            newErrors.email =
                "Enter a valid email address.";
        }

        if (!password) {
            newErrors.password =
                "Password is required.";
        } else if (
            password.length < 8
        ) {
            newErrors.password =
                "Password must be at least 8 characters.";
        }

        if (!confirmPassword) {
            newErrors.confirmPassword =
                "Please confirm your password.";
        } else if (
            password !== confirmPassword
        ) {
            newErrors.confirmPassword =
                "Passwords do not match.";
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
            await registerUser({
                firstName:
                    firstName.trim(),
                lastName:
                    lastName.trim(),
                email:
                    email.trim(),
                password,
            });

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
                    : "Unable to create account."
            );
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <AuthLayout>

            <div className="auth-heading">
                <h1>
                    Create your account
                </h1>

                <p>
                    Start building your EngineerOS workspace.
                </p>
            </div>

            <form
                className="auth-form"
                onSubmit={handleSubmit}
            >

                <div className="name-fields">

                    <Input
                        id="firstName"
                        type="text"
                        label="First name"
                        placeholder="First name"
                        autoComplete="given-name"
                        value={firstName}
                        error={errors.firstName}
                        onChange={(event) => {
                            setFirstName(
                                event.target.value
                            );

                            clearFieldError(
                                "firstName"
                            );
                        }}
                    />

                    <Input
                        id="lastName"
                        type="text"
                        label="Last name"
                        placeholder="Last name"
                        autoComplete="family-name"
                        value={lastName}
                        error={errors.lastName}
                        onChange={(event) => {
                            setLastName(
                                event.target.value
                            );

                            clearFieldError(
                                "lastName"
                            );
                        }}
                    />

                </div>

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

                        clearFieldError(
                            "email"
                        );
                    }}
                />

                <div className="form-group">

                    <label htmlFor="password">
                        Password
                    </label>

                    <div className="password-wrapper">

                        <Input
                            id="password"
                            type={
                                showPassword
                                    ? "text"
                                    : "password"
                            }
                            placeholder="Create a password"
                            autoComplete="new-password"
                            value={password}
                            error={errors.password}
                            onChange={(event) => {
                                setPassword(
                                    event.target.value
                                );

                                clearFieldError(
                                    "password"
                                );
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

                <div className="form-group">

                    <label htmlFor="confirmPassword">
                        Confirm password
                    </label>

                    <div className="password-wrapper">

                        <Input
                            id="confirmPassword"
                            type={
                                showConfirmPassword
                                    ? "text"
                                    : "password"
                            }
                            placeholder="Enter password again"
                            autoComplete="new-password"
                            value={confirmPassword}
                            error={
                                errors.confirmPassword
                            }
                            onChange={(event) => {
                                setConfirmPassword(
                                    event.target.value
                                );

                                clearFieldError(
                                    "confirmPassword"
                                );
                            }}
                        />

                        <button
                            type="button"
                            className="password-toggle"
                            onClick={() =>
                                setShowConfirmPassword(
                                    (current) =>
                                        !current
                                )
                            }
                        >
                            {showConfirmPassword
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
                    Create account
                </Button>

            </form>

            <div className="auth-divider">
                <span>OR</span>
            </div>

            <GoogleButton
                text="Sign up with Google"
            />

            <p className="auth-switch-text">
                Already have an account?{" "}
                <Link to="/login">
                    Sign in
                </Link>
            </p>

        </AuthLayout>
    );
}