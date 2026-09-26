import type { RepositoryStructureFile } from "../../types/repository";

type RepositoryExplorerProps = {
    files: RepositoryStructureFile[];
    selectedFile: RepositoryStructureFile | null;
    onSelectFile: (file: RepositoryStructureFile) => void;
};

type FileTreeNode = {
    name: string;
    folders: Map<string, FileTreeNode>;
    files: RepositoryStructureFile[];
};

function createTree(files: RepositoryStructureFile[]): FileTreeNode {
    const root: FileTreeNode = {
        name: "",
        folders: new Map(),
        files: [],
    };

    for (const file of files) {
        const parts = file.path.split("/").filter(Boolean);
        const fileName = parts.pop();

        if (!fileName) {
            continue;
        }

        let currentNode = root;

        for (const folderName of parts) {
            let folder = currentNode.folders.get(folderName);

            if (!folder) {
                folder = {
                    name: folderName,
                    folders: new Map(),
                    files: [],
                };

                currentNode.folders.set(folderName, folder);
            }

            currentNode = folder;
        }

        currentNode.files.push(file);
    }

    return root;
}

type FolderTreeProps = {
    node: FileTreeNode;
    depth: number;
    selectedFile: RepositoryStructureFile | null;
    onSelectFile: (file: RepositoryStructureFile) => void;
};

function FolderTree({
    node,
    depth,
    selectedFile,
    onSelectFile,
}: FolderTreeProps) {
    const folders = Array.from(node.folders.values()).sort((left, right) =>
        left.name.localeCompare(right.name)
    );

    const files = [...node.files].sort((left, right) =>
        left.name.localeCompare(right.name)
    );

    return (
        <ul className="repository-tree">
            {folders.map((folder) => (
                <li key={folder.name} className="repository-tree-folder">
                    <details open={depth < 2}>
                        <summary>📁 {folder.name}</summary>

                        <FolderTree
                            node={folder}
                            depth={depth + 1}
                            selectedFile={selectedFile}
                            onSelectFile={onSelectFile}
                        />
                    </details>
                </li>
            ))}

            {files.map((file) => (
                <li key={file.id} className="repository-tree-file">
                    <button
                        type="button"
                        className={
                            selectedFile?.id === file.id
                                ? "repository-file-button selected"
                                : "repository-file-button"
                        }
                        onClick={() => onSelectFile(file)}
                    >
                        📄 {file.name}
                    </button>
                </li>
            ))}
        </ul>
    );
}

export default function RepositoryExplorer({
    files,
    selectedFile,
    onSelectFile,
}: RepositoryExplorerProps) {
    const tree = createTree(files);

    return (
        <aside className="repository-explorer">
            <h2>Repository explorer</h2>

            {files.length === 0 ? (
                <p>No analysed files found.</p>
            ) : (
                <FolderTree
                    node={tree}
                    depth={0}
                    selectedFile={selectedFile}
                    onSelectFile={onSelectFile}
                />
            )}
        </aside>
    );
}
