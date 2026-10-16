/* ============================================================
   1. SUBSCRIPTION PLANS
   ============================================================ */


INSERT INTO dbo.SubscriptionPlan
(
    PlanCode,
    PlanName,
    Description,
    BillingCycle,
    Price,
    CurrencyCode,
    TrialDays,
    IsActive,
    IsSynced,
    CreatedBy
)
VALUES
-- Free Plan: 30 days trial
(
    'FREE',
    'Free Plan',
    'Free access to basic inventory features for 30 days.',
    1,
    0.00,
    'INR',
    30,
    1,
    0,
    1
),

-- Basic Monthly Plan
(
    'BASIC_MONTHLY',
    'Basic Plan',
    'Basic inventory management for small businesses.',
    1,
    499.00,
    'INR',
    NULL,
    1,
    0,
    1
),

-- Standard Monthly Plan
(
    'STANDARD_MONTHLY',
    'Standard Plan',
    'Inventory, purchase, sales, and reporting features.',
    1,
    999.00,
    'INR',
    NULL,
    1,
    0,
    1
),

-- Premium Monthly Plan
(
    'PREMIUM_MONTHLY',
    'Premium Plan',
    'Advanced inventory management with AI-powered features.',
    1,
    1999.00,
    'INR',
    NULL,
    1,
    0,
    1
),

-- Basic Yearly Plan
(
    'BASIC_YEARLY',
    'Basic Yearly Plan',
    'Basic inventory management with annual billing.',
    2,
    4999.00,
    'INR',
    NULL,
    1,
    0,
    1
),

-- Standard Yearly Plan
(
    'STANDARD_YEARLY',
    'Standard Yearly Plan',
    'Inventory, purchase, sales, and reporting with annual billing.',
    2,
    9999.00,
    'INR',
    NULL,
    1,
    0,
    1
),

-- Premium Yearly Plan
(
    'PREMIUM_YEARLY',
    'Premium Yearly Plan',
    'Advanced inventory management and AI features with annual billing.',
    2,
    19999.00,
    'INR',
    NULL,
    1,
    0,
    1
),

-- Custom Plan
(
    'CUSTOM',
    'Custom Plan',
    'Custom pricing and duration based on customer requirements.',
    1,
    0.00,
    'INR',
    NULL,
    1,
    0,
    1
);