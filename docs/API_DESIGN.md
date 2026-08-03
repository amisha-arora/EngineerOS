# EngineerOS API Specification

**Project:** EngineerOS  
**Version:** 1.0  
**Author:** Amisha Arora  
**Status:** Draft  

---

## 1. API Overview

EngineerOS uses REST APIs for communication between the React frontend and the ASP.NET Core backend.

The APIs allow the frontend to perform operations such as:

- User authentication
- Repository upload and management
- Repository analysis
- Architecture exploration
- Semantic search
- AI Mentor conversations
- Learning roadmap generation
- Dashboard data retrieval

The base API path for Version 1 is:

`/api/v1`

For example:

`/api/v1/auth/login`

---

## 2. Authentication APIs

Authentication APIs are responsible for user registration, login, Google authentication, token management, and logout.

### 2.1 Register User

**Method:** POST  
**Endpoint:** `/api/v1/auth/register`  
**Authentication Required:** No

**Purpose:**  
Creates a new EngineerOS user account.

**Request:**

```json
{
  "firstName": "Amisha",
  "lastName": "Arora",
  "email": "amisha@example.com",
  "password": "StrongPassword123!"
}
```

**Possible Responses:**

- `201 Created` — User registered successfully
- `400 Bad Request` — Invalid input
- `409 Conflict` — Email already registered

---

### 2.2 Login

**Method:** POST  
**Endpoint:** `/api/v1/auth/login`  
**Authentication Required:** No

**Purpose:**  
Authenticates an existing user using email and password.

**Request:**

```json
{
  "email": "amisha@example.com",
  "password": "StrongPassword123!"
}
```

**Possible Responses:**

- `200 OK` — Login successful
- `400 Bad Request` — Invalid input
- `401 Unauthorized` — Invalid email or password

---

### 2.3 Google Login

**Method:** POST  
**Endpoint:** `/api/v1/auth/google`  
**Authentication Required:** No

**Purpose:**  
Authenticates a user using Google Sign-In.

**Possible Responses:**

- `200 OK` — Google authentication successful
- `400 Bad Request` — Invalid authentication request
- `401 Unauthorized` — Google authentication failed

---

### 2.4 Refresh Access Token

**Method:** POST  
**Endpoint:** `/api/v1/auth/refresh`  
**Authentication Required:** No

**Purpose:**  
Generates a new access token when the existing access token expires and the refresh token is still valid.

**Possible Responses:**

- `200 OK` — New access token generated
- `401 Unauthorized` — Refresh token is invalid or expired

---

### 2.5 Logout

**Method:** POST  
**Endpoint:** `/api/v1/auth/logout`  
**Authentication Required:** Yes

**Purpose:**  
Logs the user out and invalidates the associated refresh token.

**Possible Responses:**

- `204 No Content` — Logout successful
- `401 Unauthorized` — User is not authenticated

---

## 3. User API

### 3.1 Get Current User

**Method:** GET  
**Endpoint:** `/api/v1/users/me`  
**Authentication Required:** Yes

**Purpose:**  
Returns information about the currently authenticated user.

**Example Response:**

```json
{
  "id": "user-id",
  "firstName": "Amisha",
  "lastName": "Arora",
  "email": "amisha@example.com",
  "role": "User"
}
```

**Possible Responses:**

- `200 OK` — User information returned successfully
- `401 Unauthorized` — User is not authenticated

---

## 4. Repository APIs

Repository APIs allow users to upload, retrieve, and delete repositories.

### 4.1 Upload Repository

**Method:** POST  
**Endpoint:** `/api/v1/repositories`  
**Authentication Required:** Yes

**Purpose:**  
Uploads a repository to EngineerOS so that it can later be analyzed.

**Request Data:**

- Repository name
- Description
- Repository ZIP file

**Possible Responses:**

- `201 Created` — Repository uploaded successfully
- `400 Bad Request` — Invalid repository
- `401 Unauthorized` — User is not authenticated
- `413 Payload Too Large` — Repository exceeds the allowed size
- `415 Unsupported Media Type` — Unsupported file format

---

### 4.2 Get All Repositories

**Method:** GET  
**Endpoint:** `/api/v1/repositories`  
**Authentication Required:** Yes

**Purpose:**  
Returns all repositories belonging to the authenticated user.

**Possible Responses:**

- `200 OK` — Repositories returned successfully
- `401 Unauthorized` — User is not authenticated

---

### 4.3 Get Repository

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}`  
**Authentication Required:** Yes

**Purpose:**  
Returns information about a specific repository.

`{repositoryId}` represents the unique ID of the repository.

**Possible Responses:**

- `200 OK` — Repository returned successfully
- `403 Forbidden` — User cannot access this repository
- `404 Not Found` — Repository does not exist

---

### 4.4 Delete Repository

**Method:** DELETE  
**Endpoint:** `/api/v1/repositories/{repositoryId}`  
**Authentication Required:** Yes

**Purpose:**  
Deletes a repository belonging to the authenticated user.

**Possible Responses:**

- `204 No Content` — Repository deleted successfully
- `403 Forbidden` — User cannot delete this repository
- `404 Not Found` — Repository does not exist

---

## 5. Repository Analysis APIs

These APIs are responsible for starting repository analysis and checking its progress.

### 5.1 Start Repository Analysis

**Method:** POST  
**Endpoint:** `/api/v1/repositories/{repositoryId}/analyses`  
**Authentication Required:** Yes

**Purpose:**  
Starts analysis of an uploaded repository.

The analysis may include:

- Parsing the .NET solution
- Detecting projects and files
- Extracting classes and interfaces
- Extracting methods
- Identifying controllers and services
- Detecting dependencies
- Creating code chunks
- Generating embeddings
- Indexing repository knowledge

**Possible Responses:**

- `202 Accepted` — Repository analysis started
- `403 Forbidden` — User cannot access this repository
- `404 Not Found` — Repository does not exist
- `422 Unprocessable Content` — Repository cannot be analyzed

---

### 5.2 Get Analysis Status

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}/analyses/latest`  
**Authentication Required:** Yes

**Purpose:**  
Returns the current status and progress of repository analysis.

**Example Response:**

```json
{
  "status": "Analyzing",
  "currentStage": "GeneratingEmbeddings",
  "progressPercentage": 70
}
```

Possible analysis statuses include:

- Queued
- Analyzing
- Indexed
- Failed

**Possible Responses:**

- `200 OK` — Analysis status returned successfully
- `404 Not Found` — Repository or analysis not found

---

## 6. Architecture APIs

These APIs provide the information required by the Architecture Explorer.

### 6.1 Get Repository Summary

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}/summary`  
**Authentication Required:** Yes

**Purpose:**  
Returns statistics about the analyzed repository.

The response may contain:

- Number of projects
- Number of files
- Number of classes
- Number of interfaces
- Number of methods
- Number of controllers
- Number of services

---

### 6.2 Get Architecture

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}/architecture`  
**Authentication Required:** Yes

**Purpose:**  
Returns the analyzed structure of the repository for the Architecture Explorer.

The response may contain:

- Projects
- Classes
- Interfaces
- Controllers
- Services
- Relationships

**Possible Responses:**

- `200 OK` — Architecture returned successfully
- `404 Not Found` — Architecture data not found

---

### 6.3 Get Dependency Graph

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}/dependencies`  
**Authentication Required:** Yes

**Purpose:**  
Returns dependencies between components of the repository.

Dependency types may include:

- Calls
- Implements
- Inherits
- Injects
- References
- Uses

**Possible Responses:**

- `200 OK` — Dependency information returned successfully
- `404 Not Found` — Repository not found

---

### 6.4 Get Code Symbol Details

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}/symbols/{symbolId}`  
**Authentication Required:** Yes

**Purpose:**  
Returns detailed information about a selected code symbol.

A code symbol may represent a:

- Class
- Interface
- Method
- Property
- Controller
- Service
- Enum

**Possible Responses:**

- `200 OK` — Symbol details returned successfully
- `404 Not Found` — Symbol not found

---

## 7. Semantic Search API

### 7.1 Search Repository

**Method:** POST  
**Endpoint:** `/api/v1/repositories/{repositoryId}/search`  
**Authentication Required:** Yes

**Purpose:**  
Allows users to search repository content by meaning instead of relying only on exact keywords or filenames.

**Example Request:**

```json
{
  "query": "Where is authentication implemented?",
  "limit": 10
}
```

The backend will use repository embeddings and pgvector to find relevant code.

**Possible Responses:**

- `200 OK` — Relevant results returned successfully
- `400 Bad Request` — Search query is invalid
- `404 Not Found` — Repository not found

---

## 8. AI Mentor APIs

AI Mentor APIs allow users to have conversations about an analyzed repository.

### 8.1 Create Chat Session

**Method:** POST  
**Endpoint:** `/api/v1/repositories/{repositoryId}/chat-sessions`  
**Authentication Required:** Yes

**Purpose:**  
Creates a new AI Mentor conversation for a repository.

**Possible Responses:**

- `201 Created` — Chat session created successfully
- `404 Not Found` — Repository not found

---

### 8.2 Get Chat Sessions

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}/chat-sessions`  
**Authentication Required:** Yes

**Purpose:**  
Returns previous AI Mentor conversations for a repository.

**Possible Responses:**

- `200 OK` — Chat sessions returned successfully
- `404 Not Found` — Repository not found

---

### 8.3 Get Chat Messages

**Method:** GET  
**Endpoint:** `/api/v1/chat-sessions/{sessionId}/messages`  
**Authentication Required:** Yes

**Purpose:**  
Returns previous messages from an AI Mentor conversation.

**Possible Responses:**

- `200 OK` — Messages returned successfully
- `404 Not Found` — Chat session not found

---

### 8.4 Send Message to AI Mentor

**Method:** POST  
**Endpoint:** `/api/v1/chat-sessions/{sessionId}/messages`  
**Authentication Required:** Yes

**Purpose:**  
Sends a repository-related question to the AI Mentor.

**Example Request:**

```json
{
  "content": "How is dependency injection configured?"
}
```

EngineerOS will retrieve relevant repository context and use the AI service to generate an answer.

**Possible Responses:**

- `200 OK` — AI response generated successfully
- `400 Bad Request` — Invalid question
- `404 Not Found` — Chat session not found
- `500 Internal Server Error` — AI processing failed

---

## 9. Learning Roadmap APIs

These APIs support the personalized repository learning roadmap.

### 9.1 Generate Learning Roadmap

**Method:** POST  
**Endpoint:** `/api/v1/repositories/{repositoryId}/roadmaps`  
**Authentication Required:** Yes

**Purpose:**  
Generates a recommended learning roadmap based on the analyzed repository.

The roadmap may recommend topics such as:

1. Authentication
2. Dependency Injection
3. User Module
4. Order Module
5. Payment Module

**Possible Responses:**

- `201 Created` — Learning roadmap generated
- `404 Not Found` — Repository not found
- `422 Unprocessable Content` — Repository has not been successfully analyzed

---

### 9.2 Get Learning Roadmap

**Method:** GET  
**Endpoint:** `/api/v1/repositories/{repositoryId}/roadmaps/latest`  
**Authentication Required:** Yes

**Purpose:**  
Returns the latest learning roadmap for the user and repository.

**Possible Responses:**

- `200 OK` — Learning roadmap returned
- `404 Not Found` — Learning roadmap not found

---

### 9.3 Update Roadmap Step

**Method:** PATCH  
**Endpoint:** `/api/v1/roadmap-steps/{stepId}`  
**Authentication Required:** Yes

**Purpose:**  
Updates the completion status of a learning roadmap step.

**Example Request:**

```json
{
  "isCompleted": true
}
```

**Possible Responses:**

- `200 OK` — Roadmap step updated
- `400 Bad Request` — Invalid update
- `404 Not Found` — Roadmap step not found

---

## 10. Dashboard API

### 10.1 Get Dashboard

**Method:** GET  
**Endpoint:** `/api/v1/dashboard`  
**Authentication Required:** Yes

**Purpose:**  
Returns summary information required by the EngineerOS dashboard.

The response may contain:

- Repository count
- Recent repositories
- Recent repository analyses
- Learning progress
- Recent AI Mentor questions
- Repository statistics

**Possible Responses:**

- `200 OK` — Dashboard information returned successfully
- `401 Unauthorized` — User is not authenticated

---

## 11. HTTP Status Codes

EngineerOS will use standard HTTP status codes.

| Status Code | Meaning |
|---|---|
| 200 OK | Request completed successfully |
| 201 Created | New resource created successfully |
| 202 Accepted | Request accepted for background processing |
| 204 No Content | Request completed successfully with no response body |
| 400 Bad Request | Request contains invalid data |
| 401 Unauthorized | Authentication is required or failed |
| 403 Forbidden | User does not have permission |
| 404 Not Found | Requested resource does not exist |
| 409 Conflict | Request conflicts with existing data |
| 413 Payload Too Large | Uploaded repository is too large |
| 415 Unsupported Media Type | Uploaded file type is unsupported |
| 422 Unprocessable Content | Request is valid but cannot be processed |
| 500 Internal Server Error | Unexpected server error |

---

## 12. Security Requirements

EngineerOS APIs will follow these security requirements:

- Passwords will never be stored as plain text.
- Passwords will be securely hashed.
- Protected endpoints will require authentication.
- Users will only be able to access repositories they are authorized to access.
- Repository uploads will be validated.
- File size limits will be enforced.
- Unsupported repository file types will be rejected.
- API inputs will be validated.
- Authentication tokens will be handled securely.
- Refresh tokens will be securely stored and revocable.
- Production API communication will use HTTPS.
- Uploaded source code will not be exposed to unauthorized users.

---

## 13. Notes

This document describes the planned Version 1 API for EngineerOS.

The API specification may evolve during implementation as technical requirements become clearer.`