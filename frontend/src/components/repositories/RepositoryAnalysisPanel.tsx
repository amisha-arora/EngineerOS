import type {
    RepositoryDependency,
    RepositoryStructureFile,
} from "../../types/repository";

type RepositoryAnalysisPanelProps = {
    selectedFile: RepositoryStructureFile | null;
    dependencies: RepositoryDependency[];
};

export default function RepositoryAnalysisPanel({
    selectedFile,
    dependencies,
}: RepositoryAnalysisPanelProps) {
    if (!selectedFile) {
        return (
            <section className= "analysis-panel" >
            <h2>Select a file </h2>

                <p>
                    Select a file from the repository explorer.
                </p>
            </section>
        );
    }

    const typeNames = selectedFile.types.map(
        (type) => type.name
    );

    const fileDependencies = dependencies.filter(
        (dependency) =>
            typeNames.includes(
                dependency.sourceTypeName
            )
    );

    return (
        <section className= "analysis-panel" >
        <p className="analysis-file-path" >
        { selectedFile.path }
            </p>

            < h2 > { selectedFile.name } </h2>

    {
        selectedFile.types.map((type) => (
            <article
                    key= { type.id }
                    className = "analysis-type-card"
            >
            <h3>
            { type.accessModifier }{ " "}
                        { type.modifier ?? "" }{ " "}
                        { type.kind.toLowerCase() }{ " "}
                        { type.name }
            </h3>

            <p>
                        Namespace: { type.namespace }
        </p>

        < h4 > Methods </h4>

                    {
                type.methods.length === 0 ? (
                    <p>No methods found.</p>
                    ) : (
            <ul>
            {
                type.methods.map((method) => (
                    <li key= { method.id } >
                    { method.accessModifier }{ " "}
                                    { method.returnType }{ " "}
                                    { method.name }()
                                    { " — line "}
                                    { method.lineNumber }
                    </li>
                ))
            }
            </ul>
        )
    }
    </article>
            ))
}

<h3>Dependencies </h3>

{
    fileDependencies.length === 0 ? (
        <p>No dependencies found.</p>
            ) : (
        <ul className= "analysis-dependencies" >
        {
            fileDependencies.map((dependency) => (
                <li key= { dependency.id } >
                <strong>
                { dependency.sourceTypeName }
                </strong>
                            { " → "}
                            { dependency.dependencyType }
                            { " → "}
                <strong>
                                { dependency.targetTypeName }
                </strong>
                            { " (line "}
                            { dependency.lineNumber }
                            { ")"}
                </li>
            ))
        }
        </ul>
            )
}
</section>
    );
}