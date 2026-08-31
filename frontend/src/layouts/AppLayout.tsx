import type { ReactNode } from "react";
import Header from "../components/dashboard/Header";
import Sidebar from "../components/dashboard/Sidebar";
import "../styles/dashboard.css";

type AppLayoutProps = {
    children: ReactNode;
};

export default function AppLayout({
    children,
}: AppLayoutProps) {
    return (
        <div className="app-shell">
            <Sidebar />

            <div className="app-main">
                <Header />

                <main className="app-content">
                    {children}
                </main>
            </div>
        </div>
    );
}