import { useNavigate } from "react-router-dom";

import AppLayout from "../layouts/AppLayout";

import { useAuth } from "../features/auth/AuthContext";

import "../styles/settings.css";

export default function SettingsPage() {
    const {
        user,
        logout,
    } = useAuth();

    const navigate = useNavigate();

    const handleLogout = async () => {
        await logout();

        navigate(
            "/login",
            {
                replace: true,
            }
        );
    };

    return (
        <AppLayout>
            <div className="settings-page">

                <div className="settings-heading">
                    <h1>Settings</h1>

                    <p>
                        Manage your EngineerOS account and
                        application preferences.
                    </p>
                </div>

                <section className="settings-section">

                    <div className="settings-section-heading">
                        <h2>Profile</h2>

                        <p>
                            Your personal information from
                            your EngineerOS account.
                        </p>
                    </div>

                    <div className="settings-card">

                        <div className="settings-profile-header">

                            <div className="settings-avatar">
                                {user?.firstName
                                    ?.charAt(0)
                                    .toUpperCase()}
                            </div>

                            <div>
                                <h3>
                                    {user?.firstName}{" "}
                                    {user?.lastName}
                                </h3>

                                <p>
                                    {user?.email}
                                </p>
                            </div>

                        </div>

                        <div className="settings-grid">

                            <div className="settings-field">
                                <span>First name</span>

                                <strong>
                                    {user?.firstName}
                                </strong>
                            </div>

                            <div className="settings-field">
                                <span>Last name</span>

                                <strong>
                                    {user?.lastName}
                                </strong>
                            </div>

                            <div className="settings-field settings-field-full">
                                <span>Email address</span>

                                <strong>
                                    {user?.email}
                                </strong>
                            </div>

                        </div>

                        <div className="settings-info">
                            Profile editing is not available
                            in the current backend phase.
                        </div>

                    </div>

                </section>

                <section className="settings-section">

                    <div className="settings-section-heading">
                        <h2>Account</h2>

                        <p>
                            Information associated with your
                            EngineerOS account.
                        </p>
                    </div>

                    <div className="settings-card">

                        <div className="settings-row">
                            <div>
                                <strong>Account ID</strong>

                                <p>
                                    Unique identifier for your
                                    EngineerOS account.
                                </p>
                            </div>

                            <code className="account-id">
                                {user?.id}
                            </code>
                        </div>

                        <div className="settings-divider" />

                        <div className="settings-row">
                            <div>
                                <strong>Account status</strong>

                                <p>
                                    Your account is currently
                                    active.
                                </p>
                            </div>

                            <span className="account-status">
                                Active
                            </span>
                        </div>

                    </div>

                </section>

                <section className="settings-section">

                    <div className="settings-section-heading">
                        <h2>Appearance</h2>

                        <p>
                            Customize how EngineerOS looks.
                        </p>
                    </div>

                    <div className="settings-card">

                        <div className="settings-row">

                            <div>
                                <strong>Theme</strong>

                                <p>
                                    Choose the appearance of
                                    your workspace.
                                </p>
                            </div>

                            <select
                                className="theme-select"
                                defaultValue="dark"
                                disabled
                            >
                                <option value="dark">
                                    Dark
                                </option>

                                <option value="light">
                                    Light
                                </option>
                            </select>

                        </div>

                        <div className="settings-info">
                            EngineerOS currently uses the dark
                            theme. Theme switching will be
                            enabled in a future phase.
                        </div>

                    </div>

                </section>

                <section className="settings-section">

                    <div className="settings-section-heading">
                        <h2>Security</h2>

                        <p>
                            Manage authentication and your
                            current session.
                        </p>
                    </div>

                    <div className="settings-card">

                        <div className="settings-row">

                            <div>
                                <strong>Password</strong>

                                <p>
                                    Password changes are not
                                    currently supported by the
                                    backend.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="settings-secondary-button"
                                disabled
                            >
                                Change password
                            </button>

                        </div>

                        <div className="settings-divider" />

                        <div className="settings-row">

                            <div>
                                <strong>Sign out</strong>

                                <p>
                                    End your current EngineerOS
                                    session.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="settings-danger-button"
                                onClick={handleLogout}
                            >
                                Logout
                            </button>

                        </div>

                    </div>

                </section>

            </div>
        </AppLayout>
    );
}  