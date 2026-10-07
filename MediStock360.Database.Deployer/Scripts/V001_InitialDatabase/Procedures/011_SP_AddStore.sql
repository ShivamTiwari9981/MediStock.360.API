--select * from Store 

CREATE OR ALTER PROCEDURE dbo.SP_AddStore
(
    @ClientId           BIGINT,
    @StoreName          NVARCHAR(200),
    @StoreType          INT ,
    @StoreEmail         NVARCHAR(200),
    @PhoneNumber        NVARCHAR(20),
    @AlternatePhone     NVARCHAR(20),
    @DrugLicenseNumber  NVARCHAR(30),
    @AddressLine1       NVARCHAR(200),
    @AddressLine2       NVARCHAR(200),
    @CountryId          INT,
    @StateId            INT,
    @CityId             INT,
    @PostalCode         NVARCHAR(20),
    @Latitude           INT = NULL,
    @Longitude          INT = NULL,
    @IsOnBording        Bit = 0,
    @UserId             BIGINT,
    @CreatedBy          BIGINT = NULL,
    @ErrNumber          INT OUTPUT,
    @ErrMsg             VARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @Code NVARCHAR(100);
    DECLARE @StoreId BIGINT;
    DECLARE @Err int;

    BEGIN TRY

        BEGIN TRANSACTION;

        /* =====================================================
           1. VALIDATE COMPANY
           ===================================================== */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.Store
            WHERE ClientId=@ClientId AND StoreName = @StoreName
        )
        BEGIN
            THROW 50001, 'Store Name already exists.', 1;
        END


        /* =====================================================
           3. VALIDATE EMAIL
           ===================================================== */

        IF EXISTS
        (
            SELECT 1
            FROM dbo.Store
            WHERE ClientId=@ClientId AND  StoreEmail = @StoreEmail
        )
        BEGIN
            THROW 50002, 'Store Email already exists.', 1;
        END;


        /* =====================================================
           4. GET Store CODE
           ===================================================== */

        EXEC dbo.sp_GenerateMasterCode
            @ClientId = @ClientId,
            @StoreId = NULL,
            @CodeType = 'Store',
            @CodePrefix = 'STO',
            @NumberLength = 3,
            @GeneratedCode = @Code OUTPUT;


        /* =====================================================
           5. INSERT CLIENT
           ===================================================== */
        INSERT INTO dbo.Store
        (
            ClientId,
            StoreCode,
            StoreName,
            StoreType,
            StoreEmail,
            PhoneNumber,
            AlternatePhoneNumber,
            DrugLicenseNumber,
            AddressLine1,
            AddressLine2,
            CountryId,
            StateId,
            CityId,
            PostalCode,
            Latitude,
            Longitude,
            IsActive,
            CreatedBy,
            CreatedAt
        )
        VALUES
        (
            @ClientId,
            @Code,
            @StoreName,
            @StoreType,
            @StoreEmail,
            @PhoneNumber,
            @AlternatePhone,
            @DrugLicenseNumber,
            @AddressLine1,
            @AddressLine2,
            @CountryId,
            @StateId,
            @CityId,
            @PostalCode,
            @Latitude,
            @Longitude,
            1,
            @CreatedBy,
            SYSUTCDATETIME()
        );

        SET @StoreId = CONVERT(BIGINT, SCOPE_IDENTITY());


        IF @StoreId IS NULL OR @StoreId <= 0
        BEGIN
            THROW 50003, 'Store creation failed.', 1;
        END;

        IF @IsOnBording =1
            BEGIN
                EXEC dbo.Sp_AddStoreUserMap
                @ClientId = @ClientId,
                @StoreId = @StoreId,
                @UserId = @UserId,
                @IsDefaultStore = 1,
                @ErrNumber = @Err OUTPUT
            END
            

            IF @Err <> 0
               BEGIN
                   THROW 50004, 'User Site Map Failed.', 1;
               END;

            Update Client SET OnboardingStep = 3 
            WHERE ClientId = @ClientId
        
       
        COMMIT TRANSACTION;


        /* =====================================================
           12. SUCCESS RESPONSE
           ===================================================== */

        SET @ErrNumber = 0;
        SET @ErrMsg = 'Store has been created successfully.';

        SELECT
            S.StoreId,
            s.StoreKey,
            s.ClientId,
            s.StoreName,
            s.IsActive,
            @ErrNumber AS ErrNumber,
            @ErrMsg AS ErrMsg
        FROM Store s
        INNER JOIN StoreUserMap sum
            ON sum.StoreId = s.StoreId
        WHERE sum.UserId = @UserId
          AND sum.IsActive = 1
          AND s.ClientId = @ClientId
          AND s.IsActive = 1
        ORDER BY s.StoreName
            

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
        SET @ErrMsg = ERROR_MESSAGE() + 'WITH PROCEDURE NAME' + ERROR_PROCEDURE();


        SELECT
            @ErrNumber AS ErrNumber,
            @ErrMsg AS ErrMsg;

    END CATCH
END;
