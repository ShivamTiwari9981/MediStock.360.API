--drop proc sp_PlateformRegister
CREATE OR ALTER PROCEDURE dbo.sp_Intial_InsertGenerateMaster
(
    @ClientId BIGINT,
    @StoreId BIGINT,
    @CreatedBy      BIGINT = NULL,
    @ErrNumber      INT OUTPUT,
    @ErrMsg         VARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

   

    BEGIN TRY
        /* ============================================================
   Seed : MasterCodeGeneration
   Purpose : Initial code generation configuration
   ============================================================ */


        IF NOT EXISTS
        (
            SELECT 1
            FROM MasterCodeGeneration
            WHERE ClientId =@ClientId
              AND StoreId= @StoreId
              AND CodeType = 'Client'
        )
        BEGIN
            INSERT INTO MasterCodeGeneration(ClientId,StoreId,CodeType,CodePrefix,CurrentNumber,NumberLength,IsActive)
            VALUES(@ClientId,@StoreId,'Client','CLI',0,3,1);
        END;

        IF NOT EXISTS
        (
            SELECT 1
            FROM MasterCodeGeneration
            WHERE ClientId = @ClientId
              AND StoreId = @StoreId
              AND CodeType = 'Role'
        )
        BEGIN
            INSERT INTO MasterCodeGeneration
            (
                ClientId,
                StoreId,
                CodeType,
                CodePrefix,
                CurrentNumber,
                NumberLength,
                IsActive
            )
            VALUES
            (
                @ClientId,
                @StoreId,
                'Role',
                'ROL',
                0,
                3,
                1
            );
        END;

        SET @ErrNumber = 0;
        SET @ErrMsg = 'User has been created successfully.';


        SELECT
            @ErrNumber AS ErrNumber,
            @ErrMsg AS ErrMsg;

    END TRY

    BEGIN CATCH

        /* =====================================================
           ROLLBACK
           ===================================================== */
        SET @ErrNumber = ERROR_NUMBER();
        SET @ErrMsg = ERROR_MESSAGE();


        SELECT
            @ErrNumber AS ErrNumber,
            @ErrMsg AS ErrMsg;

    END CATCH
END;
