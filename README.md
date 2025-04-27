---

# MindViz Server – Visual Task Management Server

MindViz is a modern task management system that helps organize hierarchical tasks, enabling users to manage their personal and professional activities with flexibility and context.

---

## 📚 Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Data Model](#data-model)
- [Getting Started](#getting-started)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [API Documentation](#api-documentation)
- [Frontend Integration](#frontend-integration)
- [Architecture](#architecture)
- [License](#license)

---

## 🌟 Overview

MindViz provides a robust backend for task management with support for:
- Hierarchical task organization (parent-child relationships)
- Task categorization and tagging
- Progress tracking and completion status
- Task sharing through groups
- Recurring task scheduling

The system is built on **.NET 8** using a **clean architecture** approach with separate layers for core domain, application logic, and infrastructure.

---

## ✨ Features

- **Hierarchical Task Management**: Create task hierarchies with parent-child relationships
- **Task Categories**: Organize tasks as Simple, Complex, or Category types
- **Progress Tracking**: Monitor task completion with progress percentages
- **Group Collaboration**: Share tasks with team members through groups
- **Admin Privileges**: Group administrators can manage members and tasks
- **Recurring Tasks**: Schedule tasks with daily, weekly, monthly frequencies
- **Deadlines**: Set and track task deadlines
- **Tagging**: Categorize tasks with custom tags
- **JWT Authentication**: Secure API access using JSON Web Tokens
- **Role-Based Authorization**: Manage user permissions based on roles

---

## 🗂 Data Model

### Core Entities
- **Task**: Represents actionable items or categories
- **User**: Account information and authentication details
- **Group**: Collection of users collaborating on shared tasks

### Join Entities
- **UserTask**: Maps many-to-many relationships between users and tasks
- **GroupTask**: Associates tasks to groups with metadata
- **GroupMember**: Tracks group membership and admin privileges

### Hierarchical Structure
- A root task serves as the top-level entry point (typically named after the user)
- **Category tasks** organize related activities
- **Simple tasks** represent concrete actionable items
- **Complex tasks** can have subtasks and track aggregate progress

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later
- SQL Server (local or remote)
- Environment variables for configuration

---

## 🛠 Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/MindVizBackend.git
   cd MindVizBackend
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Apply migrations to create the database:
   ```bash
   dotnet ef database update
   ```

5. Start the API server:
   ```bash
   dotnet run --project MindvizServer.API
   ```

---

## ⚙️ Configuration

Configure the following environment variables:
- `MindvizDatabase` — SQL Server connection string
- `JwtSecretKey` — Secret key for JWT token generation and validation

(You can place them in an `appsettings.Development.json` or as real environment variables.)

---

## 📄 API Documentation

When running in development mode, Swagger UI is available at:

```
/swagger
```

to explore and test the API endpoints.

### Key Endpoints
- `/Users` — User management (signup, login, profile)
- `/Tasks` — Task CRUD operations
- `/Groups` — Group creation, management, collaboration

---

## 🔗 Frontend Integration

The backend exposes standard **RESTful APIs** that can be consumed by any frontend framework.

Typical flow:
1. **Authentication**: Obtain JWT token via `/Users/login`
2. **Task Management**: Create, read, update, delete tasks
3. **Group Collaboration**: Manage group members and assign shared tasks

---

## 🏛 Architecture

MindViz follows a **Clean Architecture** approach:
- **Core Layer**: Domain models and business rules
- **Application Layer**: Business logic, service interfaces
- **Infrastructure Layer**: Database access, repository implementations
- **API Layer**: RESTful controllers, request/response mapping

### Project Structure
```
MindvizServer.Core/           // Domain Models and Interfaces
MindvizServer.Application/    // Business Services and Use Cases
MindvizServer.Infrastructure/ // EF Core Repositories, DB Context
MindvizServer.API/            // Web API Controllers
```

---

## 📝 License

```
LICENSE
```
© 2024 MindViz. All rights reserved.

---

