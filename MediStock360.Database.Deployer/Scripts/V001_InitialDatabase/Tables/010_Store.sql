IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'Store'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE dbo.Store
(
    StoreId BIGINT IDENTITY(1,1) NOT NULL,
    ClientId BIGINT NOT NULL,
    StoreKey UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWSEQUENTIALID()),
    StoreCode NVARCHAR(50) NOT NULL,
    StoreName NVARCHAR(200) NOT NULL,
    StoreType TINYINT NOT NULL DEFAULT (1),
    -- 1 = Medical Store
    -- 2 = Pharmacy
    -- 3 = Hospital Pharmacy
    -- 4 = Warehouse
    -- 5 = Distributor
    OwnerName NVARCHAR(150) NULL,
    Email NVARCHAR(150) NULL,
    PhoneNumber NVARCHAR(20) NULL,
    AlternatePhoneNumber NVARCHAR(20) NULL,
    GSTNumber NVARCHAR(50) NULL,
    DrugLicenseNumber NVARCHAR(100) NULL,
    AddressLine1 NVARCHAR(250) NULL,
    AddressLine2 NVARCHAR(250) NULL,
    CityId INT NULL,
    PostalCode NVARCHAR(20) NULL,
    Latitude DECIMAL(10,7) NULL,
    Longitude DECIMAL(10,7) NULL,
    IsActive BIT NOT NULL DEFAULT (1),
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_Store
        PRIMARY KEY (StoreId),

    CONSTRAINT FK_Store_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT FK_Store_City
        FOREIGN KEY (CityId)
        REFERENCES City(CityId),

    CONSTRAINT UQ_Store_StoreKey
        UNIQUE (StoreKey),

    CONSTRAINT UQ_Store_Client_StoreCode
        UNIQUE (ClientId, StoreCode),

    CONSTRAINT CK_Store_StoreType
        CHECK (StoreType IN (1, 2, 3, 4, 5))
);

