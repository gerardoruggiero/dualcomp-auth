-- Script de creación de entidades para validación de email y configuración de empresa
-- Versión: 1.0
-- Fecha: 2025-01-09
-- Descripción: Crea tablas para validación de email, configuración SMTP y campos adicionales en Users

-- Agregar nuevos campos a la tabla Users
ALTER TABLE Users ADD 
    IsEmailValidated BIT NOT NULL DEFAULT 0,
    MustChangePassword BIT NOT NULL DEFAULT 0,
    TemporaryPassword NVARCHAR(255) NULL,
    EmailValidatedAt DATETIME2 NULL;

-- Crear tabla EmailValidations para tokens de validación
CREATE TABLE EmailValidations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    Token NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpiresAt DATETIME2 NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    UsedAt DATETIME2 NULL,
    CONSTRAINT UK_EmailValidations_Token UNIQUE (Token),
    CONSTRAINT FK_EmailValidations_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Crear tabla CompanySettings para configuración SMTP por empresa
CREATE TABLE CompanySettings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    SmtpServer NVARCHAR(255) NOT NULL,
    SmtpPort INT NOT NULL DEFAULT 587,
    SmtpUsername NVARCHAR(255) NOT NULL,
    SmtpPassword NVARCHAR(500) NOT NULL, -- Encriptado
    SmtpUseSsl BIT NOT NULL DEFAULT 1,
    SmtpFromEmail NVARCHAR(320) NOT NULL,
    SmtpFromName NVARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CreatedBy UNIQUEIDENTIFIER,
    UpdatedBy UNIQUEIDENTIFIER,
    CONSTRAINT UK_CompanySettings_CompanyId UNIQUE (CompanyId),
    CONSTRAINT FK_CompanySettings_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CompanySettings_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(Id),
    CONSTRAINT FK_CompanySettings_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES Users(Id)
);

-- Crear tabla EmailLogs para auditoría de envío de emails
CREATE TABLE EmailLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    ToEmail NVARCHAR(320) NOT NULL,
    Subject NVARCHAR(500) NOT NULL,
    EmailType NVARCHAR(100) NOT NULL, -- 'Validation', 'PasswordReset', 'CompanyRegistration', etc.
    Status NVARCHAR(50) NOT NULL, -- 'Sent', 'Failed', 'Pending'
    ErrorMessage NVARCHAR(1000) NULL,
    SentAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_EmailLogs_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE
);

-- Crear índices para optimizar consultas
CREATE INDEX IX_EmailValidations_UserId ON EmailValidations(UserId);
CREATE INDEX IX_EmailValidations_Token ON EmailValidations(Token);
CREATE INDEX IX_EmailValidations_ExpiresAt ON EmailValidations(ExpiresAt);
CREATE INDEX IX_EmailValidations_IsUsed ON EmailValidations(IsUsed);

CREATE INDEX IX_CompanySettings_CompanyId ON CompanySettings(CompanyId);
CREATE INDEX IX_CompanySettings_IsActive ON CompanySettings(IsActive);

CREATE INDEX IX_EmailLogs_CompanyId ON EmailLogs(CompanyId);
CREATE INDEX IX_EmailLogs_ToEmail ON EmailLogs(ToEmail);
CREATE INDEX IX_EmailLogs_Status ON EmailLogs(Status);
CREATE INDEX IX_EmailLogs_CreatedAt ON EmailLogs(CreatedAt);

-- Crear índices para limpieza de tokens expirados
CREATE INDEX IX_EmailValidations_Expired ON EmailValidations(ExpiresAt, IsUsed) WHERE IsUsed = 0;

-- NOTA: La limpieza de tokens expirados y logs antiguos se manejará en la capa de aplicación
-- NOTA: La actualización de timestamps se manejará en la capa de aplicación

-- NOTA: La configuración SMTP por defecto se manejará en la capa de aplicación
-- cuando sea necesario crear configuraciones para empresas existentes

-- Comentarios sobre el diseño:
-- 1. EmailValidations: Almacena tokens únicos con expiración para validación de email
-- 2. CompanySettings: Configuración SMTP por empresa (multitenant)
-- 3. EmailLogs: Auditoría de todos los emails enviados
-- 4. Campos en Users: Control de validación y cambio obligatorio de contraseña
-- 5. Procedimientos de limpieza: Para mantener la base de datos optimizada
-- 6. Triggers: Para actualización automática de timestamps
