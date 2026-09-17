# Repository Analysis

## Purpose

EngineerOS analyzes software repositories to understand their
structure, C# code, and relationships between code elements.

## Analysis Pipeline

Repository ZIP
↓
Upload
↓
Storage
↓
Extraction
↓
File Discovery
↓
C# Parsing
↓
Class and Method Extraction
↓
Dependency Analysis
↓
Metadata Storage
↓
Project Explorer

## Storage

Uploaded repositories are initially stored on the local filesystem.

Example:

storage/
└── repositories/
    └── {repositoryId}/
        ├── repository.zip
        └── extracted/
            └── repository files

## Analysis Model

Repository
    ↓
RepositoryAnalysis
    ↓
RepositoryFile
    ↓
CodeClass
    ↓
CodeMethod

CodeClass
    ↓
CodeDependency
    ↑
CodeClass

## Analysis Status

An analysis can have the following states:

- Pending
- Processing
- Completed
- Failed

## Responsibilities

### Repository Storage

Stores uploaded repository archives.

### Repository Extraction

Extracts repository archives into a working directory.

### File Discovery

Discovers files and folders inside the repository.

### C# Analysis

Parses C# source files and extracts namespaces,
classes, interfaces, enums, records, and methods.

### Dependency Analysis

Identifies relationships between code elements.

### Metadata Storage

Stores analysis results in PostgreSQL.

### Project Explorer

Displays repository structure, code elements,
and relationships to the user.

## Initial Scope

The first implementation focuses on C# repositories.

Future versions may support additional languages.