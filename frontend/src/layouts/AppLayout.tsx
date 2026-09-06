import {
    useState,
    type ReactNode,
} from "react";

import Header from "../components/dashboard/Header";
import Sidebar from "../components/dashboard/Sidebar";

import "../styles/dashboard.css";

type AppLayoutProps = {
    children: ReactNode;
};

export default function AppLayout({
    children,
}: AppLayoutProps) {
    const [
        isSidebarOpen,
        setIsSidebarOpen,
    ] = useState(false);

    return (
        <div className="app-shell">
            <Sidebar
                isOpen={isSidebarOpen}
                onClose={() =>
                    setIsSidebarOpen(false)
                }
            />

            <div className="app-main">
                <Header
                    onMenuClick={() =>
                        setIsSidebarOpen(
                            (current) =>
                                !current
                        )
                    }
                />

                <main className="app-content">
                    {children}
                </main>
            </div>
        </div>
    );
}