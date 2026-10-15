
CREATE TABLE dbo.PaymentConfiguration
(
    PaymentConfigurationId BIGINT IDENTITY(1,1) NOT NULL,
    PaymentMethodId INT NOT NULL,

    ConfigurationCode NVARCHAR(50) NOT NULL,
    ConfigurationName NVARCHAR(100) NOT NULL,

    -- UPI / QR configuration
    UpiId NVARCHAR(150) NULL,
    UpiNumber NVARCHAR(30) NULL,
    QRCodeUrl NVARCHAR(500) NULL,

    -- Bank transfer configuration
    AccountHolderName NVARCHAR(200) NULL,
    AccountNumber NVARCHAR(50) NULL,
    IFSCCode NVARCHAR(20) NULL,
    BankName NVARCHAR(150) NULL,

    -- UI configuration
    -- Shared UI configuration
    PaymentModeLabel NVARCHAR(100) NULL,
    BadgeText NVARCHAR(50) NULL,
    CollectionInstructions NVARCHAR(500) NULL,
    ContactNumber NVARCHAR(30) NULL,
    ContactLabel NVARCHAR(100) NULL,
    Instructions NVARCHAR(500) NULL,
    IsActive BIT NOT NULL
        CONSTRAINT DF_PaymentConfiguration_IsActive DEFAULT (1),

    DisplayOrder INT NOT NULL
        CONSTRAINT DF_PaymentConfiguration_DisplayOrder DEFAULT (0),

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_PaymentConfiguration_CreatedAt
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_PaymentConfiguration
        PRIMARY KEY (PaymentConfigurationId),

    CONSTRAINT FK_PaymentConfiguration_PaymentMethod
        FOREIGN KEY (PaymentMethodId)
        REFERENCES dbo.PaymentMethod(PaymentMethodId),

    CONSTRAINT UQ_PaymentConfiguration_Code
        UNIQUE (ConfigurationCode)
);
