-- Script para configurar menús por empresa
-- Este script permite activar/desactivar menús específicos para cada empresa

-- 1. Ver empresas disponibles
SELECT "Id", "Name" FROM "Companies" LIMIT 5;

-- 2. Ver menús disponibles
SELECT "Id", "Name", "ParentId" FROM "Menus" ORDER BY "ParentId" NULLS FIRST, "Order";

-- 3. Ejemplo: Activar todos los menús para una empresa específica
-- Reemplazar 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX' con el ID real de la empresa
/*
INSERT INTO "CompanyMenuConfigurations" ("CompanyId", "MenuId", "IsEnabled", "CreatedAt")
SELECT 
    'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX'::uuid, -- Reemplazar con el ID real de la empresa
    "Id",
    true, -- true = activado, false = desactivado
    NOW()
FROM "Menus"
WHERE "IsActive" = true;
*/

-- 4. Ejemplo: Activar solo menús específicos para una empresa
-- Reemplazar 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX' con el ID real de la empresa
/*
INSERT INTO "CompanyMenuConfigurations" ("CompanyId", "MenuId", "IsEnabled", "CreatedAt")
SELECT 
    'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX'::uuid, -- Reemplazar con el ID real de la empresa
    "Id",
    true,
    NOW()
FROM "Menus"
WHERE "Name" IN ('Dashboards', 'General', 'Inventario', 'Ventas', 'Clientes', 'Finanzas', 'Reportes');
*/

-- 5. Ejemplo: Desactivar un menú específico para una empresa
-- UPDATE "CompanyMenuConfigurations" 
-- SET "IsEnabled" = false, "UpdatedAt" = NOW()
-- WHERE "CompanyId" = 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX'::uuid 
-- AND "MenuId" = (SELECT "Id" FROM "Menus" WHERE "Name" = 'CRM');

-- 6. Verificar configuración actual de menús por empresa
/*
SELECT 
    c."Name" as "Empresa",
    m."Name" as "Menú",
    m."Icon" as "Icono",
    m."Route" as "Ruta",
    cmc."IsEnabled" as "Activo",
    cmc."CreatedAt" as "Fecha Configuración"
FROM "CompanyMenuConfigurations" cmc
JOIN "Companies" c ON cmc."CompanyId" = c."Id"
JOIN "Menus" m ON cmc."MenuId" = m."Id"
WHERE c."Id" = 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX'::uuid -- Reemplazar con el ID real de la empresa
ORDER BY m."ParentId" NULLS FIRST, m."Order";
*/

-- 7. Consulta para obtener menús activos de una empresa (para el frontend)
/*
SELECT 
    m."Id",
    m."Name",
    m."Icon",
    m."Route",
    m."Description",
    m."ParentId",
    m."Order",
    m."IsActive"
FROM "Menus" m
WHERE m."IsActive" = true
AND m."Id" IN (
    SELECT "MenuId" 
    FROM "CompanyMenuConfigurations" 
    WHERE "CompanyId" = 'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX'::uuid -- Reemplazar con el ID real de la empresa
    AND "IsEnabled" = true
)
ORDER BY m."ParentId" NULLS FIRST, m."Order";
*/

