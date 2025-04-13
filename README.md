# BookOrganiser
BookOrganiser is an ASP.NET MVC 9.0 web application for managing, searching, and organising books by categories. It integrates with Ollama using the Llama3.2 model for AI-assisted features.

## Prerequisites
### macOS/Linux
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).
- [PostgreSQL](https://www.postgresql.org/download/) (for production builds).
- SQLite (for development builds).
- [Ollama](https://ollama.com/) installed and running.

### Windows
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0).
- [Visual Studio 2022 or later](https://visualstudio.microsoft.com/) with the **ASP.NET and web development** workload.
- [PostgreSQL](https://www.postgresql.org/download/) (for production builds).
- SQLite (for development builds).
- [Ollama](https://ollama.com/) installed and running.

## Build Instructions
1. Clone the repository.
2. Restore dependencies.
    ```bash
    dotnet restore
    ```
3. Configure PostgreSQL connection **(for production builds only)**.

    Before running in production or applying migrations, set the `PostgreSqlConnection` string in `appsettings.Production.json`.
    ```json
    "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=YOUR_DATABASE_NAME;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
    }
    ```
    - Host: e.g. `localhost`
    - Port: Usually `5432`.
    - Database: Replace YOUR_DATABASE_NAME with your actual database name.
    - Username: Your PostgreSQL username.
    - Password: Your PostgreSQL password.
4. Apply migrations.
    - For development builds:
        - If you are on Windows and using Powershell:
        ```bash
        $env:ASPNETCORE_ENVIRONMENT="Development" # For Windows using Powershell
        dotnet ef database update
        ```
        - If you are on Windows and using Command Prompt:
        ```bash
        set ASPNETCORE_ENVIRONMENT=Development # For Windows using Command Prompt
        dotnet ef database update
        ```
        - If you are on macOS or Linux:
        ```bash
        export ASPNETCORE_ENVIRONMENT="Development" # For Linux/MacOS
        dotnet ef database update
        ```
    - For production builds:
        - If you are on Windows and using Powershell:
        ```bash
        $env:ASPNETCORE_ENVIRONMENT="Production" # For Windows using Powershell
        dotnet ef database update
        ```
        - If you are on Windows and using Command Prompt:
        ```bash
        set ASPNETCORE_ENVIRONMENT=Production # For Windows using Command Prompt
        dotnet ef database update
        ```
        - If you are on macOS or Linux:
        ```bash
        export ASPNETCORE_ENVIRONMENT="Production" # For Linux/MacOS
        dotnet ef database update
        ```
5. Pull the Llama3.2 model using Ollama.
    ```bash
    ollama pull llama3.2
    ```
6. Build the application.
    - Development build:
    ```bash
    dotnet build -c Debug
    ```
    - Production build:
    ```bash
    dotnet build -c Release
    ```
7. Run the application.
    - Development mode:
    ```bash
    dotnet run -c Debug
    ```
    - Production mode:
    ```bash
    dotnet run -c Release
    ```
8. Access the application.
    ```
    https://localhost:5001
    or
    http://localhost:5000
    ```

## Testing
Run tests with:
```bash
dotnet test
```

## Publish
To publish for deployment:
```bash
dotnet publish -c Release -o ./publish
```