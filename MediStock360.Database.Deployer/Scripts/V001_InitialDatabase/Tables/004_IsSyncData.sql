IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'IsSyncData'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
CREATE TABLE dbo.IsSyncData (
    ClientId int NOT NULL,
    BranchId int NOT NULL,
    SyncId int primary key identity(1,1),
    TableName varchar(100)  NOT NULL,
    JsonData text  NOT NULL,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
    CreatedBy BIGINT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL
);
END