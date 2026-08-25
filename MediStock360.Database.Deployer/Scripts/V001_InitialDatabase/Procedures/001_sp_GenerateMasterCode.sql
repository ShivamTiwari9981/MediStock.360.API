--IF OBJECT_ID('dbo.sp_GenerateMasterCode', 'P') IS NOT NULL
--BEGIN
--    DROP PROCEDURE dbo.sp_GenerateMasterCode;
--END;


CREATE PROCEDURE sp_GenerateMasterCode
(
    @ClientId BIGINT = NULL,
    @StoreId BIGINT = NULL,
    @CodeType NVARCHAR(50),
    @CodePrefix NVARCHAR(20),
    @NumberLength INT = 3,
    @GeneratedCode NVARCHAR(100) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION;

    DECLARE @CurrentNumber BIGINT;

    -- Find existing code configuration
    SELECT @CurrentNumber = CurrentNumber
    FROM MasterCodeGeneration WITH (UPDLOCK, HOLDLOCK)
    WHERE
        ISNULL(ClientId, 0) = ISNULL(@ClientId, 0)
        AND ISNULL(StoreId, 0) = ISNULL(@StoreId, 0)
        AND CodeType = @CodeType
        AND IsActive = 1;

    -- If configuration doesn't exist, create it
    IF @CurrentNumber IS NULL
    BEGIN
        SET @CurrentNumber = 1;

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
            @CodeType,
            @CodePrefix,
            @CurrentNumber,
            @NumberLength,
            1
        );
    END
    ELSE
    BEGIN
        SET @CurrentNumber = @CurrentNumber + 1;

        UPDATE MasterCodeGeneration
        SET
            CurrentNumber = @CurrentNumber,
            ModifiedDate = SYSUTCDATETIME()
        WHERE
            ISNULL(ClientId, 0) = ISNULL(@ClientId, 0)
            AND ISNULL(StoreId, 0) = ISNULL(@StoreId, 0)
            AND CodeType = @CodeType
            AND IsActive = 1;
    END;

    SET @GeneratedCode =
        @CodePrefix +
        RIGHT(
            REPLICATE('0', @NumberLength) +
            CAST(@CurrentNumber AS NVARCHAR(20)),
            @NumberLength
        );

    COMMIT TRANSACTION;

    SELECT @GeneratedCode AS GeneratedCode;
END;