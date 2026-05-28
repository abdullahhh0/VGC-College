# VgcCollege - Multi-Branch Student & Course Management System

## How to Run Locally

1. Clone the repository
2. Open in Rider or Visual Studio
3. Navigate to `VgcCollege.Web` folder
4. Run the app - database will be created and seeded automatically

## Seeded Demo Accounts

| Role    | Email             | Password          |
|---------|-------------------|-------------------|
| Admin   | MuhammadAns@vgc.ie| MuhammadAns@75834 |
| Faculty | SyedHaseeb@vgc.ie | SyedHaseeb@72180  |
| Student | student1@vgc.ie   | Student@12345     |
| Student | student2@vgc.ie   | Student@12345     |

## How to Run Tests
```cmd
cd VgcCollege.Tests
dotnet test
```

## Design Decisions

- SQLite used for simplicity and portability
- ASP.NET Core Identity for authentication
- Role-based authorization: Admin, Faculty, Student
- Exam results hidden from students until ResultsReleased = true
- Seed data runs automatically on startup