-- Script de creación de entidades extendidas para empresas
-- Versión: 1.0
-- Fecha: 2025-01-09
-- Descripción: Crea tablas para tipos de contacto y entidades de contacto de empresas

-- Crear tabla AddressTypes
CREATE TABLE AddressTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UK_AddressTypes_Name UNIQUE (Name)
);

-- Crear tabla EmailTypes
CREATE TABLE EmailTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UK_EmailTypes_Name UNIQUE (Name)
);

-- Crear tabla PhoneTypes
CREATE TABLE PhoneTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UK_PhoneTypes_Name UNIQUE (Name)
);

-- Crear tabla SocialMediaTypes
CREATE TABLE SocialMediaTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UK_SocialMediaTypes_Name UNIQUE (Name)
);

-- Crear tabla CompanyAddresses
CREATE TABLE CompanyAddresses (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    AddressTypeId UNIQUEIDENTIFIER NOT NULL,
    Address NVARCHAR(500) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT FK_CompanyAddresses_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CompanyAddresses_AddressTypes FOREIGN KEY (AddressTypeId) REFERENCES AddressTypes(Id)
);

-- Crear tabla CompanyEmails
CREATE TABLE CompanyEmails (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    EmailTypeId UNIQUEIDENTIFIER NOT NULL,
    Email NVARCHAR(320) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT FK_CompanyEmails_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CompanyEmails_EmailTypes FOREIGN KEY (EmailTypeId) REFERENCES EmailTypes(Id),
    CONSTRAINT UK_CompanyEmails_Email UNIQUE (Email)
);

-- Crear tabla CompanyPhones
CREATE TABLE CompanyPhones (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    PhoneTypeId UNIQUEIDENTIFIER NOT NULL,
    Phone NVARCHAR(50) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT FK_CompanyPhones_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CompanyPhones_PhoneTypes FOREIGN KEY (PhoneTypeId) REFERENCES PhoneTypes(Id)
);

-- Crear tabla CompanySocialMedias
CREATE TABLE CompanySocialMedias (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    SocialMediaTypeId UNIQUEIDENTIFIER NOT NULL,
    Url NVARCHAR(500) NOT NULL,
    IsPrimary BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    CONSTRAINT FK_CompanySocialMedias_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CompanySocialMedias_SocialMediaTypes FOREIGN KEY (SocialMediaTypeId) REFERENCES SocialMediaTypes(Id)
);

-- Crear índices para optimizar consultas
CREATE INDEX IX_CompanyAddresses_CompanyId ON CompanyAddresses(CompanyId);
CREATE INDEX IX_CompanyAddresses_AddressTypeId ON CompanyAddresses(AddressTypeId);
CREATE INDEX IX_CompanyAddresses_IsPrimary ON CompanyAddresses(IsPrimary);

CREATE INDEX IX_CompanyEmails_CompanyId ON CompanyEmails(CompanyId);
CREATE INDEX IX_CompanyEmails_EmailTypeId ON CompanyEmails(EmailTypeId);
CREATE INDEX IX_CompanyEmails_IsPrimary ON CompanyEmails(IsPrimary);

CREATE INDEX IX_CompanyPhones_CompanyId ON CompanyPhones(CompanyId);
CREATE INDEX IX_CompanyPhones_PhoneTypeId ON CompanyPhones(PhoneTypeId);
CREATE INDEX IX_CompanyPhones_IsPrimary ON CompanyPhones(IsPrimary);

CREATE INDEX IX_CompanySocialMedias_CompanyId ON CompanySocialMedias(CompanyId);
CREATE INDEX IX_CompanySocialMedias_SocialMediaTypeId ON CompanySocialMedias(SocialMediaTypeId);
CREATE INDEX IX_CompanySocialMedias_IsPrimary ON CompanySocialMedias(IsPrimary);

-- Insertar datos iniciales para AddressTypes
INSERT INTO AddressTypes (Id, Name, Description) VALUES
(NEWID(), 'Principal', 'Dirección principal de la empresa'),
(NEWID(), 'Sucursal', 'Dirección de sucursal o filial'),
(NEWID(), 'Facturación', 'Dirección para facturación'),
(NEWID(), 'Envío', 'Dirección para envíos y entregas');

-- Insertar datos iniciales para EmailTypes
INSERT INTO EmailTypes (Id, Name, Description) VALUES
(NEWID(), 'Principal', 'Email principal de contacto'),
(NEWID(), 'Facturación', 'Email para facturación'),
(NEWID(), 'Soporte', 'Email para soporte técnico'),
(NEWID(), 'Comercial', 'Email para ventas y comercial');

-- Insertar datos iniciales para PhoneTypes
INSERT INTO PhoneTypes (Id, Name, Description) VALUES
(NEWID(), 'Principal', 'Teléfono principal de contacto'),
(NEWID(), 'Móvil', 'Teléfono móvil'),
(NEWID(), 'Fax', 'Número de fax'),
(NEWID(), 'WhatsApp', 'Número de WhatsApp');

-- Insertar datos iniciales para SocialMediaTypes
INSERT INTO SocialMediaTypes (Id, Name, Description) VALUES
(NEWID(), 'Facebook', 'Página de Facebook'),
(NEWID(), 'Instagram', 'Perfil de Instagram'),
(NEWID(), 'LinkedIn', 'Página de LinkedIn'),
(NEWID(), 'Twitter', 'Perfil de Twitter'),
(NEWID(), 'YouTube', 'Canal de YouTube');
