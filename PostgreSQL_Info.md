# This is some info related to PostgreSQL

## Setup:
1. **Install PostgreSQL Extension in VSCode**:

    - Install the [PostgreSQL extension by Chris Kolkman](https://marketplace.visualstudio.com/items?itemName=chriskolkman.vscode-postgresql) to manage PostgreSQL directly from VSCode.
    <br/><br/>

2. **Update `appsettings.json`**:

    - Modify the connection string under `DefaultConnection` to match your PostgreSQL setup. Example:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Host=localhost;Username=postgres;Password=pass;Database=gcs-database-api"
        }
        ```
    <br/>
    
3. **Install EF Core Packages**
- Run the following commands to install the necessary packages for Entity Framework Core with PostgreSQL:
        <br/><br/>
        ``
        dotnet add package Microsoft.EntityFrameworkCore
        ``
        <br/>
        ``
        dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
        ``
        <br/><br/>
1. **Using Entity Framework Core ORM**:
    - **Npgsql**:
        - Npgsql is the **PostgreSQL provider** for Entity Framework Core. It allows EF Core to communicate with a PostgreSQL database and enables PostgreSQL-specific features like JSON handling, arrays, and enums.
        - Npgsql.EntityFrameworkCore.PostgreSQL allows EF Core to translate its commands into PostgreSQL-compatible SQL, making PostgreSQL available as a first-class citizen in your EF Core-based applications.
    - *Note: I still dont understand 100% about using EF Core and this is just a definition i looked up


## Other Things
- when i tried to run a post MissionInfo route, I had a whole bunch of errors
  - turns out that a lot of the errors were that EF Core wants each entity/model to have a primary key
  - the primary key it looks for is ``public int Id {get; set;}`` or ``public int nameId {get; set;}`` (basically it's looking for 'Id')
<br/><br/>

- another error i came across was that 'collection navigations cannot be arrays' so instead of using ``public VehicleData[] vehicleKeys {get; set;}`` i had to use ``public List<VehicleData> vehicleKeys {get; set;}``
<br/><br/>

- also i forgot what error caused this, buf EF Core needs an empty constructor to setup the contexts. this applied to like all the models
<br/><br/>

- at the top of the controller definition i had to have something like 
```C#
private readonly IDbContextFactory<AppDbContext> _context;
public NameController(IDbContextFactory<AppDbContext> context){
    _context = context;
}
```
then at the top of each route, I needed to have
```C#
using var context = _context.CreateDbContext();
```
*NOTE: I'm not too sure if this was done correctly or if there is another way I was supposed to use the context. I think for this I had to use something called AddContextFactory() or something in Program.cs which manually creates DbContext instances where dependency injection isn't available (idk what that means)
<br/><br/>

- to add tables to the database, i ran these commands that setup the tables according to your context i believe. This is also ran when the schema updates. 
    - ``dotnet ef migrations add InitialCreate``
    - ``dotnet ef database update``
