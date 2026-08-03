# EngineerOS Database Design

## 1. Document Information

## 2. Database Overview

## 3. Technology Choice

## 4. Entity Definitions

### 4.1 User
### 4.2 Repository
### 4.3 RepositoryAnalysis
### 4.4 CodeFile
### 4.5 CodeSymbol
### 4.6 CodeDependency
### 4.7 CodeChunk
### 4.8 ChatSession
### 4.9 ChatMessage
### 4.10 LearningRoadmap
### 4.11 RoadmapStep
### 4.12 RefreshToken

## 5. Entity Relationships

## 6. Indexing Strategy

## 7. Data Security

## 8. Assumptions and Constraints

## 9. ER Diagram
## Indexing Strategy

The following database indexes are proposed to improve query performance as the application grows:

- User.Email — Unique index for authentication and duplicate email prevention.
- Repository.UserId — Index for retrieving repositories belonging to a user.
- Repository.Status — Index for filtering repositories by processing status.
- CodeFile.RepositoryId — Index for retrieving files belonging to a repository.
- CodeSymbol.CodeFileId — Index for retrieving symbols belonging to a code file.
- CodeSymbol.FullyQualifiedName — Index for symbol lookup.
- CodeChunk.RepositoryId — Index for repository-specific semantic search.
- ChatSession.RepositoryId — Index for retrieving repository conversations.
- RoadmapStep.LearningRoadmapId — Index for retrieving roadmap steps.

Vector similarity indexing for CodeChunk.Embedding will be configured when semantic search is implemented using pgvector.

![EngineerOS ER Diagram](../diagrams/EngineerOS-ER-Diagram.png)