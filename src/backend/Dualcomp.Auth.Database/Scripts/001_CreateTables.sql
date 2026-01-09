-- Script de creación de tablas para el sistema de autenticación y gestión de empresas
-- Versión: 1.0
-- Fecha: 2025-01-09

-- Crear tabla Companies
CREATE TABLE Companies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    TaxId NVARCHAR(50) NOT NULL,
    Address NVARCHAR(500),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT UK_Companies_TaxId UNIQUE (TaxId)
);

-- Crear tabla Users
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastLoginAt DATETIME2,
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    IsCompanyAdmin BIT NOT NULL DEFAULT 0,
    CONSTRAINT UK_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id)
);

-- Crear tabla Employees
CREATE TABLE Employees (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(320) NOT NULL,
    Phone NVARCHAR(50),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER,
    Position NVARCHAR(100),
    HireDate DATETIME2,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT FK_Employees_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id),
    CONSTRAINT FK_Employees_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Crear tabla UserSessions
CREATE TABLE UserSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    AccessToken NVARCHAR(1000) NOT NULL,
    RefreshToken NVARCHAR(1000) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpiresAt DATETIME2 NOT NULL,
    LastUsedAt DATETIME2,
    IsActive BIT NOT NULL DEFAULT 1,
    UserAgent NVARCHAR(500),
    IpAddress NVARCHAR(45),
    CONSTRAINT UK_UserSessions_AccessToken UNIQUE (AccessToken),
    CONSTRAINT UK_UserSessions_RefreshToken UNIQUE (RefreshToken),
    CONSTRAINT FK_UserSessions_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Crear índices para optimizar consultas
CREATE INDEX IX_Users_CompanyId ON Users(CompanyId);
CREATE INDEX IX_Employees_CompanyId ON Employees(CompanyId);
CREATE INDEX IX_Employees_UserId ON Employees(UserId);
CREATE INDEX IX_Employees_Email ON Employees(Email);
CREATE INDEX IX_UserSessions_UserId ON UserSessions(UserId);
CREATE INDEX IX_UserSessions_ExpiresAt ON UserSessions(ExpiresAt);

-- Crear índices para limpieza de sesiones expiradas
CREATE INDEX IX_UserSessions_Expired ON UserSessions(ExpiresAt, IsActive) WHERE IsActive = 1;
