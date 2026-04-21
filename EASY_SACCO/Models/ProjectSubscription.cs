using System;

namespace EASY_SACCO.Models
{
    public class ProjectSubscription
    {
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public string? Names { get; set; }
        public int ProjectId { get; set; }
        public decimal ContributionAmount { get; set; }
        public DateTime? SubscriptionDate { get; set; }


    }
}
