-- Script para insertar menús de navegación del sistema ERP
-- Base de datos: erp
-- Estructura basada en el JSON proporcionado
-- Usando UUIDs para los IDs

-- Limpiar menús existentes (opcional - comentar si no se desea limpiar)
-- DELETE FROM "Menus" WHERE "ParentId" IS NOT NULL;
-- DELETE FROM "Menus" WHERE "ParentId" IS NULL;

-- Menús principales (nivel 1)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
-- Dashboards
('11111111-1111-1111-1111-111111111111', 'Dashboards', 'house', '/dashboard', 'Vista general del dashboard', NULL, 1, true, NOW()),

-- General
('22222222-2222-2222-2222-222222222222', 'General', 'gear', '/dashboard/general', 'Herramientas generales', NULL, 2, true, NOW()),

-- Inventario
('33333333-3333-3333-3333-333333333333', 'Inventario', 'package', '/dashboard/inventory', 'Gestión de inventario', NULL, 3, true, NOW()),

-- Ventas
('44444444-4444-4444-4444-444444444444', 'Ventas', 'shopping-cart-simple', '/dashboard/sales', 'Gestión de ventas', NULL, 4, true, NOW()),

-- Clientes
('55555555-5555-5555-5555-555555555555', 'Clientes', 'users', '/dashboard/customers', 'Gestión de clientes', NULL, 5, true, NOW()),

-- CRM
('66666666-6666-6666-6666-666666666666', 'CRM', 'chart-pie', '/dashboard/crm', 'Gestión de relaciones con clientes', NULL, 6, true, NOW()),

-- Finanzas
('77777777-7777-7777-7777-777777777777', 'Finanzas', 'currency-dollar', '/dashboard/finance', 'Gestión financiera', NULL, 7, true, NOW()),

-- Reportes
('88888888-8888-8888-8888-888888888888', 'Reportes', 'chart-bar', '/dashboard/reports', 'Reportes y análisis', NULL, 8, true, NOW()),

-- Configuración
('99999999-9999-9999-9999-999999999999', 'Configuración', 'gear', '/dashboard/settings', 'Configuración del sistema', NULL, 9, true, NOW());

-- Submenús de Dashboards (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('11111111-1111-1111-1111-111111111101', 'Overview', 'house', '/dashboard', 'Vista general del dashboard', '11111111-1111-1111-1111-111111111111', 1, true, NOW());

-- Submenús de General (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('22222222-2222-2222-2222-222222222201', 'Calendario', 'calendar-check', '/dashboard/calendar', 'Gestión de calendario y eventos', '22222222-2222-2222-2222-222222222222', 1, true, NOW()),
('22222222-2222-2222-2222-222222222202', 'Tareas', 'kanban', '/dashboard/tasks', 'Gestión de tareas y proyectos', '22222222-2222-2222-2222-222222222222', 2, true, NOW()),
('22222222-2222-2222-2222-222222222203', 'Empresas', 'buildings', '/dashboard/companies', 'Gestión de empresas', '22222222-2222-2222-2222-222222222222', 3, true, NOW()),
('22222222-2222-2222-2222-222222222204', 'Restaurantes', 'fork-knife', '/dashboard/restaurants', 'Gestión de restaurantes', '22222222-2222-2222-2222-222222222222', 4, true, NOW());

-- Submenús de Inventario (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('33333333-3333-3333-3333-333333333301', 'Productos', 'package', '/dashboard/products', 'Gestión de productos', '33333333-3333-3333-3333-333333333333', 1, true, NOW()),
('33333333-3333-3333-3333-333333333302', 'Categorías', 'tag', '/dashboard/categories', 'Gestión de categorías', '33333333-3333-3333-3333-333333333333', 2, true, NOW());

-- Submenús de Ventas (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('44444444-4444-4444-4444-444444444401', 'Ventas', 'shopping-cart-simple', '/dashboard/sales', 'Gestión de ventas', '44444444-4444-4444-4444-444444444444', 1, true, NOW());

-- Submenús de Clientes (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('55555555-5555-5555-5555-555555555501', 'Clientes', 'users', '/dashboard/customers', 'Gestión de clientes', '55555555-5555-5555-5555-555555555555', 1, true, NOW());

-- Submenús de CRM (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('66666666-6666-6666-6666-666666666601', 'CRM', 'chart-pie', '/dashboard/crm', 'Gestión de relaciones con clientes', '66666666-6666-6666-6666-666666666666', 1, true, NOW());

-- Submenús de Finanzas (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('77777777-7777-7777-7777-777777777701', 'Finanzas', 'currency-dollar', '/dashboard/finance', 'Gestión financiera', '77777777-7777-7777-7777-777777777777', 1, true, NOW());

-- Submenús de Reportes (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('88888888-8888-8888-8888-888888888801', 'Reportes', 'chart-bar', '/dashboard/reports', 'Reportes y análisis', '88888888-8888-8888-8888-888888888888', 1, true, NOW());

-- Submenús de Configuración (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('99999999-9999-9999-9999-999999999901', 'Configuración de Menús', 'gear', '/dashboard/settings/menu-configuration', 'Configurar menús del sidebar', '99999999-9999-9999-9999-999999999999', 1, true, NOW()),
('99999999-9999-9999-9999-999999999902', 'Seguridad', 'shield-check', '/dashboard/settings/security', 'Configuración de seguridad', '99999999-9999-9999-9999-999999999999', 2, true, NOW());

-- Consulta para verificar la estructura de menús creada
-- SELECT 
--     m1."Name" as "Menu Principal",
--     m1."Icon" as "Icono Principal",
--     m1."Route" as "Ruta Principal",
--     m2."Name" as "Submenu",
--     m2."Icon" as "Icono Submenu",
--     m2."Route" as "Ruta Submenu",
--     m2."Order" as "Orden"
-- FROM "Menus" m1
-- LEFT JOIN "Menus" m2 ON m1."Id" = m2."ParentId"
-- WHERE m1."ParentId" IS NULL
-- ORDER BY m1."Order", m2."Order";

-- Nota: Para aplicar estos menús a una empresa específica, ejecutar:
-- INSERT INTO "CompanyMenuConfigurations" ("CompanyId", "MenuId", "IsEnabled", "CreatedAt")
-- SELECT 
--     'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX'::uuid, -- Reemplazar con el ID real de la empresa
--     "Id",
--     true,
--     NOW()
-- FROM "Menus"
-- WHERE "IsActive" = true;
