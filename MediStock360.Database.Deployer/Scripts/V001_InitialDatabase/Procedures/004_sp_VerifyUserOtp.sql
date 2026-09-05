CREATE OR ALTER PROCEDURE dbo.Sp_VerifyUserOtp
(
    @ClientId BIGINT = NULL,
    @UserId BIGINT,
    @OtpHash NVARCHAR(500)
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE @UserOtpId BIGINT;

        -- Get latest valid OTP
        SELECT TOP 1
            @UserOtpId = UserOtpId
        FROM dbo.UserOtp WITH (UPDLOCK, ROWLOCK)
        WHERE ClientId  IS NULL OR ClientId = @ClientId
          AND UserId = @UserId
          AND OtpHash = @OtpHash
          AND IsUsed = 0
          AND ExpiresAt > GETUTCDATE()
        ORDER BY CreatedAt DESC;

        -- OTP not found / expired / already used
        IF @UserOtpId IS NULL
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                CAST(0 AS BIT) AS IsVerified,
                'Invalid or expired OTP.' AS Message;

            RETURN;
        END;

        -- Mark OTP as used
        UPDATE dbo.UserOtp
        SET
            IsUsed = 1,
            VerifiedAt = GETUTCDATE()
        WHERE ClientId  IS NULL OR ClientId = @ClientId AND UserId =@UserId AND UserOtpId = @UserOtpId
          AND IsUsed = 0;
          
        Update dbo.[User] 
        Set IsActive =1, IsEmailVerified =1 
        WHERE ClientId  IS NULL OR ClientId = @ClientId AND UserId =@UserId


        -- Verify update succeeded
        IF @@ROWCOUNT = 0
        BEGIN
            ROLLBACK TRANSACTION;

            SELECT
                CAST(0 AS BIT) AS IsVerified,
                'OTP has already been used.' AS Message;

            RETURN;
        END;

        COMMIT TRANSACTION;

        SELECT
            CAST(1 AS BIT) AS IsVerified,
            'OTP verified successfully.' AS Message,
            @UserOtpId AS UserOtpId;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;
