# Internal Resource Booking System

A simple ASP.NET Core Razor Pages application for managing shared company resources
(such as meeting rooms, vehicles, and equipment) and preventing double-bookings.

---

## Features

### Resource Management
- Create/Edit/Delete resources
- View all resources in a clean data table
- Detailed view with all information + upcoming bookings
- Validation included (required fields, positive capacity)

### Booking Management
- Create/Edit/Delete bookings
- Choose a resource from a dropdown
- Bookings include: StartTime, EndTime, BookedBy, Purpose
- Validation:
  - Required fields
  - EndTime must be after StartTime
- Robust conflict detection
  - Prevents overlapping bookings for the same resource
	- Shows a user-friendly error message

### Additional Features
- Search & filter on Resources (name) and Bookings (name + date)
- Navigation bar for quick access
- Polished Bootstrap UI
- Logging + error handling added to all page models
- Seeding included to pre-populate initial test resources

---

## Technologies Used

- ASP.NET Core 8 (Razor Pages)
- Entity Framework Core 8
- SQL Server LocalDB
- Bootstrap 5
- C#

---

## Getting Started

### Prerequisites
- Ensure you have the following installed:
- .NET 8 SDK
- SQL Server LocalDB
- Visual Studio 2022

### Running the Project


dotnet restore
dotnet build
dotnet run

Or run using Visual Studio:

1. Open the solution (ResourceBookingSystem.sln)
2. Select ResourceBookingSystem as startup project
3. Click the green Run (HTTPS) button

---

## Database

The connection string is configured in appsettings.json:

Connection string (configured in `appsettings.json`):
Server=(localdb)\MSSQLLocalDB;
Database=ResourceBookingDb;
Trusted_Connection=True;
TrustServerCertificate=True;

- Migrations are included (/Migrations folder)
- A .bak backup file is included in the final submission ZIP


---

## Screenshots

- `/Screenshots` – screenshots of the running system and database
- `/Database/ResourceBookingDb.bak` – full database backup

---

## Developer

Nompumelelo Mbhense
Junior ASP.NET Core Developer