# Internal Resource Booking System

A simple ASP.NET Core Razor Pages application for managing shared company resources 
(such as meeting rooms, vehicles, and equipment) and preventing double-bookings.

This project was developed as part of the City of Johannesburg Junior Developer 
technical assessment.

---

## Features

### Resource Management
- Create/Edit/Delete resources
- View all resources
- Resource validation (name required, positive capacity)

### Booking Management
- Create/Edit/Delete bookings
- Select from available resources
- Date & time inputs using `datetime-local`
- Server-side validation:
  - Required fields
  - EndTime must be after StartTime
- Booking conflict detection:
  - Prevents overlapping bookings for the same resource

### Additional Features
- Navigation bar for quick access
- Entity Framework Core with SQL Server LocalDB
- Migrations included
- Clean code with comments explaining key logic
- Database backup included in the submission

---

## Technologies Used

- ASP.NET Core 8 (Razor Pages)
- Entity Framework Core 8
- SQL Server LocalDB
- Bootstrap (default)
- C#

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB
- Visual Studio 2022

### Running the Project


dotnet restore
dotnet build
dotnet run

Or run using Visual Studio:

- Open the solution
- Click the green **HTTPS Run** button

---

## Database

Connection string (configured in `appsettings.json`):
Server=(localdb)\MSSQLLocalDB; Database=ResourceBookingDb; Trusted_Connection=True; TrustServerCertificate=True;
Migrations are included.

A `.bak` database backup file is included for the assessment submission.

---

## Screenshots

Screenshots are included in the `/Screenshots` folder in the final assessment ZIP 
(as required by the submission instructions).

---

## Developer

Nompumelelo Mbhense
Junior ASP.NET Core Developer