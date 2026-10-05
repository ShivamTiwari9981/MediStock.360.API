--select * from Client

CREATE OR ALTER PROCEDURE dbo.Sp_UpdateClient
(
    @ClientId           BIGINT,
    @ClientName         NVARCHAR(150),
    @CompanyName        NVARCHAR(150),
    @OwnerName          NVARCHAR(200),
    @BusinessTypeId     INT,
    @Email              NVARCHAR(200),
    @Phone              NVARCHAR(20),
    @GSTNumber          NVARCHAR(100),
    @DrugLicenseNumber  NVARCHAR(100),
    @Address            NVARCHAR(200),
    @CityId             INT,
    @StateId            INT,
    @CountryId          INT,
    @PostalCode         NVARCHAR(10),
    @UpdatedBy          BIGINT = NULL,
    @ErrNumber          INT OUTPUT,
    @ErrMsg             VARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        UPDATE CLIENT SET
        ClientName = @ClientName,
        CompanyName = @CompanyName,
        OwnerName = @OwnerName,
        BusinessTypeId = @BusinessTypeId,
        Email = @Email,
        Phone = @Phone,
        GSTNumber = @GSTNumber,
        DrugLicenseNumber = @DrugLicenseNumber,
        CityId = @CityId,
        StateId = @StateId,
        CountryId = @CountryId,
        PostalCode = @PostalCode,
        OnboardingStep = 2,
        UpdatedBy = @UpdatedBy,
        UpdatedAt = SYSUTCDATETIME()
        WHERE ClientId = @ClientId

        SET @ErrNumber = 0;
        SET @ErrMsg = 'Client has been updated successfully.';

        SELECT TOP 1 
        ClientId,
        ClientCode,
        ClientKey,
        ClientName,
        CompanyName, 
        OwnerName, 
        BusinessTypeId,
        Email ,
        Phone ,
        GSTNumber ,
        DrugLicenseNumber ,
        CityId ,
        StateId ,
        CountryId,
        PostalCode,
        OnboardingStep,
        IsOnboardingCompleted,
        @ErrNumber AS ErrNumber,
        @ErrMsg AS ErrMsg
        FROM CLIENT 
        WHERE ClientId = @ClientId

        

    END TRY

    BEGIN CATCH
        SET @ErrNumber = ERROR_NUMBER();
        SET @ErrMsg = ERROR_MESSAGE();


        SELECT
            @ErrNumber AS ErrNumber,
            @ErrMsg AS ErrMsg;

    END CATCH
END;
