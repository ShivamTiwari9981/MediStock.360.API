using System;
using System.Collections.Generic;

namespace MediStock360.Application.DTOs.ResponseDto
{
    public class ProductOwnerDashboardDto
    {
        public int TotalClients { get; set; }
        public int ActiveClients { get; set; }
        public int PendingClients { get; set; }
        public int InactiveClients { get; set; }
        public int TotalStores { get; set; }
        public int ActiveStores { get; set; }
        public int PendingApprovals { get; set; }
        public int ApprovedPayments { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ProductOwnerClientDto> RecentClients { get; set; } = new List<ProductOwnerClientDto>();
        public List<PendingApprovalDto> RecentApprovals { get; set; } = new List<PendingApprovalDto>();
    }
}

