--select * from StoreUserMap 
CREATE OR ALTER PROCEDURE dbo.Sp_AddStoreUserMap
(
    @ClientId           BIGINT,
    @StoreId            BIGINT,
    @UserId             BIGINT ,
    @IsDefaultStore     BIT=0,
    @ErrNumber          INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    
    BEGIN TRY
        INSERT INTO StoreUserMap 
        (
            ClientId,
            StoreId,
            UserId,
            IsDefaultStore,
            IsActive,
            CreatedAt
        )
        Values
        (
            @ClientId,
            @StoreId,
            @UserId,
            @IsDefaultStore,
            1,
            SYSUTCDATETIME()
        )

        /* =====================================================
           12. SUCCESS RESPONSE
           ===================================================== */

        SET @ErrNumber = 0;


        SELECT
            @ErrNumber AS ErrNumber;

    END TRY

    BEGIN CATCH

        /* =====================================================
           ROLLBACK
           ===================================================== */

        SET @ErrNumber = ERROR_NUMBER();


        SELECT
            @ErrNumber AS ErrNumber;

    END CATCH
END;
