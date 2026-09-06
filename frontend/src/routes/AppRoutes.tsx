import { Routes, Route, Navigate } from "react-router-dom";

import LoginPage from "../pages/LoginPage";
import RegisterPage from "../pages/RegisterPage";
import DashboardPage from "../pages/DashboardPage";
import RepositoriesPage from "../pages/RepositoriesPage";
import SettingsPage from "../pages/SettingsPage";
import LearningPage from "../pages/LearningPage";
import AIMentorPage from "../pages/AIMentorPage";
import ProtectedRoute from "./ProtectedRoute";
import RepositoryDetailsPage from "../pages/RepositoryDetailsPage";
import RepositoryUploadPage from "../pages/RepositoryUploadPage";
export default function AppRoutes() {
    return (
        <Routes>
            <Route
                path="/"
                element={<Navigate to="/dashboard" replace />}
            />

            <Route
                path="/login"
                element={<LoginPage />}
            />

            <Route
                path="/register"
                element={<RegisterPage />}
            />

            <Route
                path="/dashboard"
                element={
                    <ProtectedRoute>
                        <DashboardPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/repositories"
                element={
                    <ProtectedRoute>
                        <RepositoriesPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/repositories/:repositoryId"
                element={
                    <ProtectedRoute>
                        <RepositoryDetailsPage />
                    </ProtectedRoute>
                }
            />
            <Route
                path="/repositories/upload"
                element={
                    <ProtectedRoute>
                        <RepositoryUploadPage />
                    </ProtectedRoute>
                }
            />


            <Route
                path="/learning"
                element={
                    <ProtectedRoute>
                        <LearningPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/mentor"
                element={
                    <ProtectedRoute>
                        <AIMentorPage />
                    </ProtectedRoute>
                }
            />

            <Route
                path="/settings"
                element={
                    <ProtectedRoute>
                        <SettingsPage />
                    </ProtectedRoute>
                }
            />
          
        </Routes>
    );
}