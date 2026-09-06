import { NavLink } from "react-router-dom";

type SidebarProps = {
    isOpen?: boolean;
    onClose?: () => void;
};

export default function Sidebar({
    isOpen = false,
    onClose,
}: SidebarProps) {
    return (
        <>
            {isOpen && (
                <button
                    type="button"
                    className="sidebar-overlay"
                    onClick={onClose}
                    aria-label="Close navigation menu"
                />
            )}

            <aside
                className={
                    isOpen
                        ? "sidebar sidebar-open"
                        : "sidebar"
                }
            >
                <div className="sidebar-brand">
                    <span className="sidebar-brand-mark">
                        E
                    </span>

                    <span className="sidebar-brand-text">
                        EngineerOS
                    </span>
                </div>

                <nav className="sidebar-nav">
                    <NavLink
                        to="/dashboard"
                        onClick={onClose}
                        className={({ isActive }) =>
                            isActive
                                ? "sidebar-link active"
                                : "sidebar-link"
                        }
                    >
                        Dashboard
                    </NavLink>

                    <NavLink
                        to="/repositories"
                        onClick={onClose}
                        className={({ isActive }) =>
                            isActive
                                ? "sidebar-link active"
                                : "sidebar-link"
                        }
                    >
                        Repositories
                    </NavLink>

                    <NavLink
                        to="/learning"
                        onClick={onClose}
                        className={({ isActive }) =>
                            isActive
                                ? "sidebar-link active"
                                : "sidebar-link"
                        }
                    >
                        Learning
                    </NavLink>

                    <NavLink
                        to="/mentor"
                        onClick={onClose}
                        className={({ isActive }) =>
                            isActive
                                ? "sidebar-link active"
                                : "sidebar-link"
                        }
                    >
                        AI Mentor
                    </NavLink>

                    <NavLink
                        to="/settings"
                        onClick={onClose}
                        className={({ isActive }) =>
                            isActive
                                ? "sidebar-link active"
                                : "sidebar-link"
                        }
                    >
                        Settings
                    </NavLink>
                </nav>
            </aside>
        </>
    );
}