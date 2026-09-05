IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'BusinessType'
      AND schema_id = SCHEMA_ID('dbo')
)
BEGIN
CREATE TABLE dbo.BusinessType (   
    BusinessTypeId int PRIMARY KEY identity(1,1) ,
    BusinessTypeCode NVARCHAR(150) NOT NULL,
    BusinessTypeName NVARCHAR(150) NOT NULL,
    IsActive bit default 1,
    IsSynced bit default 0,
    CreatedAt DATETIME2 NOT NULL
        DEFAULT (SYSUTCDATETIME()),

    CreatedBy BIGINT NULL,

    UpdatedAt DATETIME2 NULL,
    UpdatedBy BIGINT NULL
);
END