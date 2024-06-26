# OutOfOffice

## Setting up the Database

### Database application used: SQL Server 2022

To create and populate the database, follow these steps:
1. Open PowerShell in the `OutOfOfficeHRApp` folder.
2. Run the following command:
   ```shell
   dotnet ef database update
   ```
If you encounter an error, you may need to install Entity Framework Tools. To install, run this command in PowerShell:
  ```shell
  dotnet tool install --global dotnet-ef
```

## Running the application

To run the application
1. Open PowerShell in the `OutOfOfficeHRApp` folder.
2. Run the following command:
  ```shell
  dotnet run
```
3. Navigate to the address displayed in the console, typically `localhost:[4 number port]`
