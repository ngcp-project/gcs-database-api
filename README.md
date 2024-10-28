# GCS Database API

This is the primary codebase for the GCS database and its API.

## To Set up WebSocket:

1. Pull code
2. Open folder in vscode
3. Open Docker Desktop and start redis servers
4. In vscode, open a terminal (if not already open, press Crtl + \` or Command + \`)
5. Check Dependencies  
   5a. Make sure you have .NET 7.0.305 installed (https://dotnet.microsoft.com/en-us/download/dotnet/7.0)  
   5b. Make sure you have Node js installed (https://nodejs.org/en/download)  
   5c. Make sure you have NuGet packet manager and NuGet Gallery extension installed in vscode  
   5d. In vscode, Crtl/Command + Shift + P -> NuGet: Open NuGet Gallery  
   5e. Search and install StackExchange.Redis
6. Open terminal with Crtl/Command + `
7. Run `dotnet run` to start connection to WebSocket
8. Open new terminal in vscode
9. Run `node client.js` to test connection  
   - This will start an endless loop of updating the number in the DB, push Crtl/Command + C to stop the script whenever
10. Open Redis Commander and log in to access DB on the browser
      -  The Login Information for Redis Commander to see database information is under `.env`
11. You should now see the string with the last number sent using the js script

For Docker Setup:
1. Create .env file if it doesn't already exist using format provided in  .env.sample
2. Enter the following line into the CLI: docker-compose up --detach
3. `dotnet run` in console
4. Open Redis Commander and log in

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