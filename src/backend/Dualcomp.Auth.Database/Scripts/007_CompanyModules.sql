-- Crear tabla de unión CompanyModules
CREATE TABLE CompanyModules (
    CompanyId UNIQUEIDENTIFIER NOT NULL,
    ModuleId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY (CompanyId, ModuleId),
    CONSTRAINT FK_CompanyModules_Companies FOREIGN KEY (CompanyId) REFERENCES Companies(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CompanyModules_Modulos FOREIGN KEY (ModuleId) REFERENCES Modulos(Id) ON DELETE CASCADE
);
