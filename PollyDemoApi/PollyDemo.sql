-- Create the PollyDemoDb database
CREATE DATABASE PollyDemoDb;
GO

-- Use the database
USE PollyDemoDb;
GO

-- Create a Users table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Create a table to log external API calls / retry attempts
CREATE TABLE ApiRequests (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Endpoint NVARCHAR(500),
    RequestBody NVARCHAR(MAX),
    ResponseBody NVARCHAR(MAX),
    StatusCode INT,
    Success BIT DEFAULT 0,
    RetryCount INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- Create a Logs table (optional)
CREATE TABLE Logs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Level NVARCHAR(50),
    Message NVARCHAR(MAX),
    StackTrace NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO


-- Table for logging retry policy attempts
CREATE TABLE RetryLogs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PolicyName NVARCHAR(100),
    RetryCount INT,
    Success BIT,
    Message NVARCHAR(MAX),
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE ExternalServiceCalls (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ServiceName NVARCHAR(100),
    RequestPayload NVARCHAR(MAX),
    ResponsePayload NVARCHAR(MAX),
    StatusCode INT,
    IsSuccessful BIT,
    AttemptedAt DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE TestItems (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NULL
);

select *from TestItems
SELECT * FROM PollyDemoDb.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TestItems';