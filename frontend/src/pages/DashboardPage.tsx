import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import AppLayout from "../layouts/AppLayout";
import StatCard from "../components/dashboard/StatCard";

import { useAuth } from "../features/auth/AuthContext";
import { getRepositories } from "../api/repositoryApi";

export default function DashboardPage() {
    const { user } = useAuth();
    const navigate = useNavigate();

    const [repositoryCount, setRepositoryCount] =
        useState(0);

    const [isRepositoriesLoading, setIsRepositoriesLoading] =
        useState(true);

    useEffect(() => {
        const loadRepositories = async () => {
            try {
                const repositories =
                    await getRepositories();

                setRepositoryCount(
                    repositories.length
                );
            } catch (error) {
                console.error(
                    "Unable to load repositories:",
                    error
                );
            } finally {
                setIsRepositoriesLoading(false);
            }
        };

        loadRepositories();
    }, []);

    return (
        <AppLayout>
            <div className="dashboard-page">

                <section className="dashboard-welcome">
                    <div>
                        <h1>
                            Welcome back, {user?.firstName}
                        </h1>

                        <p>
                            Here's what's happening in your
                            engineering workspace.
                        </p>
                    </div>
                </section>

                <section className="dashboard-stats">

                    <StatCard
                        title="Repositories"
                        value={
                            isRepositoriesLoading
                                ? "..."
                                : repositoryCount
                        }
                        description="Repositories in your workspace"
                    />

                    <StatCard
                        title="Recent Analyses"
                        value={0}
                        description="Repository analyses"
                        isPlaceholder
                    />

                    <StatCard
                        title="Learning Progress"
                        value="0%"
                        description="Overall learning progress"
                        isPlaceholder
                    />

                </section>

                <section className="dashboard-grid">

                    <div className="dashboard-panel">
                        <div className="panel-header">
                            <div>
                                <h2>
                                    Recent Questions
                                </h2>

                                <p>
                                    Your latest engineering
                                    questions
                                </p>
                            </div>

                            <span className="placeholder-badge">
                                Demo
                            </span>
                        </div>

                        <div className="empty-state">
                            <p>
                                Your recent questions will
                                appear here.
                            </p>
                        </div>
                    </div>

                    <div className="dashboard-panel">
                        <div className="panel-header">
                            <div>
                                <h2>AI Mentor</h2>

                                <p>
                                    Your engineering assistant
                                </p>
                            </div>

                            <span className="placeholder-badge">
                                Demo
                            </span>
                        </div>

                        <div className="mentor-content">
                            <h3>
                                Need help understanding something?
                            </h3>

                            <p>
                                Ask your AI Mentor about your
                                repositories, architecture,
                                or code.
                            </p>

                            <button
                                type="button"
                                className="secondary-action-button"
                                onClick={() =>
                                    navigate("/mentor")
                                }
                            >
                                Open AI Mentor
                            </button>
                        </div>
                    </div>

                </section>

                <section className="quick-actions-section">

                    <div className="section-heading">
                        <h2>Quick Actions</h2>

                        <p>
                            Jump back into your engineering
                            workflow.
                        </p>
                    </div>

                    <div className="quick-actions-grid">

                        <button
                            type="button"
                            className="quick-action-card"
                            onClick={() =>
                                navigate("/repositories")
                            }
                        >
                            <strong>
                                Upload Repository
                            </strong>

                            <span>
                                Add a repository to your
                                workspace.
                            </span>
                        </button>

                        <button
                            type="button"
                            className="quick-action-card"
                            onClick={() =>
                                navigate("/repositories")
                            }
                        >
                            <strong>
                                Analyze Repository
                            </strong>

                            <span>
                                Start understanding an
                                existing codebase.
                            </span>
                        </button>

                        <button
                            type="button"
                            className="quick-action-card"
                            onClick={() =>
                                navigate("/learning")
                            }
                        >
                            <strong>
                                Continue Learning
                            </strong>

                            <span>
                                Resume your learning path.
                            </span>
                        </button>

                        <button
                            type="button"
                            className="quick-action-card"
                            onClick={() =>
                                navigate("/mentor")
                            }
                        >
                            <strong>
                                Open AI Mentor
                            </strong>

                            <span>
                                Ask questions about your
                                code or architecture.
                            </span>
                        </button>

                    </div>

                </section>

            </div>
        </AppLayout>
    );
}