/* =========================================================
   001_Seed_Permission.Sql
   Purpose: Insert default system permissions for MediStock360
   ========================================================= */

SET NOCOUNT ON;

DECLARE @Permissions TABLE (
    PermissionCode NVARCHAR(100),
    PermissionName NVARCHAR(150),
    ModuleName NVARCHAR(100),
    Description NVARCHAR(500)
);

INSERT INTO @Permissions (PermissionCode, PermissionName, ModuleName, Description)
VALUES
-- Dashboard
('DASHBOARD_VIEW', 'View Dashboard', 'Dashboard', 'Allows user to view executive dashboard'),

-- Administration
('ADMIN_VIEW', 'View Administration', 'Administration', 'Allows user to view administration menu'),
('CLIENT_VIEW', 'View Client', 'Administration', 'Allows user to view client information'),
('CLIENT_CREATE', 'Create Client', 'Administration', 'Allows user to create client account'),
('CLIENT_EDIT', 'Edit Client', 'Administration', 'Allows user to edit client profile'),
('STORE_VIEW', 'View Store', 'Administration', 'Allows user to view stores and branches'),
('STORE_CREATE', 'Create Store', 'Administration', 'Allows user to create new stores'),
('STORE_EDIT', 'Edit Store', 'Administration', 'Allows user to edit store settings'),
('USER_VIEW', 'View Users', 'Administration', 'Allows user to view user accounts'),
('USER_CREATE', 'Create User', 'Administration', 'Allows user to create new user accounts'),
('USER_EDIT', 'Edit User', 'Administration', 'Allows user to edit user profiles'),
('USER_DELETE', 'Delete User', 'Administration', 'Allows user to deactivate/delete users'),
('ROLE_VIEW', 'View Roles', 'Administration', 'Allows user to view roles and mappings'),
('ROLE_MANAGE', 'Manage Roles', 'Administration', 'Allows user to assign roles and permissions'),
('MENU_VIEW', 'View Menus', 'Administration', 'Allows user to view dynamic menus'),
('MENU_MANAGE', 'Manage Menus', 'Administration', 'Allows user to configure menus and submenus'),

-- Inventory & Stock
('INVENTORY_VIEW', 'View Inventory', 'Inventory', 'Allows user to view inventory module'),
('PRODUCT_VIEW', 'View Products', 'Inventory', 'Allows user to view medicines and products'),
('PRODUCT_CREATE', 'Create Product', 'Inventory', 'Allows user to add new medicines and products'),
('PRODUCT_EDIT', 'Edit Product', 'Inventory', 'Allows user to update product information'),
('PRODUCT_DELETE', 'Delete Product', 'Inventory', 'Allows user to delete/deactivate products'),
('STOCK_VIEW', 'View Stock', 'Inventory', 'Allows user to view real-time stock levels'),
('STOCK_ADJUST', 'Adjust Stock', 'Inventory', 'Allows user to perform stock adjustments'),
('CATEGORY_VIEW', 'View Categories', 'Inventory', 'Allows user to view categories and brands'),
('BATCH_VIEW', 'View Batches & Expiry', 'Inventory', 'Allows user to track batch numbers and expiry dates'),

-- Purchases & Suppliers
('PURCHASE_VIEW', 'View Purchases', 'Purchases', 'Allows user to view purchase management module'),
('SUPPLIER_VIEW', 'View Suppliers', 'Purchases', 'Allows user to view suppliers directory'),
('SUPPLIER_MANAGE', 'Manage Suppliers', 'Purchases', 'Allows user to add/edit suppliers'),
('PURCHASE_ORDER_VIEW', 'View Purchase Orders', 'Purchases', 'Allows user to view purchase orders'),
('PURCHASE_ORDER_CREATE', 'Create Purchase Order', 'Purchases', 'Allows user to create purchase orders'),
('GRN_VIEW', 'View Goods Received Notes', 'Purchases', 'Allows user to view and create GRNs'),

-- Sales & POS
('SALES_VIEW', 'View Sales', 'Sales', 'Allows user to view sales operations'),
('POS_VIEW', 'Access POS Counter', 'Sales', 'Allows user to operate point of sale billing'),
('INVOICE_VIEW', 'View Invoices', 'Sales', 'Allows user to view sales invoices and history'),
('SALES_RETURN_VIEW', 'View Sales Returns', 'Sales', 'Allows user to process sales returns and refunds'),

-- Customers & Prescriptions
('CUSTOMER_VIEW', 'View Customers', 'Customers', 'Allows user to view customer records'),
('CUSTOMER_MANAGE', 'Manage Customers', 'Customers', 'Allows user to add and edit customer records'),
('PRESCRIPTION_VIEW', 'View Prescriptions', 'Customers', 'Allows user to view and attach prescriptions'),

-- Reports & Analytics
('REPORT_VIEW', 'View Reports', 'Reports', 'Allows user to access reports module'),
('REPORT_SALES_VIEW', 'View Sales Reports', 'Reports', 'Allows user to view sales performance reports'),
('REPORT_INVENTORY_VIEW', 'View Inventory Reports', 'Reports', 'Allows user to view stock movement reports'),
('REPORT_FINANCIAL_VIEW', 'View Financial Reports', 'Reports', 'Allows user to view P&L and financial reports'),

-- Settings
('SETTING_VIEW', 'View Settings', 'Settings', 'Allows user to view application settings'),
('STORE_SETTING_VIEW', 'View Store Settings', 'Settings', 'Allows user to view store configuration');

MERGE dbo.Permission AS target
USING @Permissions AS source
ON target.PermissionCode = source.PermissionCode
WHEN MATCHED THEN
    UPDATE SET 
        target.PermissionName = source.PermissionName,
        target.ModuleName = source.ModuleName,
        target.Description = source.Description,
        target.IsActive = 1
WHEN NOT MATCHED THEN
    INSERT (PermissionCode, PermissionName, ModuleName, Description, IsActive, CreatedAt)
    VALUES (source.PermissionCode, source.PermissionName, source.ModuleName, source.Description, 1, SYSUTCDATETIME());
--END;