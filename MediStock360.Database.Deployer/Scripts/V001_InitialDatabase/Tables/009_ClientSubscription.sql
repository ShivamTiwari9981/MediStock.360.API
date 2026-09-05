IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'ClientSubscription'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE dbo.ClientSubscription
(
    ClientSubscriptionId BIGINT IDENTITY(1,1) NOT NULL,
    ClientId BIGINT NOT NULL,
    SubscriptionPlanId INT NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    [Status] TINYINT NOT NULL, -- 1 = Trial, 2 = Active, 3 = Expired, 4 = Cancelled
    BillingCycle TINYINT NOT NULL, -- 1 = Monthly, 2 = Yearly
    Amount DECIMAL(18,2) NOT NULL,
    CurrencyCode CHAR(3) NOT NULL DEFAULT ('INR'),
    AutoRenew BIT NOT NULL DEFAULT (0),
    PaymentStatus TINYINT NOT NULL, -- 1 = Pending, 2 = Paid, 3 = Failed, 4 = Refunded
    TransactionReference NVARCHAR(150) NULL,
    IsTrial BIT NOT NULL DEFAULT (0),
    TrialEndDate DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_ClientSubscription
        PRIMARY KEY (ClientSubscriptionId),

    CONSTRAINT FK_ClientSubscription_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT FK_ClientSubscription_SubscriptionPlan
        FOREIGN KEY (SubscriptionPlanId)
        REFERENCES SubscriptionPlan(SubscriptionPlanId),

    CONSTRAINT CK_ClientSubscription_Status
        CHECK (Status IN (1, 2, 3, 4)),

    CONSTRAINT CK_ClientSubscription_BillingCycle
        CHECK (BillingCycle IN (1, 2)),

    CONSTRAINT CK_ClientSubscription_PaymentStatus
        CHECK (PaymentStatus IN (1, 2, 3, 4)),

    CONSTRAINT CK_ClientSubscription_Amount
        CHECK (Amount >= 0),

    CONSTRAINT CK_ClientSubscription_Date
        CHECK (EndDate IS NULL OR EndDate >= StartDate),

    CONSTRAINT CK_ClientSubscription_Trial
        CHECK
        (
            (IsTrial = 0 AND TrialEndDate IS NULL)
            OR
            (IsTrial = 1 AND TrialEndDate IS NOT NULL)
        )
);

CREATE INDEX IX_ClientSubscription_ClientId
ON ClientSubscription(ClientId);

CREATE INDEX IX_ClientSubscription_SubscriptionPlanId
ON ClientSubscription(SubscriptionPlanId);

CREATE INDEX IX_ClientSubscription_Status
ON ClientSubscription(Status);