IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'StoreUserMap'
      AND schema_id = SCHEMA_ID('dbo')
)

CREATE TABLE StoreUserMap
(
    StoreUserMapId BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_StoreUserMap PRIMARY KEY,

    ClientId BIGINT NOT NULL,
    StoreId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,
    IsDefaultStore bit Not null default (1),

    IsActive BIT NOT NULL
        CONSTRAINT DF_StoreUserMap_IsActive DEFAULT (1),

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_StoreUserMap_CreatedAt DEFAULT (SYSUTCDATETIME()),

    UpdatedAt DATETIME2 NULL,

    CONSTRAINT FK_StoreUserMap_Client
        FOREIGN KEY (ClientId)
        REFERENCES Client(ClientId),

    CONSTRAINT FK_StoreUserMap_Store
        FOREIGN KEY (StoreId)
        REFERENCES Store(StoreId),

    CONSTRAINT FK_StoreUserMap_User
        FOREIGN KEY (UserId)
        REFERENCES [User](UserId),

    CONSTRAINT UQ_StoreUserMap_Client_Store_User
        UNIQUE (ClientId, StoreId, UserId)
);

CREATE INDEX IX_StoreUserMap_ClientId
    ON StoreUserMap(ClientId);

CREATE INDEX IX_StoreUserMap_StoreId
    ON StoreUserMap(StoreId);

CREATE INDEX IX_StoreUserMap_UserId
    ON StoreUserMap(UserId);