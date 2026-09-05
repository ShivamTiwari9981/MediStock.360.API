--drop proc sp_PlateformRegister
CREATE OR ALTER PROCEDURE dbo.sp_PlateformRegister
(
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
    DECLARE @UserId BIGINT;
    DECLARE @RoleId BIGINT;

    BEGIN TRY

        BEGIN TRANSACTION;
        /* =====================================================
           1. VALIDATE EMAIL
           ===================================================== */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.[User]
            WHERE Email = @Email
        )
        BEGIN
            THROW 50003, 'User Email already exists.', 1
        END;
        INSERT INTO dbo.[User]
        (
            UserName,
            Email,
            PasswordHash,
            UserSalt,
            CreatedBy
        )
        VALUES
        (
            @UserName,
            @Email,
            @HashPassword,
            @UserSalt,
            @CreatedBy
        );


        /* =====================================================
           2. GET NEW USER ID

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
           9. GET CLIENT OWNER ROLE
           ===================================================== */
        SELECT TOP 1
            @RoleId = RoleId
        FROM dbo.[Role]
        WHERE RoleCode = 'PRODUCT_OWNER'
          AND IsActive = 1;


        IF @RoleId IS NULL
        BEGIN
            THROW 50006, 'PRODUCT_OWNER role not found.', 1;
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
        SET @ErrMsg = 'User has been created successfully.';


        SELECT
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
