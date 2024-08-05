
![Badge](https://gist.githubusercontent.com/ismailbennani/41298e9ae50d221bcece16e11f668613/raw/a8af7837b4d07291930dd1c4f414f3ee4bd66e7d/badge.svg)

# SudokuVS

Play SUDOKU against your friends.

[https://sudokuvs.azurewebsites.net](https://sudokuvs.azurewebsites.net)

# Build and run locally

1. Clone the repository
```
git clone  https://github.com/SudokuVS/SudokuVS 
```

2. Build the app
```
dotnet restore
dotnet build --no-restore
```

3. Configure the app

either in the `appsettings.json` 
```json
"ConnectionStrings": {
    "GameDbContext": "", // required, SQL server connection string expected
    "AppDbContext": "" // required, SQL server connection string expected
},
"Authentication": {
   "Google": {
      "ClientId": "", // optional, enabled google auth
      "ClientSecret": "" // optional, enabled google auth
   }, 
   "Microsoft": {
      "ClientId": "optional", // optional, enabled microsoft auth
      "ClientSecret": "optional" // optional, enabled microsoft auth
   }
}, 
```

or in the dotnet user-secrets
```
dotnet user-secrets --project SudokuVS.Server set "ConnectionStrings:GameDbContext" "..."
dotnet user-secrets --project SudokuVS.Server set "ConnectionStrings:AppDbContext" "..."
```
or in any other source supported by the default ASP.NET Core configuration builder

4. Run the app: in ``
```
cd bin/Debug/net8.0
./SudokuVS.Server.exe 
```

5. The app should be served at [https://localhost:8001](https://localhost:8001)

