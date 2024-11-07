# GCS Database API

This is the primary codebase for the GCS database and its API.

## To Set up WebSocket:

1. Pull code
2. Open folder in VSCode
3. Open Docker Desktop and start PostgreSQL server
4. In VSCode, open a terminal (if not already open, press Ctrl + \` or Command + \`)
5. Check Dependencies:
   5a. Make sure you have .NET 7.0.305 installed (https://dotnet.microsoft.com/en-us/download/dotnet/7.0)  
   5b. Make sure you have Node.js installed (https://nodejs.org/en/download)  
   5c. Make sure you have NuGet package manager and NuGet Gallery extension installed in VSCode  
   5d. In VSCode, press Ctrl/Command + Shift + P -> NuGet: Open NuGet Gallery  
   5e. Search and install Npgsql.EntityFrameworkCore.PostgreSQL
6. Open terminal with Ctrl/Command + `
7. Run `dotnet run` to start connection to the WebSocket
8. Open a new terminal in VSCode
9. Run `node client.js` to test the connection  
   - This will start an endless loop of updating the number in the DB. Press Ctrl/Command + C to stop the script whenever.
10. Open PostgreSQL client (e.g., pgAdmin) to access the database.

For Docker Setup:
1. Create `.env` file if it doesn't already exist, using the format provided in `.env.sample`.
2. Enter the following line into the CLI: `docker-compose up --detach`.
3. Run `dotnet run` in the console.
4. Open PostgreSQL client to log in and access the database.

## **PostgreSQL Configuration**

1. **Install PostgreSQL Extension in VSCode**:
   - Install the [PostgreSQL extension by Chris Kolkman](https://marketplace.visualstudio.com/items?itemName=chriskolkman.vscode-postgresql) to manage PostgreSQL directly from VSCode.

2. **Update `appsettings.json`**:
   - Modify the connection string under `DefaultConnection` to match your PostgreSQL setup. Example:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=mydatabase;Username=myuser;Password=mypassword"
     }
     ```

3. **Install Required NuGet Packages**:
   - Run the following commands to install the necessary packages for Entity Framework Core with PostgreSQL:
     ```bash
     dotnet add package Microsoft.EntityFrameworkCore
     dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
     ```

4. **Using Entity Framework Core ORM**:
   - **Npgsql**:
     - Npgsql is the **PostgreSQL provider** for Entity Framework Core. It allows EF Core to communicate with a PostgreSQL database and enables PostgreSQL-specific features like JSON handling, arrays, and enums.
     - Npgsql.EntityFrameworkCore.PostgreSQL allows EF Core to translate its commands into PostgreSQL-compatible SQL, making PostgreSQL available as a first-class citizen in your EF Core-based applications.

## Running Tests
`cd Tests | dotnet test`

## [Documentation comments](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/)

### Summary
```
/**
 * <summary>
 *    This class performs an important function.
 * </summary>
 */
public class MyClass { }
```

### Summary + Parameters + Return
```
/**
* <summary>
*  Enter description here for the second constructor.
*  ID string generated is "M:MyNamespace.MyClass.#ctor(System.Int32)".
* </summary>
* <param name="i">Describe parameter.</param>
* <param name="ptr">Describe parameter.</param>
* <returns>Describe return value.</returns
*/
public int SomeMethod(int i, void* ptr) {return 1;}
```