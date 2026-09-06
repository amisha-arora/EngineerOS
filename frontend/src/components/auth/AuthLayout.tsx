import type { ReactNode } from "react";

import engineerOsLogo from "../../assets/engineeros-logo.png";

import "../../styles/auth.css";

type AuthLayoutProps = {
    children: ReactNode;
};

export default function AuthLayout({
    children,
}: AuthLayoutProps) {
    return (
        <main className="auth-page">

            <header className="auth-header">
                <img
                    src={engineerOsLogo}
                    alt="EngineerOS"
                    className="auth-logo"
                />
            </header>

            <section className="auth-center">

                <div className="auth-content">
                    {children}
                </div>

            </section>

        </main>
    );
}