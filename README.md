# Star Wars Starships

This repository is a locally running example of an .NET 8 ASP.NET MVC Project. The project is in the form of a starship dealership called Orbital Outfitter, enjoy!

## Dependencies

To successfully run the application, ensure you have the following dependencies installed:

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed on your machine.
- [SQLite](https://www.sqlite.org/download.html) for local development.
- [Entity Framework Core CLI tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) for database migrations.

## Setup

To build and run the application, follow these steps:

1. **Restore Dependencies**

   Run the following command to restore the project dependencies:

   ```bash
   dotnet restore
   ```

2. **Apply Database Migrations**

   Using Entity Framework Core, apply any pending migrations to the database:

   ```bash
   dotnet ef database update
   ```

3. **Build the Project**

   Build the project to ensure there are no compilation errors:

   ```bash
   dotnet build
   ```

4. **Run the Project**

   Start the application:

   ```bash
   dotnet run
   ```

   By default, the application will run on http://localhost:5071 (or another port if configured differently) and be seeded with initial starship data.

## Access the Application

To access the application, open your browser and go to http://localhost:5071 (or whichever port was configured for the project to run at). The application should now be running with a UI to interact with the starship backend.
