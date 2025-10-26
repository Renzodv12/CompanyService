-- Script para insertar menús de navegación del sistema ERP
-- Base de datos: erp
-- Usando IDs autoincrementales (integer) como está en la tabla real

-- Limpiar menús existentes (opcional - comentar si no se desea limpiar)
-- DELETE FROM "Menus" WHERE "ParentId" IS NOT NULL;
-- DELETE FROM "Menus" WHERE "ParentId" IS NULL;

-- Menús principales (nivel 1) - sin especificar Id para que sea autoincremental
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
-- Dashboards
('Dashboards', 'house', '/dashboard', 'Vista general del dashboard', NULL, 1, true, NOW()),

-- General
('General', 'gear', '/dashboard/general', 'Herramientas generales', NULL, 2, true, NOW()),

-- Inventario
('Inventario', 'package', '/dashboard/inventory', 'Gestión de inventario', NULL, 3, true, NOW()),

-- Ventas
('Ventas', 'shopping-cart-simple', '/dashboard/sales', 'Gestión de ventas', NULL, 4, true, NOW()),

-- Clientes
('Clientes', 'users', '/dashboard/customers', 'Gestión de clientes', NULL, 5, true, NOW()),

-- CRM
('CRM', 'chart-pie', '/dashboard/crm', 'Gestión de relaciones con clientes', NULL, 6, true, NOW()),

-- Finanzas
('Finanzas', 'currency-dollar', '/dashboard/finance', 'Gestión financiera', NULL, 7, true, NOW()),

-- Reportes
('Reportes', 'chart-bar', '/dashboard/reports', 'Reportes y análisis', NULL, 8, true, NOW()),

-- Configuración
('Configuración', 'gear', '/dashboard/settings', 'Configuración del sistema', NULL, 9, true, NOW());

-- Submenús de Dashboards (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Overview', 'house', '/dashboard', 'Vista general del dashboard', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Dashboards'), 1, true, NOW());

-- Submenús de General (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Calendario', 'calendar-check', '/dashboard/calendar', 'Gestión de calendario y eventos', (SELECT "Id" FROM "Menus" WHERE "Name" = 'General'), 1, true, NOW()),
('Tareas', 'kanban', '/dashboard/tasks', 'Gestión de tareas y proyectos', (SELECT "Id" FROM "Menus" WHERE "Name" = 'General'), 2, true, NOW()),
('Empresas', 'buildings', '/dashboard/companies', 'Gestión de empresas', (SELECT "Id" FROM "Menus" WHERE "Name" = 'General'), 3, true, NOW()),
('Restaurantes', 'fork-knife', '/dashboard/restaurants', 'Gestión de restaurantes', (SELECT "Id" FROM "Menus" WHERE "Name" = 'General'), 4, true, NOW());

-- Submenús de Inventario (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Productos', 'package', '/dashboard/products', 'Gestión de productos', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Inventario'), 1, true, NOW()),
('Categorías', 'tag', '/dashboard/categories', 'Gestión de categorías', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Inventario'), 2, true, NOW());

-- Submenús de Ventas (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Ventas', 'shopping-cart-simple', '/dashboard/sales', 'Gestión de ventas', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Ventas'), 1, true, NOW());

-- Submenús de Clientes (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Clientes', 'users', '/dashboard/customers', 'Gestión de clientes', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Clientes'), 1, true, NOW());

-- Submenús de CRM (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('CRM', 'chart-pie', '/dashboard/crm', 'Gestión de relaciones con clientes', (SELECT "Id" FROM "Menus" WHERE "Name" = 'CRM'), 1, true, NOW());

-- Submenús de Finanzas (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Finanzas', 'currency-dollar', '/dashboard/finance', 'Gestión financiera', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Finanzas'), 1, true, NOW());

-- Submenús de Reportes (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Reportes', 'chart-bar', '/dashboard/reports', 'Reportes y análisis', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Reportes'), 1, true, NOW());

-- Submenús de Configuración (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Configuración de Menús', 'gear', '/dashboard/settings/menu-configuration', 'Configurar menús del sidebar', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Configuración'), 1, true, NOW()),
('Seguridad', 'shield-check', '/dashboard/settings/security', 'Configuración de seguridad', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Configuración'), 2, true, NOW());

-- Consulta para verificar la estructura de menús creada
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
