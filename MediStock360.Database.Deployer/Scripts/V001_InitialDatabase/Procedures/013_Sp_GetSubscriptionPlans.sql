--select * from SubscriptionPlanFeature
--select * from SubscriptionFeature
--exec Sp_GetSubscriptionPlans
CREATE OR ALTER PROCEDURE dbo.Sp_GetSubscriptionPlans
AS
BEGIN
    SELECT
    PF.SubscriptionPlanId,
    PF.SubscriptionFeatureId,
    P.Description,
    P.BillingCycle,
    P.Price,
    P.CurrencyCode,
    P.PlanCode,
    P.PlanName,
    F.FeatureCode,
    F.FeatureName,
    PF.FeatureValue,
    PF.IsEnabled,
    PF.DisplayOrder,
    F.DisplayTemplate
    
    FROM dbo.SubscriptionPlanFeature PF
    INNER JOIN dbo.SubscriptionPlan P
        ON P.SubscriptionPlanId = PF.SubscriptionPlanId
    INNER JOIN dbo.SubscriptionFeature F
        ON F.SubscriptionFeatureId = PF.SubscriptionFeatureId
    WHERE PF.FeatureValue IS NOT NULL AND PF.IsEnabled = 1 and F.IsActive =1
    ORDER BY
        P.SubscriptionPlanId,
        PF.DisplayOrder;
END


