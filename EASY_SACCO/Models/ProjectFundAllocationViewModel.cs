using System;

namespace EASY_SACCO.Models
{
    public class ProjectFundAllocationViewModel
    {
        public string ProjectName { get; set; }
        public string MemberNo { get; set; }
        public string MemberName { get; set; }
        public decimal TotalContribution { get; set; }
        public decimal OwnershipPercentage { get; set; }
        public DateTime AllocationDate { get; set; }
    }
}
