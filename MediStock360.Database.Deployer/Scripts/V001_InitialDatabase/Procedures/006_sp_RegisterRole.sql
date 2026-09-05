--select * from [User]
----select * from [Role]
CREATE OR ALTER PROCEDURE dbo.sp_RegisterRole
(
    @ClientId    BIGINT,
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
            FROM dbo.[Role]
            WHERE ClientId = @ClientId AND RoleCode = 'CLIENT_OWNER'
        )
        BEGIN
            INSERT INTO dbo.[Role]
            (
                ClientId,
                RoleCode,
                RoleName,
                Description,
                IsSystemRole,
                IsActive,
                CreatedAt,
                CreatedBy
            )
            VALUES
            (   @ClientId,
                'CLIENT_OWNER',
                'Client Owner',
                'Primary owner of the client account',
                1,
                1,
                SYSUTCDATETIME(),
                @CreatedBy
            );
        END;


        IF NOT EXISTS (
            SELECT 1
            FROM dbo.[Role]
            WHERE ClientId = @ClientId AND RoleCode = 'CLIENT_ADMIN'
        )
        BEGIN
            INSERT INTO dbo.[Role]
            (   ClientId,
                RoleCode,
                RoleName,
                Description,
                IsSystemRole,
                IsActive,
                CreatedAt,
                CreatedBy
            )
            VALUES
            (    @ClientId,
                'CLIENT_ADMIN',
                'Client Admin',
                'Administrator of the client account',
                1,
                1,
                SYSUTCDATETIME(),
                @CreatedBy
            );
        END;


        IF NOT EXISTS (
            SELECT 1
            FROM dbo.[Role]
            WHERE ClientId = @ClientId AND RoleCode = 'STORE_MANAGER'
        )
        BEGIN
            INSERT INTO dbo.[Role]
            (   ClientId,
                RoleCode,
                RoleName,
                Description,
                IsSystemRole,
                IsActive,
                CreatedAt,
                CreatedBy
            )
            VALUES
            (   @ClientId,
                'STORE_MANAGER',
                'Store Manager',
                'Manages store operations',
                1,
                 1,
                SYSUTCDATETIME(),
                @CreatedBy
            );
        END;


        IF NOT EXISTS (
            SELECT 1
            FROM dbo.[Role]
            WHERE ClientId = @ClientId AND RoleCode = 'PHARMACIST'
        )
        BEGIN
            INSERT INTO dbo.[Role]
            (   ClientId,
                RoleCode,
                RoleName,
                Description,
                IsSystemRole,
                IsActive,
                CreatedAt,
                CreatedBy
            )
            VALUES
            (    @ClientId,
                'PHARMACIST',
                'Pharmacist',
                'Handles pharmacy operations',
                1,
                1,
                SYSUTCDATETIME(),
                @CreatedBy
            );
        END;


        IF NOT EXISTS (
            SELECT 1
            FROM dbo.[Role]
            WHERE ClientId = @ClientId AND RoleCode = 'STAFF'
        )
        BEGIN
            INSERT INTO dbo.[Role]
            (
                ClientId,
                RoleCode,
                RoleName,
                Description,
                IsSystemRole,
                IsActive,
                CreatedAt,
                CreatedBy
            )
            VALUES
            (
                @ClientId,
                'STAFF',
                'Staff',
                'General store staff',
                1,
                 1,
                SYSUTCDATETIME(),
                @CreatedBy
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
