/* =========================================================
   004_Seed_Menu.Sql
   Purpose: Insert default system Menus and Submenus with
            PermissionCode mapping for MediStock360
   ========================================================= */

SET NOCOUNT ON;

-- Helper table variable to hold menu definitions
DECLARE @MenuSeed TABLE (
    MenuName NVARCHAR(200),
    ParentMenuName NVARCHAR(200) NULL,
    MenuIcon NVARCHAR(50),
    RouterLink NVARCHAR(100),
    PermissionCode NVARCHAR(100) NULL,
    DisplayOrder INT,
    IsVisible BIT,
    IsActive INT
);

-- 1. Top-Level Menus
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('Dashboard', NULL, 'dashboard', '/dashboard', 'DASHBOARD_VIEW', 1, 1, 1),
('Administration', NULL, 'admin_panel_settings', '/admin', 'ADMIN_VIEW', 2, 1, 1),
('Inventory & Stock', NULL, 'inventory_2', '/inventory', 'INVENTORY_VIEW', 3, 1, 1),
('Purchases & Suppliers', NULL, 'shopping_cart', '/purchases', 'PURCHASE_VIEW', 4, 1, 1),
('Sales & POS', NULL, 'point_of_sale', '/sales', 'SALES_VIEW', 5, 1, 1),
('Customers & Prescriptions', NULL, 'person_search', '/customers', 'CUSTOMER_VIEW', 6, 1, 1),
('Reports & Analytics', NULL, 'assessment', '/reports', 'REPORT_VIEW', 7, 1, 1),
('Settings', NULL, 'settings', '/settings', 'SETTING_VIEW', 8, 1, 1);

-- 2. Submenus for Administration
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('Clients', 'Administration', 'business', '/admin/clients', 'CLIENT_VIEW', 1, 1, 1),
('Stores & Branches', 'Administration', 'store', '/admin/stores', 'STORE_VIEW', 2, 1, 1),
('Users & Staff', 'Administration', 'people', '/admin/users', 'USER_VIEW', 3, 1, 1),
('Roles & Permissions', 'Administration', 'security', '/admin/roles-permissions', 'ROLE_VIEW', 4, 1, 1),
('Menu Management', 'Administration', 'menu_book', '/admin/menus', 'MENU_VIEW', 5, 1, 1);

-- 3. Submenus for Inventory & Stock
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('Product Catalog', 'Inventory & Stock', 'medication', '/inventory/products', 'PRODUCT_VIEW', 1, 1, 1),
('Stock Overview', 'Inventory & Stock', 'warehouse', '/inventory/stock', 'STOCK_VIEW', 2, 1, 1),
('Categories & Brands', 'Inventory & Stock', 'category', '/inventory/categories', 'CATEGORY_VIEW', 3, 1, 1),
('Batch & Expiry Tracker', 'Inventory & Stock', 'event_busy', '/inventory/batches', 'BATCH_VIEW', 4, 1, 1);

-- 4. Submenus for Purchases & Suppliers
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('Suppliers', 'Purchases & Suppliers', 'local_shipping', '/purchases/suppliers', 'SUPPLIER_VIEW', 1, 1, 1),
('Purchase Orders', 'Purchases & Suppliers', 'receipt_long', '/purchases/orders', 'PURCHASE_ORDER_VIEW', 2, 1, 1),
('Goods Received Notes (GRN)', 'Purchases & Suppliers', 'inventory', '/purchases/grn', 'GRN_VIEW', 3, 1, 1);

-- 5. Submenus for Sales & POS
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('POS Billing Counter', 'Sales & POS', 'receipt', '/sales/pos', 'POS_VIEW', 1, 1, 1),
('Invoices & Bills', 'Sales & POS', 'history', '/sales/invoices', 'INVOICE_VIEW', 2, 1, 1),
('Sales Returns', 'Sales & POS', 'assignment_return', '/sales/returns', 'SALES_RETURN_VIEW', 3, 1, 1);

-- 6. Submenus for Customers & Prescriptions
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('Customer Directory', 'Customers & Prescriptions', 'contacts', '/customers/list', 'CUSTOMER_VIEW', 1, 1, 1),
('Prescriptions', 'Customers & Prescriptions', 'description', '/customers/prescriptions', 'PRESCRIPTION_VIEW', 2, 1, 1);

-- 7. Submenus for Reports & Analytics
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('Sales Reports', 'Reports & Analytics', 'trending_up', '/reports/sales', 'REPORT_SALES_VIEW', 1, 1, 1),
('Inventory Reports', 'Reports & Analytics', 'inventory', '/reports/inventory', 'REPORT_INVENTORY_VIEW', 2, 1, 1),
('Financial Reports', 'Reports & Analytics', 'account_balance_wallet', '/reports/financial', 'REPORT_FINANCIAL_VIEW', 3, 1, 1);

-- 8. Submenus for Settings
INSERT INTO @MenuSeed (MenuName, ParentMenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive)
VALUES 
('Store Settings', 'Settings', 'storefront', '/settings/store', 'STORE_SETTING_VIEW', 1, 1, 1),
('General Settings', 'Settings', 'tune', '/settings/general', 'SETTING_VIEW', 2, 1, 1);

-- Process Insertion: First Parents
DECLARE @MenuName NVARCHAR(200), @ParentMenuName NVARCHAR(200), @MenuIcon NVARCHAR(50), 
        @RouterLink NVARCHAR(100), @PermissionCode NVARCHAR(100), @DisplayOrder INT, 
        @IsVisible BIT, @IsActive INT;

-- Insert Parents
DECLARE cur_parents CURSOR FOR 
SELECT MenuName, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive
FROM @MenuSeed
WHERE ParentMenuName IS NULL;

OPEN cur_parents;
FETCH NEXT FROM cur_parents INTO @MenuName, @MenuIcon, @RouterLink, @PermissionCode, @DisplayOrder, @IsVisible, @IsActive;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Menu WHERE MenuName = @MenuName)
    BEGIN
        INSERT INTO dbo.Menu (MenuName, ParentMenuId, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive, CreatedAt)
        VALUES (@MenuName, NULL, @MenuIcon, @RouterLink, @PermissionCode, @DisplayOrder, @IsVisible, @IsActive, SYSUTCDATETIME());
    END
    ELSE
    BEGIN
        UPDATE dbo.Menu
        SET MenuIcon = @MenuIcon,
            RouterLink = @RouterLink,
            PermissionCode = @PermissionCode,
            DisplayOrder = @DisplayOrder,
            IsVisible = @IsVisible,
            IsActive = @IsActive
        WHERE MenuName = @MenuName AND ParentMenuId IS NULL;
    END

    FETCH NEXT FROM cur_parents INTO @MenuName, @MenuIcon, @RouterLink, @PermissionCode, @DisplayOrder, @IsVisible, @IsActive;
END;

CLOSE cur_parents;
DEALLOCATE cur_parents;

-- Insert Children
DECLARE cur_children CURSOR FOR 
SELECT s.MenuName, s.ParentMenuName, s.MenuIcon, s.RouterLink, s.PermissionCode, s.DisplayOrder, s.IsVisible, s.IsActive
FROM @MenuSeed s
WHERE s.ParentMenuName IS NOT NULL;

OPEN cur_children;
FETCH NEXT FROM cur_children INTO @MenuName, @ParentMenuName, @MenuIcon, @RouterLink, @PermissionCode, @DisplayOrder, @IsVisible, @IsActive;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @ParentId INT;
    SELECT @ParentId = MenuId FROM dbo.Menu WHERE MenuName = @ParentMenuName;

    IF @ParentId IS NOT NULL
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.Menu WHERE MenuName = @MenuName)
        BEGIN
            INSERT INTO dbo.Menu (MenuName, ParentMenuId, MenuIcon, RouterLink, PermissionCode, DisplayOrder, IsVisible, IsActive, CreatedAt)
            VALUES (@MenuName, @ParentId, @MenuIcon, @RouterLink, @PermissionCode, @DisplayOrder, @IsVisible, @IsActive, SYSUTCDATETIME());
        END
        ELSE
        BEGIN
            UPDATE dbo.Menu
            SET ParentMenuId = @ParentId,
                MenuIcon = @MenuIcon,
                RouterLink = @RouterLink,
                PermissionCode = @PermissionCode,
                DisplayOrder = @DisplayOrder,
                IsVisible = @IsVisible,
                IsActive = @IsActive
            WHERE MenuName = @MenuName;
        END
    END

    FETCH NEXT FROM cur_children INTO @MenuName, @ParentMenuName, @MenuIcon, @RouterLink, @PermissionCode, @DisplayOrder, @IsVisible, @IsActive;
END;

CLOSE cur_children;
DEALLOCATE cur_children;

