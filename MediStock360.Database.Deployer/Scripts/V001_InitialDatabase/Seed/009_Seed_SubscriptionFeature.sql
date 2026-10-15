
IF OBJECT_ID('dbo.SubscriptionFeature', 'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.SubscriptionFeature
    (
        FeatureCode,
        FeatureName,
        DisplayTemplate,
        IsActive,
        IsSynced,
        CreatedBy
    )
    SELECT
        V.FeatureCode,
        V.FeatureName,
        V.DisplayTemplate,
        1,
        0,
        1
    FROM
    (
        VALUES
        (
            'BARCODE',
            'Barcode Scanner',
            'Barcode scanning: {value}'
        ),
        (
            'EXPIRY_ALERT',
            'Expiry Alerts',
            'Expiry alerts: {value}'
        ),
        (
            'LOW_STOCK_ALERT',
            'Low Stock Alerts',
            'Low stock alerts: {value}'
        ),
        (
            'MULTI_STORE',
            'Multiple Stores',
            'Up to {value} stores'
        ),
        (
            'ADVANCED_REPORTS',
            'Advanced Reports',
            'Advanced reports'
        ),
        (
            'AI_DEMAND_FORECAST',
            'AI Demand Forecasting',
            'AI demand forecasting'
        ),
        (
            'AI_STOCK_RECOMMENDATION',
            'AI Stock Recommendations',
            'AI-powered stock recommendations'
        ),
        (
            'WHATSAPP_INVOICE',
            'WhatsApp Invoice',
            'WhatsApp invoice sharing'
        ),
        (
            'IMPORT_EXPORT',
            'Import and Export',
            'Data import and export'
        ),
        (
            'AUDIT_LOG',
            'Audit Logs',
            'Audit log tracking'
        ),
        (
            'PURCHASE_MANAGEMENT',
            'Purchase Management',
            'Purchase management'
        ),
        (
            'SALES_MANAGEMENT',
            'Sales Management',
            'Sales management'
        ),
        (
            'CUSTOMER_MANAGEMENT',
            'Customer Management',
            'Manage up to {value} customers'
        ),
        (
            'SUPPLIER_MANAGEMENT',
            'Supplier Management',
            'Manage up to {value} suppliers'
        ),
        (
            'USER_LIMIT',
            'User Limit',
            'Up to {value} users'
        ),
        (
            'PRODUCT_LIMIT',
            'Product Limit',
            'Up to {value} products'
        ),
        (
            'INVOICE_LIMIT',
            'Monthly Invoice Limit',
            'Up to {value} invoices per month'
        )
    ) AS V
    (
        FeatureCode,
        FeatureName,
        DisplayTemplate
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.SubscriptionFeature AS F
        WHERE F.FeatureCode = V.FeatureCode
    );
END;