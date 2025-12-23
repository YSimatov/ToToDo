# ToToDo Project Setup

## Prerequisites
- Node.js & npm
- .NET 8 / 9 / 10 SDK
- PostgreSQL Server running on localhost

## Configuration
Before running the backend, please update the database connection string in `backend/appsettings.json`.

1. Open `backend/appsettings.json`
2. Update the `DefaultConnection` value:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=totodo;Username=postgres;Password=YOUR_PASSWORD"
   }
   ```
   Replace `YOUR_PASSWORD` with your PostgreSQL password. If your user is different, update `Username` as well.

## Running the Project

### Backend (.NET WebAPI)
1. Open a terminal in `backend` directory.
2. Run:
   ```bash
   dotnet run
   ```
   The server will start on `http://localhost:5288`.
   It will automatically create the database `totodo` and tables if credentials are correct.
   It will also seed two users:
   - Login: `admin`, Password: `admin` (Role: Admin)
   - Login: `user`, Password: `user` (Role: User)

### Frontend (Angular)
1. Open a terminal in the root directory.
2. Run:
   ```bash
   npm start
   ```
3. Open browser at `http://localhost:4200`.

## Features Implemented (Labs 1-6)
- **Lab 1**: User Stories (see `USER_STORIES.md`).
- **Lab 2**: Angular UI (Calendar, Task List, Theme Toggle).
- **Lab 3**: .NET Backend (REST API).
- **Lab 4**: PostgreSQL Database (SQL implementation for Reading/Adding tasks).
- **Lab 5**: Entity Framework (EF implementation for Updating/Deleting tasks).
- **Lab 6**: Authentication & Authorization (Basic Auth, Middleware, Role-based separation logic placeholder).

## Usage
1. Login with `admin` / `admin`.
2. Navigate the calendar.
3. Add tasks (uses SQL).
4. Update/Delete tasks (uses EF).
5. Switch Theme.
