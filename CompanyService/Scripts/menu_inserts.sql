-- Script para insertar menús básicos del sistema ERP
-- Ejecutar después de aplicar las migraciones

-- Menús principales (nivel 1)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Dashboard', 'dashboard', '/dashboard', 'Panel principal con métricas y resúmenes', NULL, 1, true, NOW()),
('Ventas', 'shopping_cart', '/sales', 'Gestión de ventas y facturación', NULL, 2, true, NOW()),
('Compras', 'shopping_bag', '/purchases', 'Gestión de compras y proveedores', NULL, 3, true, NOW()),
('Inventario', 'inventory', '/inventory', 'Control de stock y productos', NULL, 4, true, NOW()),
('Clientes', 'people', '/customers', 'Gestión de clientes', NULL, 5, true, NOW()),
('Proveedores', 'business', '/suppliers', '/suppliers', NULL, 6, true, NOW()),
('Contabilidad', 'account_balance', '/accounting', 'Gestión contable y financiera', NULL, 7, true, NOW()),
('Reportes', 'assessment', '/reports', 'Reportes y análisis', NULL, 8, true, NOW()),
('Tareas', 'task', '/tasks', 'Gestión de tareas y proyectos', NULL, 9, true, NOW()),
('Eventos', 'event', '/events', 'Calendario y gestión de eventos', NULL, 10, true, NOW()),
('Configuración', 'settings', '/settings', 'Configuración del sistema', NULL, 11, true, NOW());

-- Submenús de Ventas (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Nueva Venta', 'add_shopping_cart', '/sales/new', 'Crear nueva venta', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Ventas'), 1, true, NOW()),
('Lista de Ventas', 'list', '/sales/list', 'Ver todas las ventas', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Ventas'), 2, true, NOW()),
('Facturas', 'receipt', '/sales/invoices', 'Gestión de facturas', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Ventas'), 3, true, NOW());

-- Submenús de Compras (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Nueva Compra', 'add_shopping_cart', '/purchases/new', 'Registrar nueva compra', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Compras'), 1, true, NOW()),
('Lista de Compras', 'list', '/purchases/list', 'Ver todas las compras', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Compras'), 2, true, NOW()),
('Órdenes de Compra', 'assignment', '/purchases/orders', 'Gestión de órdenes de compra', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Compras'), 3, true, NOW());

-- Submenús de Inventario (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Productos', 'inventory_2', '/inventory/products', 'Gestión de productos', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Inventario'), 1, true, NOW()),
('Categorías', 'category', '/inventory/categories', 'Gestión de categorías', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Inventario'), 2, true, NOW()),
('Stock', 'storage', '/inventory/stock', 'Control de stock', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Inventario'), 3, true, NOW()),
('Movimientos', 'swap_horiz', '/inventory/movements', 'Historial de movimientos', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Inventario'), 4, true, NOW());

-- Submenús de Contabilidad (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Plan de Cuentas', 'account_tree', '/accounting/accounts', 'Plan de cuentas contables', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Contabilidad'), 1, true, NOW()),
('Asientos Contables', 'book', '/accounting/entries', 'Registro de asientos contables', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Contabilidad'), 2, true, NOW()),
('Balance General', 'balance', '/accounting/balance', 'Balance general', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Contabilidad'), 3, true, NOW()),
('Estado de Resultados', 'trending_up', '/accounting/income', 'Estado de pérdidas y ganancias', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Contabilidad'), 4, true, NOW());

-- Submenús de Reportes (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Reportes de Ventas', 'bar_chart', '/reports/sales', 'Análisis de ventas', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Reportes'), 1, true, NOW()),
('Reportes de Inventario', 'pie_chart', '/reports/inventory', 'Análisis de inventario', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Reportes'), 2, true, NOW()),
('Reportes Financieros', 'show_chart', '/reports/financial', 'Análisis financiero', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Reportes'), 3, true, NOW()),
('Reportes Personalizados', 'analytics', '/reports/custom', 'Reportes personalizados', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Reportes'), 4, true, NOW());

-- Submenús de Configuración (nivel 2)
INSERT INTO "Menus" ("Name", "Icon", "Route", "Description", "ParentId", "Order", "IsActive", "CreatedAt") VALUES
('Empresa', 'business_center', '/settings/company', 'Configuración de la empresa', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Configuración'), 1, true, NOW()),
('Usuarios', 'group', '/settings/users', 'Gestión de usuarios', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Configuración'), 2, true, NOW()),
('Roles y Permisos', 'security', '/settings/permissions', 'Configuración de roles y permisos', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Configuración'), 3, true, NOW()),
('Impuestos', 'receipt_long', '/settings/taxes', 'Configuración de impuestos', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Configuración'), 4, true, NOW()),
('Backup', 'backup', '/settings/backup', 'Respaldo y restauración', (SELECT "Id" FROM "Menus" WHERE "Name" = 'Configuración'), 5, true, NOW());

-- Ejemplo de configuración para una empresa (reemplazar el GUID con el ID real de la empresa)
-- INSERT INTO "CompanyMenuConfigurations" ("CompanyId", "MenuId", "IsEnabled", "CreatedAt")
-- SELECT 
--     'XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX'::uuid, -- Reemplazar con el ID real de la empresa
--     "Id",
--     true,
--     NOW()
-- FROM "Menus"
-- WHERE "IsActive" = true;

-- Consulta para verificar la estructura de menús creada
-- SELECT 
--     m1."Name" as "Menu Principal",
--     m2."Name" as "Submenu",
--     m2."Route" as "Ruta",
--     m2."Order" as "Orden"
-- FROM "Menus" m1
-- LEFT JOIN "Menus" m2 ON m1."Id" = m2."ParentId"
-- WHERE m1."ParentId" IS NULL
-- ORDER BY m1."Order", m2."Order";