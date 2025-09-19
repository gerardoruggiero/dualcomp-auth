-- Crear tabla Modulos
CREATE TABLE Modulos (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT UK_Modulos_Name UNIQUE (Name)
);

-- Insertar datos iniciales para Modulos
INSERT INTO Modulos (Id, Name, Description) VALUES
(NEWID(), 'Usuarios', 'Módulo de gestión de usuarios'),
(NEWID(), 'Empresas', 'Módulo de gestión de empresas'),
(NEWID(), 'Reportes', 'Módulo de reportes y estadísticas'),
(NEWID(), 'Configuración', 'Módulo de configuración del sistema'),
(NEWID(), 'Seguridad', 'Módulo de seguridad y permisos'),
(NEWID(), 'Inventario', 'Módulo de gestión de inventario'),
(NEWID(), 'Ventas', 'Módulo de gestión de ventas'),
(NEWID(), 'Contabilidad', 'Módulo de contabilidad y finanzas');
