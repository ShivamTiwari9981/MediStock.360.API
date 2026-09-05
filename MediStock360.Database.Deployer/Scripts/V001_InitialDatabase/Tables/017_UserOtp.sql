IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'UserOtp'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE UserOtp
(
    UserOtpId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ClientId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,
    -- 1 = Email, 2 = Phone
    OtpType TINYINT NOT NULL,
    OtpHash NVARCHAR(500) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    AttemptCount INT NOT NULL DEFAULT (0),
    IsUsed BIT NOT NULL DEFAULT (0),
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    VerifiedAt DATETIME2 NULL,

    CONSTRAINT FK_UserOtp_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT CK_UserOtp_OtpType
        CHECK (OtpType IN (1, 2))
);

CREATE INDEX IX_UserOtp_UserId_OtpType
ON UserOtp(UserId, OtpType);

CREATE INDEX IX_UserOtp_Active
ON UserOtp(UserId, OtpType, IsUsed, ExpiresAt);
