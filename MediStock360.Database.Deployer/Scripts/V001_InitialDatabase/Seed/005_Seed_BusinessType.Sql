SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    INSERT INTO BusinessType
    (
        BusinessTypeCode,
        BusinessTypeName,
        IsActive,
        CreatedAt
    )
    SELECT
        V.BusinessTypeCode,
        V.BusinessTypeName,
        1,
        GETUTCDATE()
    FROM
    (
        VALUES
            ('PHARMACY',       'Pharmacy'),
            ('MEDICAL_STORE',  'Medical Store'),
            ('WHOLESALE',      'Medical Wholesale'),
            ('DISTRIBUTOR',    'Medical Distributor'),
            ('HOSPITAL',       'Hospital'),
            ('CLINIC',         'Clinic'),
            ('DIAGNOSTIC',     'Diagnostic Center'),
            ('OTHER',          'Other')
    ) V(BusinessTypeCode, BusinessTypeName)
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM BusinessType B
        WHERE B.BusinessTypeCode = V.BusinessTypeCode
    );

    COMMIT TRANSACTION;

    PRINT 'BusinessType seed data inserted successfully.';
END TRY
BEGIN CATCH

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;