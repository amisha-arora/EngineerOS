export type RepositoryListItem = {
    id: string;
    name: string;
    url: string;
    createdAtUtc: string;
};
export type RepositoryDetails = {
    id: string;
    name: string;
    url: string;
    createdAtUtc: string;
};
export type AnalyzeRepositoryResponse = {
    analysisId: string;
    status: string;
    fileCount: number;
    typeCount: number;
    methodCount: number;
    dependencyCount: number;
};

export type RepositoryStructureMethod = {
    id: string;
    name: string;
    returnType: string;
    accessModifier: string;
    lineNumber: number;
};

export type RepositoryStructureType = {
    id: string;
    name: string;
    namespace: string;
    kind: string;
    accessModifier: string;
    modifier: string | null;
    methods: RepositoryStructureMethod[];
};

export type RepositoryStructureFile = {
    id: string;
    name: string;
    path: string;
    extension: string;
    type: string;
    size: number;
    parentDirectory: string;
    types: RepositoryStructureType[];
};

export type RepositoryStructure = {
    repositoryId: string;
    files: RepositoryStructureFile[];
};

export type RepositoryDependency = {
    id: string;
    sourceCodeClassId: string | null;
    sourceTypeName: string;
    targetCodeClassId: string | null;
    targetTypeName: string;
    dependencyType: string;
    lineNumber: number;
};

export type RepositoryDependencies = {
    repositoryId: string;
    dependencies: RepositoryDependency[];
};