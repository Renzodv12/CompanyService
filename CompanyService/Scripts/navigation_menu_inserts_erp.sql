-- Script para insertar menús de navegación del sistema ERP
-- Base de datos: erp
-- Estructura basada en el JSON proporcionado

-- Limpiar menús existentes (opcional - comentar si no se desea limpiar)
-- DELETE FROM "Menus" WHERE "ParentId" IS NOT NULL;
-- DELETE FROM "Menus" WHERE "ParentId" IS NULL;

-- Menús principales (nivel 1)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
-- Dashboards
('dashboards', 'Dashboards', 'house', '/dashboard', 'Vista general del dashboard', NULL, 1, true, NOW()),

-- General
('general', 'General', 'gear', '/dashboard/general', 'Herramientas generales', NULL, 2, true, NOW()),

-- Inventario
('inventory', 'Inventario', 'package', '/dashboard/inventory', 'Gestión de inventario', NULL, 3, true, NOW()),

-- Ventas
('sales', 'Ventas', 'shopping-cart-simple', '/dashboard/sales', 'Gestión de ventas', NULL, 4, true, NOW()),

-- Clientes
('customers', 'Clientes', 'users', '/dashboard/customers', 'Gestión de clientes', NULL, 5, true, NOW()),

-- CRM
('crm', 'CRM', 'chart-pie', '/dashboard/crm', 'Gestión de relaciones con clientes', NULL, 6, true, NOW()),

-- Finanzas
('finance', 'Finanzas', 'currency-dollar', '/dashboard/finance', 'Gestión financiera', NULL, 7, true, NOW()),

-- Reportes
('reports', 'Reportes', 'chart-bar', '/dashboard/reports', 'Reportes y análisis', NULL, 8, true, NOW()),

-- Configuración
('settings', 'Configuración', 'gear', '/dashboard/settings', 'Configuración del sistema', NULL, 9, true, NOW());

-- Submenús de Dashboards (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('overview', 'Overview', 'house', '/dashboard', 'Vista general del dashboard', 'dashboards', 1, true, NOW());

-- Submenús de General (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('calendar', 'Calendario', 'calendar-check', '/dashboard/calendar', 'Gestión de calendario y eventos', 'general', 1, true, NOW()),
('tasks', 'Tareas', 'kanban', '/dashboard/tasks', 'Gestión de tareas y proyectos', 'general', 2, true, NOW()),
('companies', 'Empresas', 'buildings', '/dashboard/companies', 'Gestión de empresas', 'general', 3, true, NOW()),
('restaurants', 'Restaurantes', 'fork-knife', '/dashboard/restaurants', 'Gestión de restaurantes', 'general', 4, true, NOW());

-- Submenús de Inventario (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('products', 'Productos', 'package', '/dashboard/products', 'Gestión de productos', 'inventory', 1, true, NOW()),
('categories', 'Categorías', 'tag', '/dashboard/categories', 'Gestión de categorías', 'inventory', 2, true, NOW());

-- Submenús de Ventas (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('sales-overview', 'Ventas', 'shopping-cart-simple', '/dashboard/sales', 'Gestión de ventas', 'sales', 1, true, NOW());

-- Submenús de Clientes (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('customers-overview', 'Clientes', 'users', '/dashboard/customers', 'Gestión de clientes', 'customers', 1, true, NOW());

-- Submenús de CRM (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('crm-overview', 'CRM', 'chart-pie', '/dashboard/crm', 'Gestión de relaciones con clientes', 'crm', 1, true, NOW());

-- Submenús de Finanzas (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('finance-overview', 'Finanzas', 'currency-dollar', '/dashboard/finance', 'Gestión financiera', 'finance', 1, true, NOW());

-- Submenús de Reportes (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('reports-overview', 'Reportes', 'chart-bar', '/dashboard/reports', 'Reportes y análisis', 'reports', 1, true, NOW());

-- Submenús de Configuración (nivel 2)
INSERT INTO "Menus" ("Id", "Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('menu-configuration', 'Configuración de Menús', 'gear', '/dashboard/settings/menu-configuration', 'Configurar menús del sidebar', 'settings', 1, true, NOW()),
('security', 'Seguridad', 'shield-check', '/dashboard/settings/security', 'Configuración de seguridad', 'settings', 2, true, NOW());

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
