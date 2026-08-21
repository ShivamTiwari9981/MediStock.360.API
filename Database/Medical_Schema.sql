--drop database medical_store_db
--create database medical_store_db
--use medical_store_db



CREATE TABLE Country (
    CountryId INT IDENTITY PRIMARY KEY,                -- Internal PK
    CountryName NVARCHAR(100) NOT NULL,
    IsActive bit default 1,
    IsSynced bit default 0,
     CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,
);

CREATE UNIQUE INDEX IX_Country_CountryGuid ON Country(CountryId);


CREATE TABLE [State] (
    StateId INT IDENTITY(1,1) PRIMARY KEY,                  -- Internal PK
    StateName NVARCHAR(100) NOT NULL,
    CountryId INT NOT NULL,        
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT FK_State_Country FOREIGN KEY (CountryId)-- FK to Country
        REFERENCES Country(CountryId)
);

CREATE UNIQUE INDEX IX_State_State ON State(StateId);

CREATE TABLE City (
    CityId INT IDENTITY(1,1) PRIMARY KEY,                   -- Internal PK
    CityName NVARCHAR(100) NOT NULL,
    StateId INT NOT NULL,                              -- FK to State
    CountryId INT NOT NULL, 
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,
    CONSTRAINT FK_City_State FOREIGN KEY (StateId)-- FK to Country (optional but useful)
        REFERENCES State(StateId),
    CONSTRAINT FK_City_Country FOREIGN KEY (CountryId)
        REFERENCES Country(CountryId)
);

CREATE UNIQUE INDEX IX_City_City ON City(CityId);

CREATE TABLE IsSyncData (
    ClientId int NOT NULL,
    BranchId int NOT NULL,
    SyncId int primary key identity(1,1),
    TableName varchar(100)  NOT NULL,
    JsonData text  NOT NULL,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,
);

CREATE TABLE Menu (
    MenuId int NOT NULL primary key Identity(1,1),
    ParentMenuId int NULL default null,
    MenuName NVARCHAR(200) NOT NULL unique,
    MenuIcon NVARCHAR(50) NOT NULL,
    RouterLink NVARCHAR(100) NOT NULL,
    DisplayOrder int,
    [IsVisible] bit default 0,
    [IsActive] int default 1,
    IsSynced bit default 0,

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,
    CONSTRAINT FK_Menu_Parent FOREIGN KEY (ParentMenuId) REFERENCES Menu(MenuId)
);


CREATE TABLE BusinessType (   
    BusinessTypeId int PRIMARY KEY identity(1,1) ,
    BusinessTypeName NVARCHAR(150) NOT NULL,
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,
);

--drop table Client
CREATE TABLE Client (   
    ClientId BIGINT PRIMARY KEY Identity(100,1),
    ClientKey UNIQUEIDENTIFIER NOT NULL UNIQUE,
    ClientCode NVARCHAR(50) Unique NOT NULL,
    ClientName NVARCHAR(150) NOT NULL,
    CompanyName NVARCHAR(150) NOT NULL UNIQUE,
    OwnerName NVARCHAR(150) NULL,
    BusinessTypeId int NOT NULL,
    
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    GSTNumber NVARCHAR(50) NULL,
    DrugLicenseNumber NVARCHAR(150) NULL,
    [Address] TEXT,
    CityId Int NULL ,
    [StateId] Int NULL,
    [CountryId] Int NULL,
    [PostalCode] NVARCHAR(10) Null,
    IsOnboardingCompleted BIT NOT NULL DEFAULT 0,
    OnboardingStep INT NOT NULL DEFAULT 1,
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

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

CREATE TABLE SubscriptionPlan
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
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

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


CREATE TABLE ClientSubscription
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

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

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




-- 1. Shops (multi-branch support)
CREATE TABLE Store
(
    StoreId BIGINT IDENTITY(1,1) NOT NULL,

    ClientId BIGINT NOT NULL,
    StoreKey UNIQUEIDENTIFIER NOT NULL
        DEFAULT (NEWSEQUENTIALID()),

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

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

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


CREATE TABLE [User]
(
    UserId BIGINT IDENTITY(1,1) NOT NULL,

    ClientId BIGINT NOT NULL,
    StoreId BIGINT NULL,

    UserKey UNIQUEIDENTIFIER NOT NULL
        DEFAULT (NEWSEQUENTIALID()),

    EmployeeCode NVARCHAR(50) NULL,

    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NULL,

    Email NVARCHAR(150) NOT NULL,
    PhoneNumber NVARCHAR(20) NULL,

    UserName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,

    IsActive BIT NOT NULL DEFAULT (1),
    IsEmailVerified BIT NOT NULL DEFAULT (0),
    IsPhoneVerified BIT NOT NULL DEFAULT (0),

    LastLoginAt DATETIME2 NULL,

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_User
        PRIMARY KEY (UserId),

    CONSTRAINT FK_User_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT FK_User_Store
        FOREIGN KEY (StoreId)
        REFERENCES Store(StoreId),

    CONSTRAINT UQ_User_UserKey
        UNIQUE (UserKey),

    CONSTRAINT UQ_User_Client_Email
        UNIQUE (ClientId, Email),

    CONSTRAINT UQ_User_Client_UserName
        UNIQUE (ClientId, UserName)
);


CREATE TABLE Role
(
    RoleId INT IDENTITY(1,1) NOT NULL,

    ClientId BIGINT NULL,

    RoleCode NVARCHAR(50) NOT NULL,
    RoleName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,

    IsSystemRole BIT NOT NULL DEFAULT (0),
    IsActive BIT NOT NULL DEFAULT (1),

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL,

    CONSTRAINT PK_Role
        PRIMARY KEY (RoleId),

    CONSTRAINT FK_Role_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT UQ_Role_Client_RoleCode
        UNIQUE (ClientId, RoleCode)
);


CREATE TABLE UserRole
(
    UserRoleId BIGINT IDENTITY(1,1) NOT NULL,

    UserId BIGINT NOT NULL,
    RoleId INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    CONSTRAINT PK_UserRole
        PRIMARY KEY (UserRoleId),

    CONSTRAINT FK_UserRole_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT FK_UserRole_Role
        FOREIGN KEY (RoleId)
        REFERENCES Role(RoleId),

    CONSTRAINT UQ_UserRole_User_Role
        UNIQUE (UserId, RoleId)
);


CREATE TABLE Permission
(
    PermissionId INT IDENTITY(1,1) NOT NULL,

    PermissionCode NVARCHAR(100) NOT NULL,
    PermissionName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,

    ModuleName NVARCHAR(100) NOT NULL,

    IsActive BIT NOT NULL DEFAULT (1),

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    UpdatedAt DATETIME2 NULL,

    CONSTRAINT PK_Permission
        PRIMARY KEY (PermissionId),

    CONSTRAINT UQ_Permission_Code
        UNIQUE (PermissionCode)
);

CREATE TABLE RolePermission
(
    RolePermissionId BIGINT IDENTITY(1,1) NOT NULL,

    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    CONSTRAINT PK_RolePermission
        PRIMARY KEY (RolePermissionId),

    CONSTRAINT FK_RolePermission_Role
        FOREIGN KEY (RoleId)
        REFERENCES Role(RoleId),

    CONSTRAINT FK_RolePermission_Permission
        FOREIGN KEY (PermissionId)
        REFERENCES Permission(PermissionId),

    CONSTRAINT UQ_RolePermission_Role_Permission
        UNIQUE (RoleId, PermissionId)
);


CREATE TABLE MasterCodeGeneration
(
    MasterCodeGenerationId BIGINT IDENTITY(1,1) PRIMARY KEY,

    ClientId BIGINT NULL,

    CodeType NVARCHAR(50) NOT NULL,
    CodePrefix NVARCHAR(20) NOT NULL,

    CurrentNumber BIGINT NOT NULL DEFAULT 0,
    NumberLength INT NOT NULL DEFAULT 3,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedDate DATETIME2 NULL,

    CONSTRAINT FK_MasterCodeGeneration_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId)
);