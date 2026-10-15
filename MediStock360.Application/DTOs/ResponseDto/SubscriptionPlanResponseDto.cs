using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediStock360.Application.DTOs.ResponseDto
{

    public class SubscriptionPlanDto
    {
        public int SubscriptionPlanId { get; set; }
        
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public string Description { get; set; }
        public int BillingCycle { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public List<SubscriptionFeatureDto> Features { get; set; }
    }

    public class SubscriptionFeatureDto
    {
        public int SubscriptionFeatureId { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public int FeatureValue { get; set; }
        public bool IsEnabled { get; set; }
        public int DisplayOrder { get; set; }
        public string DisplayTemplate { get; set; }
    }
    public class SubscriptionPlanResponseDto
    {
        public int SubscriptionPlanId { get; set; }
        public int SubscriptionFeatureId { get; set; }
        public string PlanCode { get; set; }
        public string PlanName { get; set; }
        public string Description { get; set; }
        public int BillingCycle { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public string DisplayTemplate { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureName { get; set; }
        public int FeatureValue { get; set; }
        public bool IsEnabled { get; set; }
        public int DisplayOrder { get; set; }
    }
}
