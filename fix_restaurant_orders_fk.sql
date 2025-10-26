ALTER TABLE ""RestaurantOrders"" ADD COLUMN IF NOT EXISTS ""RestaurantTableId"" uuid NULL;
DO fix_restaurant_orders_fk.sql
BEGIN
IF NOT EXISTS (
  SELECT 1 FROM pg_constraint WHERE conname = 'FK_RestaurantOrders_RestaurantTables_RestaurantTableId'
) THEN
  ALTER TABLE ""RestaurantOrders"" ADD CONSTRAINT ""FK_RestaurantOrders_RestaurantTables_RestaurantTableId"" FOREIGN KEY (""RestaurantTableId"") REFERENCES ""RestaurantTables""(""Id"") ON DELETE SET NULL;
END IF;
ENDfix_restaurant_orders_fk.sql;
