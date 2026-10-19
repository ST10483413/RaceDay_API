# RaceDay API

[![Validate Repository Structure](https://github.com/ST10483413/RaceDay_API/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/ST10483413/RaceDay_API/actions/workflows/dotnet-desktop.yml)


YOUTUBE LINK (https://youtu.be/UzqlmXQ_64Y)
##  Project Overview

The RaceDay API is the backend system for a comprehensive event management application. It streamlines the process of organizing and participating in running, cycling, and walking events. The API is built using ASP.NET Core Web API, Entity Framework Core (Code-First) and SQL Server and it implements role-based access control via JWT authentication.

## User Roles

The system supports two distinct roles with specific permissions:

### 1. Organiser
- Create, update, and delete events.
- Define categories for events (e.g., age groups, distances).
- View all enrollments for their events.
- Capture and manage finishing times and positions (results) for participants.

### 2. Participant
- View available events and categories.
- Enroll in specific event categories.
- View their own enrollment history.
- View their own race results.

---

##  Technology Stack

- Framework: .NET 8.0 (ASP.NET Core Web API)
- Database:SQL Server (LocalDB)
- ORM: Entity Framework Core (Code-First)
- Authentication:JWT (JSON Web Tokens) & BCrypt for password hashing
- Documentation:Swagger / OpenAPI
- Testing: xUnit & EF Core InMemory
- CI/CD: GitHub Actions
[![Validate Repository Structure](https://github.com/ST10483413/RaceDay_API/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/ST10483413/RaceDay_API/actions/workflows/dotnet-desktop.yml)
---

##  Setup & Installation

### Prerequisites
- [Visual Studio 2022] with ASP.NET and web development workload
- [SQL Server Management Studio (SSMS) 22]
- SQL Server Express LocalDB

### Step 1: Clone the Repository
```bash
git clone https://github.com/ST10483413/RaceDay_API.git
```

### Step 2: Configure the Database
1. Ensure your LocalDB instance is running. You can create it via Command Prompt:
   ```cmd
   sqllocaldb create part_2
   ```
2. Open `RaceDay.API/appsettings.json` and verify the connection string matches your setup:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\part_2;Database=part_2;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

### Step 3: Apply Database Migrations
1. Open the solution in **Visual Studio 2022**.
2. Go to **Tools** > **NuGet Package Manager** > **Package Manager Console**.
3. Run the following command:
   powershell
   Update-Database
   

### Step 4: Run the API
1. Set `RaceDay_API` as the Startup Project.
2. Press **F5** or click the green **Start** button.
3. Your browser will automatically open to the Swagger UI at `https://localhost:7115/swagger`.

---

## API Endpoints Summary

For a full list of endpoints and to test them directly, please use the Swagger UI when running the application.

| Method | Endpoint | Description | Role Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register a new user (Organiser or Participant) | None |
| POST | `/api/auth/login` | Authenticate and receive a JWT token | None |
| GET/PUT| `/api/users/profile` | View or update own profile | Any |
| GET | `/api/events` | View all events | Any |
| POST | `/api/events` | Create a new event | Organiser |
| PUT | `/api/events/{id}` | Update an event | Organiser |
| DELETE | `/api/events/{id}` | Delete an event | Organiser |
| POST | `/api/categories` | Create a new category for an event | Organiser |
| POST | `/api/enrollments` | Enroll in a category | Participant |
| GET | `/api/enrollments/me`| View own enrollments | Participant |
| GET | `/api/enrollments/event/{id}` | View all enrollments for an event | Organiser |
| POST | `/api/enrollments/{id}/results`| Add a result to an enrollment | Organiser |
| GET | `/api/results/me` | View own results | Participant |

---

##  Part 1 to Part 2 Design Iterations

To align with the **Part 2 Functional Requirements**, the following changes were made to the original Part 1 database design:

| Field Added | Entity | Reason |
|-------------|--------|--------|
| `Distance` (decimal) | Event | Part 2 requires distance captured per event |
| `EventType` (string) | Event | Part 2 requires event type: Run, Walk, or Cycle |


---

## 🧪 Unit Testing

The project includes automated unit tests using **xUnit** and **Entity Framework Core InMemory**. Tests cover authentication, role-based access rejection, and core functionalities.

To run the tests locally:
1. Open **Test** > **Test Explorer** in Visual Studio, or
2. Run the following command in the Package Manager Console:
   ```powershell
   dotnet test
   ```

---

##  CI/CD Pipeline

This repository uses **GitHub Actions** for continuous integration.
- On every push or pull request to the `main` branch, the workflow triggers.
- It restores dependencies, builds the solution, runs all unit tests, and validates that required planning documents exist in the `/docs` folder.

### Green Build Screenshot
[![Validate Repository Structure](https://github.com/ST10483413/RaceDay_API/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/ST10483413/RaceDay_API/actions/workflows/dotnet-desktop.yml)

![CI Build Success](docs/screenshots/ci-green-build.png)

---

##  Video Walkthrough

A complete video demonstration of the API functionality, role-based access, Swagger testing, and CI/CD pipeline can be found below:

**Watch the Part 2 API Walkthrough on YouTube
( https://youtu.be/UzqlmXQ_64Y)**

---

##  Author

- **Student Name:** Thato Maleka
- **Student Number:** ST10483413
- **Module:** PROG6212 Programming 2B
```

 
