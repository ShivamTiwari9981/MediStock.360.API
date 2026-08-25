IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'SubscriptionPlan'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN

CREATE TABLE dbo.SubscriptionPlan
(
    SubscriptionPlanId INT IDENTITY(1,1) NOT NULL,
    PlanCode NVARCHAR(50) NOT NULL,
    PlanName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    BillingCycle TINYINT NOT NULL, -- 1 = Monthly, 2 = Yearly
    Price DECIMAL(18,2) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL DEFAULT ('INR'),
    MaxStores INT NOT NULL,
    MaxUsers INT NOT NULL,
    MaxProducts INT NULL,
    MaxInvoicesPerMonth INT NULL,
    MaxCustomers INT NULL,
    MaxSuppliers INT NULL,
    IsInventoryEnabled BIT NOT NULL DEFAULT (1),
    IsPurchaseEnabled BIT NOT NULL DEFAULT (1),
    IsSalesEnabled BIT NOT NULL DEFAULT (1),
    IsReportsEnabled BIT NOT NULL DEFAULT (1),
    IsAIEnabled BIT NOT NULL DEFAULT (0),
    IsActive BIT NOT NULL DEFAULT (1),
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_SubscriptionPlan
        PRIMARY KEY (SubscriptionPlanId),

    CONSTRAINT UQ_SubscriptionPlan_PlanCode
        UNIQUE (PlanCode),

    CONSTRAINT CK_SubscriptionPlan_Price
        CHECK (Price >= 0),

    CONSTRAINT CK_SubscriptionPlan_MaxStores
        CHECK (MaxStores > 0),

    CONSTRAINT CK_SubscriptionPlan_MaxUsers
        CHECK (MaxUsers > 0),

    CONSTRAINT CK_SubscriptionPlan_BillingCycle
        CHECK (BillingCycle IN (1, 2)),

    CONSTRAINT CK_SubscriptionPlan_CurrencyCode
        CHECK (CurrencyCode IN ('INR', 'USD', 'EUR', 'GBP', 'AED'))
);
END