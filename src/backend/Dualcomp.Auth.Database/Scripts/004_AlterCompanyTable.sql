-- Script de alteración de tabla Companies
-- Versión: 1.0
-- Fecha: 2025-01-09
-- Descripción: Remueve la columna Address de la tabla Companies ya que ahora se maneja en CompanyAddresses

-- Verificar si la columna Address existe antes de eliminarla
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Companies' AND COLUMN_NAME = 'Address')
BEGIN
    -- Eliminar la columna Address de la tabla Companies
    ALTER TABLE Companies DROP COLUMN Address;
    
    PRINT 'Columna Address eliminada exitosamente de la tabla Companies.';
END
ELSE
BEGIN
    PRINT 'La columna Address no existe en la tabla Companies.';
END
