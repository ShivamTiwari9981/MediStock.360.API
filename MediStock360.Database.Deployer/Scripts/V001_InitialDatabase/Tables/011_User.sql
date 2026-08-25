IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = '[User]'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
CREATE TABLE dbo.[User]
(
    UserId BIGINT IDENTITY(1,1) NOT NULL,
    ClientId BIGINT NOT NULL,
    UserKey UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWSEQUENTIALID()),
    EmployeeId BIGINT  NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NULL,
    Email NVARCHAR(150) NOT NULL Unique,
    PhoneNumber NVARCHAR(20) NOT NULL Unique,
    UserName NVARCHAR(100) NOT NULL Unique,
    PasswordHash NVARCHAR(500) NOT NULL,
    UserSalt NVARCHAR(max) NOT NULL,
    IsEmailVerified BIT NOT NULL DEFAULT (0),
    IsPhoneVerified BIT NOT NULL DEFAULT (0),
    IsPrimary BIT NOT NULL DEFAULT (0),
    IsActive BIT NOT NULL DEFAULT (0),
    LastLoginAt DATETIME2 NULL,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_User
        PRIMARY KEY (UserId),

    CONSTRAINT FK_User_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT UQ_User_UserKey
        UNIQUE (UserKey),

    CONSTRAINT UQ_User_Client_Email
        UNIQUE (ClientId, Email),

    CONSTRAINT UQ_User_Client_UserName
        UNIQUE (ClientId, UserName)
);
END

