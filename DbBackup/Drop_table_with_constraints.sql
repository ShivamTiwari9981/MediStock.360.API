SELECT
    fk.name AS ForeignKeyName,
    OBJECT_SCHEMA_NAME(fk.parent_object_id) AS TableSchema,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable
FROM sys.foreign_keys fk
WHERE fk.parent_object_id = OBJECT_ID('dbo.PaymentConfiguration')
   OR fk.referenced_object_id = OBJECT_ID('dbo.PaymentConfiguration');


ALTER TABLE dbo.ClientPayment
DROP CONSTRAINT FK_ClientPayment_PaymentConfiguration;