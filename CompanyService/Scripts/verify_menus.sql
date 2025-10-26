-- Script de verificación de menús insertados
-- Ejecutar en pgAdmin o cliente PostgreSQL

-- Verificar menús principales
SELECT 
    "Name" as "Menu Principal",
    "Icon" as "Icono",
    "Route" as "Ruta",
    "Order" as "Orden"
FROM "Menus" 
WHERE "ParentId" IS NULL 
ORDER BY "Order";

-- Verificar estructura completa de menús
SELECT 
    m1."Name" as "Menu Principal",
    m1."Icon" as "Icono Principal",
    m1."Route" as "Ruta Principal",
    m2."Name" as "Submenu",
    m2."Icon" as "Icono Submenu",
    m2."Route" as "Ruta Submenu",
    m2."Order" as "Orden"
FROM "Menus" m1
LEFT JOIN "Menus" m2 ON m1."Id" = m2."ParentId"
WHERE m1."ParentId" IS NULL
ORDER BY m1."Order", m2."Order";

-- Contar total de menús insertados
SELECT 
    COUNT(*) as "Total Menús",
    COUNT(CASE WHEN "ParentId" IS NULL THEN 1 END) as "Menús Principales",
    COUNT(CASE WHEN "ParentId" IS NOT NULL THEN 1 END) as "Submenús"
FROM "Menus";
