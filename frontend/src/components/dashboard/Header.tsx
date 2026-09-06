import { useNavigate } from "react-router-dom";
import { useAuth } from "../../features/auth/AuthContext";

type HeaderProps = {
    onMenuClick?: () => void;
};

export default function Header({
    onMenuClick,
}: HeaderProps) {
    const { user, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = async () => {
        await logout();

        navigate("/login", {
            replace: true,
        });
    };

    return (
        <header className="app-header">
            <div className="header-left">
                <button
                    type="button"
                    className="mobile-menu-button"
                    onClick={onMenuClick}
                    aria-label="Open navigation menu"
                >
                    ☰
                </button>

                <h2 className="header-title">
                    EngineerOS
                </h2>
            </div>

            <div className="header-user">
                <div className="user-details">
                    <div className="user-details">
                    <span className="user-name">
                        {user?.firstName}{" "}
                        {user?.lastName}
                    </span>

                    <span className="user-email">
                        {user?.email}
                    </span>
                </div>

                <button
                    type="button"
                    className="logout-button"
                    onClick={handleLogout}
                >
                    Logout
                </button>
            </div>
        </header>
    );
}