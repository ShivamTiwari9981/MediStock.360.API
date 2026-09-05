INSERT INTO RolePermission (RoleId, PermissionId)
SELECT 1, PermissionId
FROM Permission
WHERE PermissionCode IN
(
    'DASHBOARD_VIEW',

    'CLIENT_VIEW',
    'CLIENT_CREATE',
    'CLIENT_UPDATE',

    'STORE_VIEW',
    'STORE_CREATE',
    'STORE_UPDATE',

    'USER_VIEW',
    'USER_CREATE',
    'USER_UPDATE',

    'ROLE_VIEW',
    'ROLE_CREATE',
    'ROLE_UPDATE',

    'MENU_VIEW',
    'MENU_CREATE',
    'MENU_UPDATE'
);

INSERT INTO RolePermission (RoleId, PermissionId)
SELECT 2, PermissionId
FROM Permission
WHERE PermissionCode IN
(
    'DASHBOARD_VIEW',
    'STORE_VIEW',
    'STORE_CREATE',
    'USER_VIEW',
    'USER_CREATE',
    'PRODUCT_VIEW',
    'PRODUCT_CREATE',
    'STOCK_VIEW',
    'PURCHASE_VIEW',
    'SALES_VIEW',
    'CUSTOMER_VIEW',
    'REPORT_VIEW',
    'SETTING_VIEW',
    'CLIENT_EDIT',
    'CLIENT_VIEW'
);
