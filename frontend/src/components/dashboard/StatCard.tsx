type StatCardProps = {
    title: string;
    value: string | number;
    description: string;
    isPlaceholder?: boolean;
};

export default function StatCard({
    title,
    value,
    description,
    isPlaceholder = false,
}: StatCardProps) {
    return (
        <article className="stat-card">
            <div className="stat-card-header">
                <span className="stat-card-title">
                    {title}
                </span>

                {isPlaceholder && (
                    <span className="placeholder-badge">
                        Demo
                    </span>
                )}
            </div>

            <strong className="stat-card-value">
                {value}
            </strong>

            <span className="stat-card-description">
                {description}
            </span>
        </article>
    );
}