-- Script de datos iniciales para el sistema de autenticación y gestión de empresas
-- Versión: 1.0
-- Fecha: 2025-01-09

-- Insertar empresa de ejemplo
INSERT INTO Companies (Id, Name, TaxId, Address) 
VALUES (
    '11111111-1111-1111-1111-111111111111',
    'DualComp Solutions',
    '12345678-9',
    'Av. Principal 123, Santiago, Chile'
);

-- Insertar usuario administrador de la empresa
-- Contraseña: Admin123! (hasheada con BCrypt)
INSERT INTO Users (Id, FirstName, LastName, Email, Password, CompanyId, IsCompanyAdmin) 
VALUES (
    '22222222-2222-2222-2222-222222222222',
    'Admin',
    'Sistema',
    'admin@dualcomp.com',
    '$2a$11$VnD9sgvkKBTTIu1YyIGVB.6QZer70ETQbKE2H76i0pNNKTgBktQ3m', -- Admin123!
    '11111111-1111-1111-1111-111111111111',
    1
);

-- Insertar empleado de ejemplo
INSERT INTO Employees (Id, FullName, Email, Phone, CompanyId, Position, HireDate, UserId) 
VALUES (
    '33333333-3333-3333-3333-333333333333',
    'Juan Pérez',
    'juan.perez@dualcomp.com',
    '+56912345678',
    '11111111-1111-1111-1111-111111111111',
    'Desarrollador Senior',
    '2024-01-15',
    '22222222-2222-2222-2222-222222222222'
);

-- Insertar usuario de prueba sin empresa
-- Contraseña: Test123! (hasheada con BCrypt)
INSERT INTO Users (Id, FirstName, LastName, Email, Password) 
VALUES (
    '44444444-4444-4444-4444-444444444444',
    'Usuario',
    'Prueba',
    'test@dualcomp.com',
    '$2a$11$2XCP2SmZZRXQKKR2At6cvesPSkEx6nxYHXrs92S.hz25luS/AOLBO' -- Test123!
);
