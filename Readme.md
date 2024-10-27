# Search API

This project is an ASP.NET Core Web API for managing search histories with JWT authentication and IP rate limiting.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Run Migration Command](#Run Migration Command)
- [Inserting Default Data](#inserting-default-data)
- [Running the Project](#running-the-project)
- [Modifying the Connection String](#modifying-the-connection-string)
- [Run Migration Command](#Run Migration Command)
- [Inserting Default Data](#Inserting Default Data)
- [Running the Application](#Running the Application)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Postman](https://www.postman.com/downloads/) (optional, for API testing)

## Setup Instructions

### Clone the repository:
```bash
git clone https://github.com/yogendra-ykv/searchapi
cd searchapi
```
### Restore Project Dependencies

To ensure everything is up-to-date, restore all project dependencies by running the following command:

```bash
dotnet restore
```

###. Modifying the Connection String
Update the connection string in appsettings.json:

- Edit appsettings.json "DefaultConnection"

### Run Migration Command
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```


### Inserting Default Data
If you want to manually create the table and insert some initial data, you can use the following SQL commands. Open SQL Server Management Studio (SSMS) and execute the following:

USE SearchDB; 

GO 

CREATE TABLE SearchHistories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Query NVARCHAR(255) NOT NULL,
    Result NVARCHAR(255) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UserId NVARCHAR(50) NOT NULL
); 

INSERT INTO SearchHistories (Query, Result, CreatedDate, UserId)
VALUES 
    ('How to use C#?', 'Search results for C# tutorial', GETDATE(), 'user1'),
    ('What is Entity Framework?', 'Search results for EF', GETDATE(), 'user2'),
    ('Best practices for ASP.NET', 'Search results for ASP.NET practices', GETDATE(), 'user1'),
    ('Introduction to LocalDB', 'Search results for LocalDB', GETDATE(), 'user3'),
    ('How to connect to SQL Server', 'Search results for SQL Server connection', GETDATE(), 'user2');
	
	
## Running the Application
1. In Visual Studio, set the project as the startup project.

2. Press F5 or click on the Start button to run the application.

3. The application will launch and open in your default web browser.

4. Navigate to /swagger in the URL to access the Swagger UI, which allows you to test the API endpoints interactively.

```
http://localhost:5116/swagger
```