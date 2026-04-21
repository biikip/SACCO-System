using System;

namespace EASY_SACCO.Models
{
    public class FundDriveAllocationViewModel
    {
        public Project Project { get; set; }
        public string? MemberNo { get; set; }
        public string MemberName { get; set; }
        public decimal ContributionAmount { get; set; }
        public double OwnershipPercentage { get; set; }
        public DateTime AllocationDate { get; set; }
    }
}
