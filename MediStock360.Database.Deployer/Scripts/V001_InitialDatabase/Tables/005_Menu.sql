IF NOT EXISTS
(
    SELECT 1
    FROM sys.tables
    WHERE name = 'Menu'
      AND schema_id = SCHEMA_ID('dbo')
)
CREATE TABLE dbo.Menu (
    MenuId int NOT NULL primary key Identity(1,1),
    ParentMenuId int NULL default null,
    MenuName NVARCHAR(200) NOT NULL unique,
    MenuIcon NVARCHAR(50) NOT NULL,
    RouterLink NVARCHAR(100) NOT NULL,
    PermissionCode NVARCHAR(100) NULL,
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
--BEGIN
--    IF NOT EXISTS (
--        SELECT 1 FROM sys.columns 
--        WHERE object_id = OBJECT_ID('dbo.Menu') AND name = 'PermissionCode'
--    )
--    BEGIN
--        ALTER TABLE dbo.Menu ADD PermissionCode NVARCHAR(100) NULL;
--    END
--END