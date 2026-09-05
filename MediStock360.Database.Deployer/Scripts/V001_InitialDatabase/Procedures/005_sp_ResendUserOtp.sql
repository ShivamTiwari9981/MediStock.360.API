CREATE OR ALTER PROCEDURE dbo.Sp_ResendUserOtp
(
    @ClientId BIGINT = NULL,
    @UserId BIGINT,
    @OtpType Int,
    @OtpHash NVARCHAR(500),
    @ExpiryMinutes INT = 10
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        -- Invalidate previous active OTPs
        UPDATE dbo.UserOtp
        SET IsUsed = 1
        WHERE  ClientId  IS NULL OR ClientId = @ClientId AND UserId = @UserId
          AND IsUsed = 0


        -- Insert new OTP
        INSERT INTO dbo.UserOtp
        (
            ClientId,
            UserId,
            OtpType,
            OtpHash,
            ExpiresAt,
            IsUsed,
            CreatedAt
        )
        VALUES
        (
            @ClientId,
            @UserId,
            @OtpType,
            @OtpHash,
            DATEADD(MINUTE, @ExpiryMinutes, SYSUTCDATETIME()),
            0,
            SYSUTCDATETIME()
        );

        DECLARE @UserOtpId BIGINT = SCOPE_IDENTITY();

        COMMIT TRANSACTION;

        SELECT
            CAST(1 AS BIT) AS IsSuccess,
            'OTP resent successfully.' AS Message,
            @UserOtpId AS UserOtpId;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;