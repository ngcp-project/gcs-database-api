install vscode extension PostgreSQL by Chris Kolkman


appsettings.json will need to change DefaultConnection

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL

using EntityFrameworkCore ORM


# Npgsql:
1. Database Provider for EF Core: Entity Framework Core relies on different "database providers" to work with specific databases. Npgsql.EntityFrameworkCore.PostgreSQL is the official EF Core provider for PostgreSQL, so it’s what enables EF Core to translate its commands into PostgreSQL-compatible SQL.
2. Enables EF Core’s PostgreSQL-Specific Features: By using the Npgsql provider for EF Core, you gain access to PostgreSQL-specific features like JSON, arrays, enums, and more—all while working through EF Core’s abstractions.