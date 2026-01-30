# Monri

## Docker Compose Setup  
There are two ways of starting the project, the docker compose way and the manual setup
For the docker compose way there are again two ways to do this:
1. Inside the terminal position yourself inside the root directory of the project, (ex. C:\source\repos\Monri) and execute the **docker-compose up --build** command and open the MVC app inside the browser (http://localhost:5001)
2. The alternate way is to select the **docker-compose** launch profile inside visual studio and run it. Again navigate to http://localhost:5001 to open the MVC app
   
---
## Manual Setup

Before running the projects, you need to set up your **SQL Server** database. Follow these steps:

1. Create an SQL Server instance.  

2. **Run the necessary scripts** to create the tables, stored procedures and view.

```sql
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Monri')
BEGIN
    CREATE DATABASE Monri;
END
GO

USE Monri;
GO
IF OBJECT_ID('Geo', 'U') IS NULL
BEGIN
    CREATE TABLE Geo (
    Id INT IDENTITY PRIMARY KEY,
    Latitude NVARCHAR(50) NOT NULL,
    Longitude NVARCHAR(50) NOT NULL,

    Created DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    Updated DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    Deleted DATETIME2 NULL,
    DeletedBy UNIQUEIDENTIFIER NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
END
GO
IF OBJECT_ID('Address', 'U') IS NULL
BEGIN
    CREATE TABLE Address (
    Id INT IDENTITY PRIMARY KEY,
    Street NVARCHAR(100) NOT NULL,
    Suite NVARCHAR(100) NOT NULL,
    City NVARCHAR(100) NOT NULL,
    Zipcode NVARCHAR(20) NOT NULL,
    GeoId INT NOT NULL,

    Created DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    Updated DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    Deleted DATETIME2 NULL,
    DeletedBy UNIQUEIDENTIFIER NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Address_Geo
        FOREIGN KEY (GeoId)
        REFERENCES Geo(Id)
        ON DELETE CASCADE
);
END
GO

IF OBJECT_ID('Company', 'U') IS NULL
BEGIN
    CREATE TABLE Company (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CatchPhrase NVARCHAR(100) NOT NULL,
    Bs NVARCHAR(100) NOT NULL,

    Created DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    Updated DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    Deleted DATETIME2 NULL,
    DeletedBy UNIQUEIDENTIFIER NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
END
GO

IF OBJECT_ID('Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Username NVARCHAR(100) NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(50) NULL,
    Website NVARCHAR(100) NULL,

    AddressId INT NULL,
    CompanyId INT NULL,

    Created DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    Updated DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    Deleted DATETIME2 NULL,
    DeletedBy UNIQUEIDENTIFIER NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    CONSTRAINT FK_Users_Address
        FOREIGN KEY (AddressId)
        REFERENCES Address(Id),

    CONSTRAINT FK_Users_Company
        FOREIGN KEY (CompanyId)
        REFERENCES Company(Id)
);
END
GO

IF OBJECT_ID('SubmissionCheck', 'P') IS NULL
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[SubmissionCheck]
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1
        FROM Users
        WHERE Email = @Email
          AND Created >= DATEADD(MINUTE, -1, SYSUTCDATETIME())
    )
    BEGIN
        SELECT 1 AS Status;
    END
    ELSE
    BEGIN
        
        SELECT 0 AS Status;
    END
END
    ')
END
GO

IF OBJECT_ID('InsertUserWithDetails', 'P') IS NULL
BEGIN
    EXEC('
    CREATE PROCEDURE [dbo].[InsertUserWithDetails]
    @Name NVARCHAR(100),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Username NVARCHAR(100),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(50),
    @Website NVARCHAR(100),
    @Street NVARCHAR(100),
    @Suite NVARCHAR(100),
    @City NVARCHAR(100),
    @Zipcode NVARCHAR(20),
    @Latitude NVARCHAR(50),
    @Longitude NVARCHAR(50),
    @CompanyName NVARCHAR(100),
    @CompanyCatchPhrase NVARCHAR(100),
    @CompanyBs NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @GeoId INT;
    DECLARE @AddressId INT;
    DECLARE @CompanyId INT;
    DECLARE @Status INT = 0;

    BEGIN TRY
        BEGIN TRANSACTION;
        BEGIN

            INSERT INTO Geo (Latitude, Longitude)
            VALUES (@Latitude, @Longitude);
            SET @GeoId = SCOPE_IDENTITY();

            INSERT INTO Address (Street, Suite, City, Zipcode, GeoId)
            VALUES (@Street, @Suite, @City, @Zipcode, @GeoId);
            SET @AddressId = SCOPE_IDENTITY();

            INSERT INTO Company (Name, CatchPhrase, Bs)
            VALUES (@CompanyName, @CompanyCatchPhrase, @CompanyBs);
            SET @CompanyId = SCOPE_IDENTITY();

            INSERT INTO Users (Name, FirstName, LastName, Username, Email, Phone, Website, AddressId, CompanyId)
            VALUES (@Name, @FirstName, @LastName, @Username, @Email, @Phone, @Website, @AddressId, @CompanyId);

            SET @Status = 1;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SET @Status = 0;
    END CATCH

    -- Vrati status
    SELECT @Status AS Status;
END
    ')
END
GO


IF OBJECT_ID('UsersWithBizEmail', 'V') IS  NULL
BEGIN
    EXEC('
        CREATE VIEW UsersWithBizEmail
        AS
        SELECT *
        FROM Users
        WHERE Email LIKE ''%.biz'';')
END
GO
```


## Project Setup

1. In the API project update the appsettings.Development.json with your connection string
```
{
  "ConnectionStrings": {
    "DefaultConnection": "{yourDetailsHere}"
  }
}
```
2. Select the **Startup** launch profile and start the project

