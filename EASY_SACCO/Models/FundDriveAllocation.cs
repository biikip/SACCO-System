using System;

namespace EASY_SACCO.Models
{
    public class FundDriveAllocation
    {
        public int Id { get; set; }
        public string? MemberNo { get; set; }
        public string? Names { get; set; }
        public int ProjectId { get; set; }
        public decimal ContributionAmount { get; set; }
        public decimal OwnershipPercentage { get; set; }
        public DateTime AllocationDate { get; set; }
        public Project Project { get; set; }
    }
}