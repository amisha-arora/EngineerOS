# EngineerOS Knowledge Base

## 1. Purpose

The EngineerOS Knowledge Base provides a searchable representation of both repository code and repository documentation.

The Knowledge Base is introduced in Phase 4.

The goal is to allow a developer to ask questions about a repository and retrieve relevant code and documentation context using semantic search.

The AI Mentor and RAG system will be implemented in Phase 5.

Phase 4 is responsible for:

* preparing repository knowledge
* generating embeddings
* storing vectors
* retrieving relevant knowledge

Phase 5 will use the retrieved knowledge to generate AI explanations and answers.

---

## 2. Knowledge Sources

EngineerOS has two primary knowledge sources:

```text
Repository
│
├── Code Knowledge
│
└── Documentation Knowledge
```

### Code Knowledge

Code knowledge comes from the repository analysis performed in Phase 3.

Phase 3 already extracts:

* Repository files
* C# classes
* C# methods
* C# dependencies
* namespaces
* relationships

Phase 4 will make this structured code information searchable.

The existing Phase 3 analysis system remains responsible for parsing and understanding the code.

Phase 4 is responsible for converting useful code information into searchable knowledge.

### Documentation Knowledge

Documentation knowledge comes from supported documentation files:

* Markdown
* Text
* PDF
* DOCX

---

## 3. Existing Phase 3 Knowledge

Phase 4 builds on the entities already produced by repository analysis.

```text
Repository
    |
    ├── RepositoryFile
    ├── CodeClass
    ├── CodeMethod
    └── CodeDependency
```

These entities are not replaced by the Knowledge Base.

Instead, Phase 4 creates searchable representations from them.

For example:

```text
CodeClass
    ↓
Searchable Code Knowledge
    ↓
Embedding
```

and:

```text
CodeMethod
    ↓
Searchable Code Knowledge
    ↓
Embedding
```

This allows questions such as:

* Where is login implemented?
* What does AuthService do?
* Which repository does UserService depend on?
* What methods exist in UserService?
* Which classes implement IUserRepository?

---

## 4. Documentation Model

Documentation is represented separately from the existing repository-file model.

```text
Repository
    |
    └── Document
          |
          └── DocumentChunk
                |
                └── Embedding
```

A `RepositoryFile` represents any file discovered inside a repository.

A `Document` represents a repository file containing documentation that EngineerOS should index.

A `DocumentChunk` represents a meaningful portion of a document.

An `Embedding` represents the vector representation of searchable knowledge.

---

## 5. Supported Documentation Types

The initial version of EngineerOS supports:

| Extension | Document Type |
| --------- | ------------- |
| `.md`     | Markdown      |
| `.txt`    | Text          |
| `.pdf`    | PDF           |
| `.docx`   | DOCX          |

The first implementation will focus only on these formats.

Other formats may be supported in future phases.

---

## 6. Document

A `Document` represents a documentation source belonging to a repository.

### Fields

```text
Id
RepositoryId
Name
RelativePath
DocumentType
FileSize
CreatedAt
```

### Example

```text
Name:
authentication.md

RelativePath:
docs/authentication.md

DocumentType:
Markdown
```

The `RepositoryId` ensures that documents remain associated with the correct repository.

The `RelativePath` is preserved so that search results can identify the original source.

---

## 7. Document Chunk

A document may contain a large amount of text.

Instead of generating one embedding for an entire document, the document is divided into smaller meaningful chunks.

```text
Document
    |
    ├── Chunk 0
    ├── Chunk 1
    ├── Chunk 2
    └── Chunk 3
```

### Fields

```text
Id
DocumentId
Content
ChunkIndex
StartOffset
EndOffset
Metadata
```

A chunk should contain enough information to be useful independently during retrieval.

---

## 8. Searchable Code Knowledge

Code should not be treated exactly like documentation.

Phase 3 already understands the structure of the codebase.

For example:

```text
UserService.cs
    |
    └── UserService
          |
          ├── GetUser()
          ├── DeleteUser()
          └── IUserRepository
```

Phase 4 should preserve this structure while also creating searchable representations.

A searchable representation may contain information such as:

```text
Class: UserService

Namespace:
EngineerOS.Application.Services

Methods:
- GetUser(Guid id) : User
- DeleteUser(Guid id) : void

Dependencies:
- IUserRepository

File:
Application/Services/UserService.cs
```

This representation can then be embedded.

The structured Phase 3 entities remain the source of truth.

The searchable representation exists specifically for retrieval.

---

## 9. Code Knowledge Types

The initial searchable code knowledge should include:

```text
Code File
Code Class
Code Method
Code Dependency
```

Examples:

```text
CodeClass
    ↓
UserService
```

```text
CodeMethod
    ↓
UserService.GetUser()
```

```text
CodeDependency
    ↓
UserService → IUserRepository
```

This allows semantic search across the structure already extracted during Phase 3.

---

## 10. Embedding

An embedding represents the semantic meaning of a searchable knowledge item as a vector.

Knowledge that may receive embeddings includes:

```text
Documentation Chunk
Code Class
Code Method
Code Dependency
```

Conceptually:

```text
Documentation
      ↓
DocumentChunk
      ↓
Embedding


Code
      ↓
Searchable Code Knowledge
      ↓
Embedding
```

### Embedding fields

```text
Id
KnowledgeItemId
Vector
Model
Dimensions
CreatedAt
```

The exact database representation of the relationship between knowledge items and embeddings will be finalized during the database implementation.

The vector will be stored in PostgreSQL using `pgvector`.

---

## 11. What Gets Embedded

EngineerOS embeds meaningful textual representations of knowledge.

For documentation:

```text
"Refresh tokens are stored in PostgreSQL and associated
with the authenticated user."
```

For code:

```text
"Class UserService in namespace
EngineerOS.Application.Services.

Methods:
GetUser(Guid id) returns User.
DeleteUser(Guid id) returns void.

Dependency:
IUserRepository."
```

The goal is to create representations that contain enough semantic context for meaningful retrieval.

---

## 12. What Does Not Get Embedded

Pure database metadata is not embedded.

Examples:

```text
Id
RepositoryId
FileSize
CreatedAt
ChunkIndex
StartOffset
EndOffset
```

These remain normal database fields.

They can be used for:

* filtering
* source identification
* navigation
* displaying search results

---

## 13. Metadata

Metadata describes where searchable knowledge came from.

Possible metadata includes:

```text
RepositoryId
FileId
DocumentId
RelativePath
DocumentType
ClassId
MethodId
Namespace
Section
Heading
PageNumber
```

Metadata allows EngineerOS to identify the source of a search result.

For example:

```text
Knowledge:
UserService.GetUser()

Source:
Application/Services/UserService.cs
```

or:

```text
Knowledge:
Refresh token explanation

Source:
docs/authentication.md
```

---

## 14. Chunking Strategy

Documentation will be divided into meaningful pieces before embeddings are generated.

The initial strategy will prefer logical document boundaries:

```text
Document
    ↓
Heading / Section
    ↓
Paragraphs
    ↓
Chunks
```

The chunking implementation will be refined during Day 5 of Phase 4.

Code will use the structured entities already extracted by Phase 3 rather than blindly splitting C# source files into arbitrary text chunks.

---

## 15. Embedding Strategy

The overall indexing pipeline will be:

```text
Repository
    |
    ├── Code
    |    ↓
    |  Phase 3 Analysis
    |    ↓
    |  Searchable Code Knowledge
    |
    └── Documentation
         ↓
       Extract Text
         ↓
       Document Chunks
         
             ↓
      Searchable Knowledge
             ↓
      Generate Embeddings
             ↓
    PostgreSQL + pgvector
```

An embedding service abstraction will be used so that the embedding provider can be changed without changing the rest of the Knowledge Base architecture.

---

## 16. Search Flow

The semantic search flow will be:

```text
User Query
    ↓
Generate Query Embedding
    ↓
Search PostgreSQL + pgvector
    ↓
Find Similar Knowledge
    ↓
Filter By Repository
    ↓
Rank Results
    ↓
Return Relevant Code + Documentation
    ↓
Display Source
```

For example:

```text
Question:

"How does authentication work?"
```

Possible results:

```text
1. AuthController.Login()
   AuthController.cs

2. AuthService.Authenticate()
   AuthService.cs

3. JwtService.GenerateToken()
   JwtService.cs

4. docs/authentication.md
   JWT authentication explanation
```

Another question:

```text
"Where are refresh tokens stored?"
```

may return:

```text
1. RefreshTokenRepository.cs
2. RefreshToken.cs
3. docs/authentication.md
4. docs/database.md
```

---

## 17. Search Result

A search result should retain enough information to identify both the retrieved knowledge and its source.

Conceptually:

```text
SearchResult
-------------------------
Content
KnowledgeType
Score
RepositoryId
FileId
DocumentId
RelativePath
ClassId
MethodId
```

Not every field will apply to every result.

For example, a documentation result may have:

```text
KnowledgeType:
Documentation

RelativePath:
docs/authentication.md
```

while a code result may have:

```text
KnowledgeType:
CodeMethod

RelativePath:
Application/Auth/AuthService.cs

Class:
AuthService

Method:
Login()
```

---

## 18. Initial Design Decisions

### Decision 1 — PostgreSQL + pgvector

PostgreSQL will remain the primary database.

`pgvector` will be used for vector storage and similarity search.

### Decision 2 — Code and Documentation Are Both Knowledge

EngineerOS must be able to retrieve information from both:

```text
Repository Code
+
Repository Documentation
```

This is required because developers may ask questions about implementation details that exist only in source code.

### Decision 3 — Phase 3 Remains the Code Source of Truth

Phase 4 does not replace the Phase 3 code-analysis system.

Phase 3 remains responsible for:

```text
Files
Classes
Methods
Dependencies
Relationships
```

Phase 4 makes this information searchable.

### Decision 4 — Documentation Is Chunked

Documentation is divided into meaningful chunks before embeddings are generated.

### Decision 5 — Code Uses Structured Knowledge

Code is not treated as arbitrary text.

Existing classes, methods, and dependencies are converted into meaningful searchable representations.

### Decision 6 — Preserve Source Information

Every searchable knowledge item must remain connected to its source repository and source file or document.

This is necessary for:

* repository-aware search
* source navigation
* future citations
* RAG

### Decision 7 — Retrieval and Generation Are Separate

Phase 4 is responsible for:

```text
Query
  ↓
Semantic Search
  ↓
Relevant Code + Documentation
```

Phase 5 will add:

```text
Query
  ↓
Semantic Search
  ↓
Relevant Context
  ↓
LLM
  ↓
Answer + Citations
```

The Knowledge Base therefore provides the retrieval foundation for the future RAG system.
