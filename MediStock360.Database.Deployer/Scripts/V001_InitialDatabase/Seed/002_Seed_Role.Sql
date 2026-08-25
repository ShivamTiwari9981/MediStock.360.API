/* =========================================================
   Seed_Role.sql
   Purpose: Insert default system roles
   ========================================================= */

IF NOT EXISTS (
    SELECT 1
    FROM dbo.[Role]
    WHERE RoleCode = 'CLIENT_OWNER'
)
BEGIN
    INSERT INTO dbo.[Role]
    (
        RoleCode,
        RoleName,
        Description,
        IsSystemRole,
        IsActive
    )
    VALUES
    (
        'CLIENT_OWNER',
        'Client Owner',
        'Primary owner of the client account',
        1,
        1
    );
END;


IF NOT EXISTS (
    SELECT 1
    FROM dbo.[Role]
    WHERE RoleCode = 'CLIENT_ADMIN'
)
BEGIN
    INSERT INTO dbo.[Role]
    (
        RoleCode,
        RoleName,
        Description,
        IsSystemRole,
        IsActive
    )
    VALUES
    (
        'CLIENT_ADMIN',
        'Client Admin',
        'Administrator of the client account',
        1,
        1
    );
END;


IF NOT EXISTS (
    SELECT 1
    FROM dbo.[Role]
    WHERE RoleCode = 'STORE_MANAGER'
)
BEGIN
    INSERT INTO dbo.[Role]
    (
        RoleCode,
        RoleName,
        Description,
        IsSystemRole,
        IsActive
    )
    VALUES
    (
        'STORE_MANAGER',
        'Store Manager',
        'Manages store operations',
        1,
        1
    );
END;


IF NOT EXISTS (
    SELECT 1
    FROM dbo.[Role]
    WHERE RoleCode = 'PHARMACIST'
)
BEGIN
    INSERT INTO dbo.[Role]
    (
        RoleCode,
        RoleName,
        Description,
        IsSystemRole,
        IsActive
    )
    VALUES
    (
        'PHARMACIST',
        'Pharmacist',
        'Handles pharmacy operations',
        1,
        1
    );
END;


IF NOT EXISTS (
    SELECT 1
    FROM dbo.[Role]
    WHERE RoleCode = 'STAFF'
)
BEGIN
    INSERT INTO dbo.[Role]
    (
        RoleCode,
        RoleName,
        Description,
        IsSystemRole,
        IsActive
    )
    VALUES
    (
        'STAFF',
        'Staff',
        'General store staff',
        1,
        1
    );
END;