CREATE OR ALTER PROCEDURE dbo.sp_RegisterClientUser
(
    @CompanyName    NVARCHAR(150),
    @UserName      NVARCHAR(200),
    @Email          NVARCHAR(200),
    @HashPassword   NVARCHAR(MAX),
    @UserSalt       NVARCHAR(MAX),
    @CreatedBy      BIGINT = NULL,
    @ErrNumber      INT OUTPUT,
    @ErrMsg         VARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Code NVARCHAR(100);
    DECLARE @RoleStatus INT;
    DECLARE @ClientId BIGINT;
    DECLARE @UserId BIGINT;
    DECLARE @RoleId BIGINT;

    BEGIN TRY

        BEGIN TRANSACTION;

        /* =====================================================
           1. VALIDATE COMPANY
           ===================================================== */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.Client
            WHERE CompanyName = @CompanyName
        )
        BEGIN
            THROW 50001, 'Company Name already exists.', 1;
        END


        /* =====================================================
           3. VALIDATE EMAIL
           ===================================================== */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.[User]
            WHERE Email = @Email
        )
        BEGIN
            THROW 50003, 'User Email already exists.', 1;
        END;


        /* =====================================================
           4. GET CLIENT CODE
           ===================================================== */

        EXEC dbo.sp_GenerateMasterCode
            @ClientId = NULL,
            @StoreId = NULL,
            @CodeType = 'Client',
            @CodePrefix = 'CLI',
            @NumberLength = 3,
            @GeneratedCode = @Code OUTPUT;


        /* =====================================================
           5. INSERT CLIENT
           ===================================================== */

        INSERT INTO dbo.Client
        (
            ClientCode,
            ClientName,
            CompanyName,
            CreatedBy,
            CreatedAt
        )
        VALUES
        (
            @Code,
            @CompanyName,
            @CompanyName,
            @CreatedBy,
            SYSUTCDATETIME()
        );


        /* =====================================================
           6. GET NEW CLIENT ID
           ===================================================== */

        SET @ClientId = CONVERT(BIGINT, SCOPE_IDENTITY());


        IF @ClientId IS NULL OR @ClientId <= 0
        BEGIN
            THROW 50004, 'Client creation failed.', 1;
        END;


        /* =====================================================
           7. INSERT USER
           ===================================================== */

        INSERT INTO dbo.[User]
        (
            ClientId,
            UserName,
            Email,
            PasswordHash,
            UserSalt,
            CreatedBy
        )
        VALUES
        (
            @ClientId,
            @UserName,
            @Email,
            @HashPassword,
            @UserSalt,
            @CreatedBy
        );


        /* =====================================================
           8. GET NEW USER ID

           IMPORTANT:
           Don't search User by ClientId.
           A client can have multiple users.
           ===================================================== */

        SET @UserId = CONVERT(BIGINT, SCOPE_IDENTITY());


        IF @UserId IS NULL OR @UserId <= 0
        BEGIN
            THROW 50005, 'User creation failed.', 1;
        END;

        /* =====================================================
           9. Insert Role
           ===================================================== */

        EXEC dbo.sp_RegisterRole
            @ClientId = @ClientId,
            @CreatedBy = @UserId,
            @ErrNumber = @RoleStatus OUTPUT;

        IF @RoleStatus <> 0
        BEGIN
            THROW 50006, 'Role creation failed.', 1;
        END;



        /* =====================================================
           9. GET CLIENT OWNER ROLE
           ===================================================== */

        SELECT TOP 1
            @RoleId = RoleId
        FROM dbo.[Role]
        WHERE RoleCode = 'CLIENT_OWNER'
          AND IsActive = 1;


        IF @RoleId IS NULL
        BEGIN
            THROW 50006, 'CLIENT_OWNER role not found.', 1;
        END;


        /* =====================================================
           10. INSERT USER ROLE
           ===================================================== */

        INSERT INTO dbo.UserRole
        (
            UserId,
            RoleId,
            CreatedBy
        )
        VALUES
        (
            @UserId,
            @RoleId,
            @CreatedBy
        );


        /* =====================================================
           11. COMMIT TRANSACTION
           ===================================================== */

        COMMIT TRANSACTION;


        /* =====================================================
           12. SUCCESS RESPONSE
           ===================================================== */

        SET @ErrNumber = 0;
        SET @ErrMsg = 'Client has been created successfully.';


        SELECT
            @ClientId AS ClientId,
            @Code AS ClientCode,
            @UserId AS UserId,
            @RoleId AS RoleId,
            @ErrNumber AS ErrNumber,
            @ErrMsg AS ErrMsg;

    END TRY

    BEGIN CATCH

        /* =====================================================
           ROLLBACK
           ===================================================== */

        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;


        SET @ErrNumber = ERROR_NUMBER();
        SET @ErrMsg = ERROR_MESSAGE();


        SELECT
            @ErrNumber AS ErrNumber,
            @ErrMsg AS ErrMsg;

    END CATCH
END;
