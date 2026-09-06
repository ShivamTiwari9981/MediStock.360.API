--select * from [User]
----select * from [Role]
--select * from RolePermission
--select * from Permission

CREATE OR ALTER PROCEDURE dbo.sp_RolePermission
(
    @RoleId INT,
    @CreatedBy      BIGINT = NULL,
    @ErrNumber      INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        /* =====================================================
           1. VALIDATE ROLE
           ===================================================== */

        IF NOT EXISTS (
            SELECT 1
            FROM dbo.RolePermission
            WHERE RoleId = @RoleId 
        )
        BEGIN
            INSERT INTO RolePermission (RoleId, PermissionId)
SELECT @RoleId, PermissionId
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
        END;
        /* =====================================================
           12. SUCCESS RESPONSE
           ===================================================== */

        SET @ErrNumber = 0;


        SELECT
            @ErrNumber AS ErrNumber

    END TRY

    BEGIN CATCH

        /* =====================================================
           ROLLBACK
           ===================================================== */
        SET @ErrNumber = ERROR_NUMBER();


        SELECT
            @ErrNumber AS ErrNumber

    END CATCH
END;
