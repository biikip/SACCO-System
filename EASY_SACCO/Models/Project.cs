using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace EASY_SACCO.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ExpectedROI { get; set; }
        public int DurationMonths { get; set; }
        public decimal DailyContributionAmount { get; set; }
        public bool IsActive { get; set; }

        // ✅ Navigation property for subscriptions
        public virtual ICollection<ProjectSubscription> ProjectSubscriptions { get; set; } = new List<ProjectSubscription>();

        // ✅ Computed property: sum of all contributions
        [NotMapped]
        public decimal TotalFundsRaised => ProjectSubscriptions?.Sum(s => s.ContributionAmount) ?? 0;
        // ✅ Navigation property for fund allocations
        public virtual ICollection<FundDriveAllocation> FundDriveAllocations { get; set; } = new List<FundDriveAllocation>();
    }
}
