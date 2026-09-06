import type { ReactNode } from "react";

type PageHeaderProps = {
    title: string;
    description?: string;
    action?: ReactNode;
};

export default function PageHeader({
    title,
    description,
    action,
}: PageHeaderProps) {
    return (
        <div className="page-header">
            <div className="page-header-content">
                <h1>{title}</h1>

                {description && (
                    <p>{description}</p>
                )}
            </div>

            {action && (
                <div className="page-header-action">
                    {action}
                </div>
            )}
        </div>
    );
}