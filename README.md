# Cyber Bloom

Cyber Bloom is a desktop cybersecurity awareness application built with **C#**, **WPF**, and **Entity Framework Core** backed by **MySQL**. It helps users build cybersecurity knowledge through an interactive chatbot, task management, quizzes, and a personal activity log.

Developed as part of a Software Development practical assessment, the project demonstrates object-oriented programming, GUI development, database integration, and basic Natural Language Processing (NLP).

---

## Features

### Chatbot
An interactive cybersecurity assistant that personalizes the experience for each user. It remembers returning users, detects sentiment, handles basic spelling corrections, recognizes keywords across multiple topics, and generates contextual follow-up responses. Chat history is preserved across sessions.

### Task Assistant
A built-in task manager for tracking cybersecurity-related actions. Users can create tasks with optional reminder dates, mark them as complete, and delete them. All tasks are persisted via Entity Framework Core and MySQL.

### Quiz
A cybersecurity knowledge quiz featuring multiple-choice and true/false questions. Each answer receives immediate feedback, scores are tracked throughout the session, and completed attempts are saved to the database.

### Activity Log
A running log of user actions across the application — chatbot interactions, task changes, and quiz attempts — displayed as a history timeline.

---

## Cybersecurity Topics

The chatbot covers the following topics:

- Password Security
- Phishing
- Malware
- Ransomware
- Safe Browsing
- Online Scams
- Privacy
- Two-Factor Authentication

---

## Technologies

| Layer | Technology |
|---|---|
| Language | C# |
| UI Framework | Windows Presentation Foundation (WPF) |
| Runtime | .NET |
| ORM | Entity Framework Core |
| Database | MySQL 8.x |
| MySQL Provider | Pomelo.EntityFrameworkCore.MySql |

---

## Database

The application uses Entity Framework Core Migrations to manage the schema. The main tables are:

- `Users`
- `GuardianTasks`
- `ActivityLogEntries`
- `QuizAttempts`

---

## Getting Started

### Prerequisites

- Visual Studio 2022 with the .NET Desktop Development workload
- MySQL Community Server 8.x
- .NET SDK (version compatible with your project target)

### Install NuGet Packages

```powershell
Install-Package Microsoft.EntityFrameworkCore
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Pomelo.EntityFrameworkCore.MySql
```

### Configure the Connection String

Update your `appsettings.json` or `DbContext` configuration with your local MySQL credentials before running migrations.

### Apply Migrations

```powershell
Add-Migration InitialCreate
Update-Database
```

### Run

Open the solution in Visual Studio and press **F5** to build and launch the application.

---

## Project Highlights

This project demonstrates practical application of the following concepts:

- Object-Oriented Programming (encapsulation, inheritance, polymorphism)
- Event-driven programming with WPF
- CRUD operations via Entity Framework Core
- Basic NLP simulation (keyword recognition, sentiment detection, spelling correction)
- Exception handling and input validation
- Use of delegates and collections

---

## Planned Improvements

- Email and push notifications for task reminders
- User authentication and login
- Cloud database integration
- Password strength checker
- Dashboard with usage analytics
- Report export (PDF or CSV)
- Dark mode

---

## Author

**Raesibe Betty Lelaka**  
Student Number: ST10485428  
Diploma in Information and Communication Technology (Software Development)
