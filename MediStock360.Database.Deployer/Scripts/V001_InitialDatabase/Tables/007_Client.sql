IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'Client'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
    CREATE TABLE dbo.Client (   
        ClientId BIGINT PRIMARY KEY Identity(100,1),
        ClientKey UNIQUEIDENTIFIER NOT NULL UNIQUE DEFAULT NEWID(),
        ClientCode NVARCHAR(50) Unique NOT NULL,
        ClientName NVARCHAR(150) NULL,
        CompanyName NVARCHAR(150) NOT NULL UNIQUE,
        OwnerName NVARCHAR(150) NULL,
        BusinessTypeId int  NULL,
        Email NVARCHAR(100) NULL,
        Phone NVARCHAR(20)  NULL,
        GSTNumber NVARCHAR(50) NULL,
        DrugLicenseNumber NVARCHAR(150) NULL,
        [Address] TEXT,
        CityId Int NULL ,
        [StateId] Int NULL,
        [CountryId] Int NULL,
        [PostalCode] NVARCHAR(10) Null,
        IsOnboardingCompleted BIT NOT NULL DEFAULT 0,
        OnboardingStep INT NOT NULL DEFAULT 1,
        IsActive bit default 0,
        IsSynced bit default 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        CreatedBy BIGINT NULL,
        UpdatedAt DATETIME2 NULL,
        UpdatedBy BIGINT NULL,
        FOREIGN KEY (BusinessTypeId) REFERENCES BusinessType(BusinessTypeId),

        CONSTRAINT FK_Client_Country FOREIGN KEY (CountryId)-- FK to Country
            REFERENCES Country(CountryId),

        CONSTRAINT FK_Client_City FOREIGN KEY (CityId)-- FK to Country
            REFERENCES City(CityId),

        CONSTRAINT FK_Client_State FOREIGN KEY (StateId)-- FK to Country
            REFERENCES [State](StateId)

    );
END