/* =========================================================
   Seed_Role.sql
   Purpose: Insert default system roles
   ========================================================= */
--IF NOT EXISTS (
--    SELECT 1
--    FROM dbo.[Role]
--    WHERE RoleCode = 'PRODUCT_OWNER'
--)
--BEGIN
--    INSERT INTO dbo.[Role]
--    (
--        RoleCode,
--        RoleName,
--        Description,
--        IsSystemRole,
--        IsActive
--    )
--    VALUES
--    (
--        'PRODUCT_OWNER',
--        'Product Owner',
--        'Product owner of the product account',
--        1,
--        1
--    );
--END;
