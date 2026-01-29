# Monri

## Setup  

---
## Database Setup

Before running the projects, you need to set up your **SQL Server** database. Follow these steps:

1. **Create the database** in your SQL Server instance.  

2. **Run the necessary scripts** to create tables and stored procedures.

```sql
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

CREATE TABLE Address (
    Id INT IDENTITY PRIMARY KEY,
    Street NVARCHAR(200) NOT NULL,
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

CREATE TABLE Company (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    CatchPhrase NVARCHAR(300) NOT NULL,
    Bs NVARCHAR(200) NOT NULL,

    Created DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy UNIQUEIDENTIFIER NULL,
    Updated DATETIME2 NULL,
    UpdatedBy UNIQUEIDENTIFIER NULL,
    Deleted DATETIME2 NULL,
    DeletedBy UNIQUEIDENTIFIER NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

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

-- Create stored procedure for submission check
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

-- Create stored procedure for submission check user insert (with details)
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

-- Create DB View for users that have an email ending in .biz
CREATE VIEW UsersWithBizEmail
AS
SELECT 
    *
FROM Users
WHERE Email LIKE '%.biz';


--In the API project replace the connection string with yours
{
  "ConnectionStrings": {
    "DefaultConnection": "{yourDetailsHere}"
  }
}


