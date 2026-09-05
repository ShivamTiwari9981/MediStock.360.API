--select * from Client
--select * from Store
--select * from StoreUserMap
--select * from [User]

--EXEC [Sp_Login] '100',10
CREATE OR ALTER PROCEDURE [dbo].[Sp_Login]
(
    @ClientId BIGINT = NULL,
    @UserId BIGINT 
)
AS
BEGIN
    SET NOCOUNT ON;
    
    --------------------------------------------------
    -- 1. GET USER
    --------------------------------------------------
    SELECT TOP 1
        u.ClientId,
        u.UserId, 
        UserKey,
        Email,
        UserName,
        IsEmailVerified,
        LastLoginAt,
        UP.ProfileImageUrl,
        IsLocked,
        IsActive
    FROM [User] u left join [UserProfile] UP
    on u.ClientId = UP.ClientId
    and u.UserId = UP.UserId
    WHERE u.ClientId = @ClientId
       OR u.UserId = @UserId;

    IF @UserId IS NULL
    BEGIN
        THROW 50001, 'Invalid username/email or password.', 1;
    END;

    --update Role set ClientId = 100 where RoleCode <> 'PRODUCT_OWNER'
    --------------------------------------------------
    -- 2. USER ROLES
    --------------------------------------------------
    SELECT DISTINCT
        r.RoleId,
        r.ClientId,
        r.RoleCode,
        r.RoleName,
        r.IsSystemRole,
        r.IsActive
    FROM UserRole ur
    INNER JOIN Role r
        ON r.RoleId = ur.RoleId
    WHERE ur.UserId = @UserId
    and r.ClientId = @ClientId
      AND r.IsActive = 1;


    --------------------------------------------------
    -- 4. USER PERMISSIONS
    --------------------------------------------------
    SELECT DISTINCT
    p.PermissionId,
    p.PermissionCode,
    p.PermissionName,
    rp.IsActive
    FROM UserRole ur
    INNER JOIN [Role] r
        ON r.RoleId = ur.RoleId
    INNER JOIN RolePermission rp
        ON rp.RoleId = r.RoleId
    INNER JOIN Permission p
        ON p.PermissionId = rp.PermissionId
    WHERE ur.UserId = @UserId
      AND r.ClientId = @ClientId
      AND r.IsActive = 1
      AND p.IsActive = 1
      AND rp.IsActive =1;


    --------------------------------------------------
    -- 5. CLIENT
    --------------------------------------------------
    SELECT
        c.ClientId,
        c.ClientKey,
        c.ClientName,
        c.CompanyName,
        c.Email,
        c.IsOnboardingCompleted,
        c.OnboardingStep,
        c.IsActive
    FROM Client c
    WHERE c.ClientId = @ClientId;


    --------------------------------------------------
    -- 6. STORES
    --
    -- Client Owner/Admin:
    --     All active stores
    --
    -- Regular User:
    --     Only mapped stores
    --------------------------------------------------

    IF EXISTS
    (
        SELECT 1
        FROM UserRole ur
        INNER JOIN [Role] r
            ON r.RoleId = ur.RoleId
        WHERE ur.UserId = @UserId
          AND r.IsActive = 1
          AND UPPER(r.RoleCode) IN
              ('CLIENT_OWNER', 'CLIENT_ADMIN')
    )
    BEGIN

        -- Client Owner / Client Admin
        SELECT
            S.StoreId,
            s.StoreKey,
            s.ClientId,
            s.StoreName,
            s.IsActive
        FROM Store s
        WHERE s.ClientId = @ClientId
          AND s.IsActive = 1
        ORDER BY s.StoreName;

    END
    ELSE
    BEGIN

        -- Regular User
        SELECT
            S.StoreId,
            s.StoreKey,
            s.ClientId,
            s.StoreName,
            s.IsActive
        FROM Store s
        INNER JOIN StoreUserMap sum
            ON sum.StoreId = s.StoreId
        WHERE sum.UserId = @UserId
          AND sum.IsActive = 1
          AND s.ClientId = @ClientId
          AND s.IsActive = 1
        ORDER BY s.StoreName;

    END;

    SELECT DISTINCT
    m.MenuId,
    m.MenuName,
    m.ParentMenuId,
    m.PermissionCode, 
    m.RouterLink,
    m.MenuIcon,
    m.DisplayOrder,
    m.IsVisible
FROM UserRole ur
INNER JOIN [Role] r
    ON r.RoleId = ur.RoleId
INNER JOIN RolePermission rp
    ON rp.RoleId = r.RoleId
INNER JOIN Permission p
    ON p.PermissionId = rp.PermissionId
INNER JOIN Menu m
    ON m.PermissionCode = p.PermissionCode
WHERE ur.UserId = 10
  AND r.ClientId = 100
  AND r.IsActive = 1
  AND p.IsActive = 1
  AND m.IsActive = 1
ORDER BY m.DisplayOrder;


    --------------------------------------------------
    -- 7. UPDATE LAST LOGIN
    --------------------------------------------------
    UPDATE [User]
    SET LastLoginAt = GETUTCDATE()
    WHERE UserId = @UserId;

END


