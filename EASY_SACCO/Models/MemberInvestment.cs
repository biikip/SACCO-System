using System;

namespace EASY_SACCO.Models
{
    public class MemberInvestment
    {
        public int Id { get; set; }
        public string MemberId { get; set; }
        public int ProjectId { get; set; }
        public decimal TotalContributed { get; set; }
        public decimal OwnershipPercentage { get; set; }
        public decimal PayoutReceived { get; set; }
        public DateTime LastContributionDate { get; set; }
    }
}
