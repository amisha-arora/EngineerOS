# EngineerOS User Flow

**Project:** EngineerOS  
**Version:** 1.0  
**Author:** Amisha Arora  
**Status:** Draft  

---

## 1. Overview

This document describes the main user journey through EngineerOS Version 1.

EngineerOS helps software engineers upload and analyze an unfamiliar repository, explore its architecture, search repository knowledge, interact with an AI Mentor, and follow a guided learning roadmap.

---

## 2. Primary User Flow

The main flow is:

Login  
↓  
Dashboard  
↓  
Repository Upload  
↓  
Repository Analysis  
↓  
Repository Indexed  
↓  

From the indexed repository, the user can access:

- Architecture Explorer
- AI Mentor
- Repository Details
- Semantic Search
- Learning Roadmap

---

## 3. Login Flow

The user opens EngineerOS and authenticates using:

- Email and password
- Google Sign-In

If authentication is successful, the user is redirected to the Dashboard.

If authentication fails, an appropriate error message is shown.

---

## 4. Dashboard Flow

After login, the Dashboard provides an overview of the user's EngineerOS activity.

The dashboard may display:

- Uploaded repositories
- Recent analyses
- Learning progress
- Recent AI Mentor questions
- Repository statistics

The user can navigate to:

- Repository Upload
- Existing repositories
- AI Mentor
- Semantic Search
- Settings

---

## 5. Repository Upload Flow

The user chooses to upload a repository.

The user provides:

- Repository name
- Optional description
- Repository ZIP file

EngineerOS validates the repository before analysis begins.

Supported repository formats for Version 1 include:

- `.zip`
- C#/.NET repositories containing `.sln` or `.csproj` files

After a successful upload, repository analysis starts.

---

## 6. Repository Analysis Flow

During repository analysis, EngineerOS performs operations such as:

- Reading the solution structure
- Detecting projects and files
- Extracting classes and interfaces
- Extracting methods
- Detecting controllers and services
- Identifying dependencies
- Creating code chunks
- Generating embeddings
- Indexing repository knowledge

The user sees an analysis progress state while processing is taking place.

When analysis is complete, the repository status becomes:

`Indexed`

---

## 7. Repository Dashboard

After successful analysis, the user can access information about the repository.

The repository dashboard may display:

- Repository statistics
- Analysis status
- Number of projects
- Number of classes
- Number of methods
- Number of controllers
- Number of services
- Last analyzed time

From here, the user can access the main repository features.

---

## 8. Architecture Explorer Flow

The user opens the Architecture Explorer to understand the structure of the repository.

The Architecture Explorer may show:

- Projects
- Classes
- Interfaces
- Controllers
- Services
- Dependency relationships

The user can select a component to view additional information such as:

- Methods
- Dependencies
- Description
- File location

---

## 9. AI Mentor Flow

The user opens the AI Mentor for an indexed repository.

The user can ask questions such as:

- Where is authentication implemented?
- How does dependency injection work?
- What does this service do?
- Which module should I understand first?

EngineerOS retrieves relevant repository context and generates a repository-grounded answer.

The conversation is stored as a chat session.

---

## 10. Semantic Search Flow

The user can search the repository using natural-language queries.

Example:

`Where is inventory updated?`

EngineerOS performs semantic search over repository knowledge and returns relevant:

- Files
- Classes
- Methods
- Code snippets
- Relationships

Semantic search focuses on meaning rather than only exact keyword matching.

---

## 11. Learning Roadmap Flow

The user can generate a learning roadmap for the analyzed repository.

The roadmap may suggest an ordered learning sequence such as:

1. Authentication
2. Dependency Injection
3. User Module
4. Order Module
5. Payment Module

Each roadmap step may include:

- Topic name
- Description
- Difficulty
- Estimated learning time
- Progress status
- AI explanation

Users can mark roadmap steps as completed.

---

## 12. Repository Details Flow

The Repository Details screen provides metadata and analysis information for a selected repository.

It may display:

- Repository name
- Primary language
- Projects
- Classes
- Interfaces
- Controllers
- Services
- Methods
- Last analyzed time
- Analysis status

---

## 13. Settings Flow

Users can access application settings from the main navigation.

Settings may include:

- Account information
- Theme preferences
- Security settings
- Logout

---

## 14. User Flow Diagram

The following diagram represents the main EngineerOS user journey.

![EngineerOS User Flow](../diagrams/EngineerOS-User-Flow.png)

---

## 15. Version 1 Scope

The Version 1 user flow focuses on:

- Authentication
- Repository upload
- Repository analysis
- Architecture exploration
- AI Mentor
- Semantic search
- Repository details
- Learning roadmap
- Dashboard
- Settings

Features such as team collaboration, IDE plugins, live GitHub synchronization, and multi-language repository analysis are outside the current Version 1 user flow.
![EngineerOS User Flow](../diagrams/EngineerOS-User-Flow.png)