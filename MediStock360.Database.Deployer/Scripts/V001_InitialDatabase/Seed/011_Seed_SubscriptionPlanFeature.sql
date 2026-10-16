
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    ;WITH PlanList AS
    (
        SELECT *
        FROM (VALUES
            ('FREE',             0),
            ('BASIC_MONTHLY',    1),
            ('STANDARD_MONTHLY', 2),
            ('PREMIUM_MONTHLY',  3),
            ('BASIC_YEARLY',     1),
            ('STANDARD_YEARLY',  2),
            ('PREMIUM_YEARLY',   3),
            ('CUSTOM',           4)
        ) P (PlanCode, TierLevel)
    ),
    FeatureList AS
    (
        SELECT *
        FROM (VALUES
            ('BARCODE',                  1),
            ('EXPIRY_ALERT',             2),
            ('LOW_STOCK_ALERT',          3),
            ('MULTI_STORE',              4),
            ('ADVANCED_REPORTS',         5),
            ('AI_DEMAND_FORECAST',       6),
            ('AI_STOCK_RECOMMENDATION',  7),
            ('WHATSAPP_INVOICE',         8),
            ('IMPORT_EXPORT',            9),
            ('AUDIT_LOG',               10),
            ('PURCHASE_MANAGEMENT',     11),
            ('SALES_MANAGEMENT',        12),
            ('CUSTOMER_MANAGEMENT',     13),
            ('SUPPLIER_MANAGEMENT',     14),
            ('USER_LIMIT',              15),
            ('PRODUCT_LIMIT',           16),
            ('INVOICE_LIMIT',           17)
        ) F (FeatureCode, DisplayOrder)
    ),
    FeatureSettings AS
    (
        SELECT
            P.PlanCode,
            P.TierLevel,
            F.FeatureCode,
            F.DisplayOrder,

            CAST(
                CASE
                    WHEN F.FeatureCode = 'MULTI_STORE'
                        THEN CASE P.TierLevel
                            WHEN 0 THEN 1
                            WHEN 1 THEN 1
                            WHEN 2 THEN 3
                            WHEN 3 THEN 5
                            ELSE NULL
                        END

                    WHEN F.FeatureCode = 'USER_LIMIT'
                        THEN CASE P.TierLevel
                            WHEN 0 THEN 1
                            WHEN 1 THEN 3
                            WHEN 2 THEN 10
                            WHEN 3 THEN 25
                            ELSE NULL
                        END

                    WHEN F.FeatureCode = 'PRODUCT_LIMIT'
                        THEN CASE P.TierLevel
                            WHEN 0 THEN 100
                            WHEN 1 THEN 1000
                            WHEN 2 THEN 5000
                            WHEN 3 THEN 20000
                            ELSE NULL
                        END

                    WHEN F.FeatureCode = 'INVOICE_LIMIT'
                        THEN CASE P.TierLevel
                            WHEN 0 THEN 100
                            WHEN 1 THEN 1000
                            WHEN 2 THEN 5000
                            WHEN 3 THEN 20000
                            ELSE NULL
                        END

                    WHEN F.FeatureCode = 'CUSTOMER_MANAGEMENT'
                        THEN CASE P.TierLevel
                            WHEN 0 THEN 100
                            WHEN 1 THEN 1000
                            WHEN 2 THEN 5000
                            WHEN 3 THEN 20000
                            ELSE NULL
                        END

                    WHEN F.FeatureCode = 'SUPPLIER_MANAGEMENT'
                        THEN CASE P.TierLevel
                            WHEN 0 THEN 20
                            WHEN 1 THEN 100
                            WHEN 2 THEN 500
                            WHEN 3 THEN 2000
                            ELSE NULL
                        END

                    ELSE NULL
                END AS INT
            ) AS FeatureValue,

            CAST(
                CASE
                    -- Features available in every plan
                    WHEN F.FeatureCode IN
                    (
                        'BARCODE',
                        'EXPIRY_ALERT',
                        'LOW_STOCK_ALERT',
                        'MULTI_STORE',
                        'IMPORT_EXPORT',
                        'PURCHASE_MANAGEMENT',
                        'SALES_MANAGEMENT',
                        'CUSTOMER_MANAGEMENT',
                        'SUPPLIER_MANAGEMENT',
                        'USER_LIMIT',
                        'PRODUCT_LIMIT',
                        'INVOICE_LIMIT'
                    ) THEN 1

                    -- Standard, Premium and Custom plans
                    WHEN F.FeatureCode IN
                    (
                        'ADVANCED_REPORTS',
                        'WHATSAPP_INVOICE',
                        'AUDIT_LOG'
                    )
                    AND P.TierLevel >= 2 THEN 1

                    -- Premium and Custom plans
                    WHEN F.FeatureCode IN
                    (
                        'AI_DEMAND_FORECAST',
                        'AI_STOCK_RECOMMENDATION'
                    )
                    AND P.TierLevel >= 3 THEN 1

                    ELSE 0
                END AS BIT
            ) AS IsEnabled

        FROM PlanList P
        CROSS JOIN FeatureList F
    )
    INSERT INTO dbo.SubscriptionPlanFeature
    (
        SubscriptionPlanId,
        SubscriptionFeatureId,
        FeatureValue,
        IsEnabled,
        DisplayOrder
    )
    SELECT
        P.SubscriptionPlanId,
        F.SubscriptionFeatureId,
        S.FeatureValue,
        S.IsEnabled,
        S.DisplayOrder
    FROM FeatureSettings S
    INNER JOIN dbo.SubscriptionPlan P
        ON P.PlanCode = S.PlanCode
    INNER JOIN dbo.SubscriptionFeature F
        ON F.FeatureCode = S.FeatureCode
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.SubscriptionPlanFeature PF
        WHERE PF.SubscriptionPlanId = P.SubscriptionPlanId
          AND PF.SubscriptionFeatureId = F.SubscriptionFeatureId
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;