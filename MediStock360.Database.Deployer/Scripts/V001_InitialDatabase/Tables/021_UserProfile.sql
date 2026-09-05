IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'UserProfile'
      AND schema_id = SCHEMA_ID('dbo')
)

CREATE TABLE Dbo.UserProfile
(
    UserProfileId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ClientId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NULL,
    DisplayName NVARCHAR(200) NULL,
    PhoneNumber NVARCHAR(20) NOT NULL Unique,
    AlternatePhoneNumber NVARCHAR(20) NULL,
    ProfileImageUrl NVARCHAR(500) NULL,
    DateOfBirth DATE NULL,
    Gender NVARCHAR(20) NULL,
    AddressLine1 NVARCHAR(250) NULL,
    AddressLine2 NVARCHAR(250) NULL,
    CityId BIGINT NULL,
    StateId BIGINT NULL,
    CountryId BIGINT NULL,
    PostalCode NVARCHAR(20) NULL,
    IsPhoneVerified BIT NOT NULL DEFAULT (0),
    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate DATETIME2 NULL,

    CONSTRAINT FK_UserProfile_User
        FOREIGN KEY (UserId) REFERENCES [User](UserId),

    CONSTRAINT UQ_UserProfile_User
        UNIQUE (UserId)
);
