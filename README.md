# GCS Database API

This is the primary codebase for the GCS database and its API.

## To Set up WebSocket:

1. Pull code
2. Open folder in vscode
3. Open Docker Desktop and start redis servers
4. In vscode, open a terminal (if not already open, press Crtl + \` or Command + \`)
5. Check Dependencies  
   5a. Make sure you have .NET installed (https://dotnet.microsoft.com/en-us/download/dotnet/7.0)  
   5b. Make sure you have Node js installed (https://nodejs.org/en/download)  
   5c. Make sure you have NuGet packet manager extension installed in vscode  
   5d. In vscode, Crtl/Command + Shift + P -> NuGet: Open NuGet Gallery  
   5e. Search and install StackExchange.Redis
6. Open terminal with Crtl/Command + `
7. Run `dotnet run` to start connection to WebSocket
8. Open new terminal in vscode
9. Run `node client.js` to test connection  
   9a. This will start an endless loop of updating the number in the DB, push Crtl/Command + C to stop the script whenever
10. Open Redis Commander and log in to access DB on the browser
10a. The Login Information for Redis Commander to see database information is under `.env`
11. You should now see the string with the last number sent using the js script
