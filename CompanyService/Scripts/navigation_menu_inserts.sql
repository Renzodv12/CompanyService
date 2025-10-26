-- Script para insertar menús de navegación del sistema ERP
-- Estructura solicitada: Dashboards, General, Inventario, Ventas, Clientes, CRM, Finanzas, Reportes
-- Ejecutar después de aplicar las migraciones

-- Limpiar menús existentes (opcional - comentar si no se desea limpiar)
-- DELETE FROM "Menus" WHERE "ParentId" IS NOT NULL;
-- DELETE FROM "Menus" WHERE "ParentId" IS NULL;

-- Menús principales (nivel 1)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
-- Dashboards
('11111111-1111-1111-1111-111111111111', 'Dashboards', 'dashboard', '/dashboards', 'Panel principal con métricas y resúmenes', NULL, 1, true, NOW()),

-- General
('22222222-2222-2222-2222-222222222222', 'General', 'apps', '/general', 'Funciones generales del sistema', NULL, 2, true, NOW()),

-- Inventario
('33333333-3333-3333-3333-333333333333', 'Inventario', 'inventory', '/inventory', 'Control de stock y productos', NULL, 3, true, NOW()),

-- Ventas
('44444444-4444-4444-4444-444444444444', 'Ventas', 'shopping_cart', '/sales', 'Gestión de ventas y facturación', NULL, 4, true, NOW()),

-- Clientes
('55555555-5555-5555-5555-555555555555', 'Clientes', 'people', '/customers', 'Gestión de clientes', NULL, 5, true, NOW()),

-- CRM
('66666666-6666-6666-6666-666666666666', 'CRM', 'contact_support', '/crm', 'Gestión de relaciones con clientes', NULL, 6, true, NOW()),

-- Finanzas
('77777777-7777-7777-7777-777777777777', 'Finanzas', 'account_balance', '/finance', 'Gestión financiera y contable', NULL, 7, true, NOW()),

-- Reportes
('88888888-8888-8888-8888-888888888888', 'Reportes', 'assessment', '/reports', 'Reportes y análisis', NULL, 8, true, NOW());

-- Submenús de Dashboards (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('11111111-1111-1111-1111-111111111101', 'Overview', 'dashboard', '/dashboards/overview', 'Vista general del dashboard', '11111111-1111-1111-1111-111111111111', 1, true, NOW());

-- Submenús de General (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('22222222-2222-2222-2222-222222222201', 'Calendario', 'calendar_today', '/general/calendar', 'Gestión de calendario y eventos', '22222222-2222-2222-2222-222222222222', 1, true, NOW()),
('22222222-2222-2222-2222-222222222202', 'Tareas', 'task', '/general/tasks', 'Gestión de tareas y proyectos', '22222222-2222-2222-2222-222222222222', 2, true, NOW()),
('22222222-2222-2222-2222-222222222203', 'Empresas', 'business', '/general/companies', 'Gestión de empresas', '22222222-2222-2222-2222-222222222222', 3, true, NOW()),
('22222222-2222-2222-2222-222222222204', 'Restaurantes', 'restaurant', '/general/restaurants', 'Gestión de restaurantes', '22222222-2222-2222-2222-222222222222', 4, true, NOW());

-- Submenús de Inventario (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('33333333-3333-3333-3333-333333333301', 'Productos', 'inventory_2', '/inventory/products', 'Gestión de productos', '33333333-3333-3333-3333-333333333333', 1, true, NOW()),
('33333333-3333-3333-3333-333333333302', 'Categorías', 'category', '/inventory/categories', 'Gestión de categorías', '33333333-3333-3333-3333-333333333333', 2, true, NOW());

-- Submenús de Ventas (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('44444444-4444-4444-4444-444444444401', 'Ventas', 'point_of_sale', '/sales', 'Gestión de ventas', '44444444-4444-4444-4444-444444444444', 1, true, NOW());

-- Submenús de Clientes (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('55555555-5555-5555-5555-555555555501', 'Clientes', 'people', '/customers', 'Gestión de clientes', '55555555-5555-5555-5555-555555555555', 1, true, NOW());

-- Submenús de CRM (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('66666666-6666-6666-6666-666666666601', 'CRM', 'contact_support', '/crm', 'Gestión de relaciones con clientes', '66666666-6666-6666-6666-666666666666', 1, true, NOW());

-- Submenús de Finanzas (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('77777777-7777-7777-7777-777777777701', 'Finanzas', 'account_balance', '/finance', 'Gestión financiera', '77777777-7777-7777-7777-777777777777', 1, true, NOW());

-- Submenús de Reportes (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('88888888-8888-8888-8888-888888888801', 'Reportes', 'assessment', '/reports', 'Reportes y análisis', '88888888-8888-8888-8888-888888888888', 1, true, NOW());

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
