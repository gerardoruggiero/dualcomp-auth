-- Script de creación de entidades Titles y DocumentTypes
-- Versión: 1.0
-- Fecha: 2025-01-09
-- Descripción: Crea tablas para títulos profesionales y tipos de documentos

-- Crear tabla Titles
CREATE TABLE Titles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UK_Titles_Name UNIQUE (Name)
);

-- Crear tabla DocumentTypes
CREATE TABLE DocumentTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UK_DocumentTypes_Name UNIQUE (Name)
);

-- Insertar datos iniciales para Titles
INSERT INTO Titles (Id, Name, Description) VALUES
(NEWID(), 'Ingeniero', 'Título de Ingeniero'),
(NEWID(), 'Doctor', 'Título de Doctor'),
(NEWID(), 'Licenciado', 'Título de Licenciado'),
(NEWID(), 'Técnico', 'Título de Técnico'),
(NEWID(), 'Magíster', 'Título de Magíster'),
(NEWID(), 'Especialista', 'Título de Especialista'),
(NEWID(), 'Bachiller', 'Título de Bachiller'),
(NEWID(), 'Postgrado', 'Título de Postgrado');

-- Insertar datos iniciales para DocumentTypes
INSERT INTO DocumentTypes (Id, Name, Description) VALUES
(NEWID(), 'DNI', 'Documento Nacional de Identidad'),
(NEWID(), 'Pasaporte', 'Pasaporte'),
(NEWID(), 'Cédula', 'Cédula de Identidad'),
(NEWID(), 'RUC', 'Registro Único de Contribuyentes'),
(NEWID(), 'Carné de Extranjería', 'Carné de Extranjería'),
(NEWID(), 'Carné de Identidad', 'Carné de Identidad'),
(NEWID(), 'Licencia de Conducir', 'Licencia de Conducir'),
(NEWID(), 'Permiso de Trabajo', 'Permiso de Trabajo');
