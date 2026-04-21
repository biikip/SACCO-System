using System;
using System.Collections.Generic;

namespace EASY_SACCO.Models
{
    public class TrialBalanceViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public List<TrialBalanceItem> TrialBalanceItems { get; set; }
    }

    public class TrialBalanceItem
    {
        public string AccNo { get; set; }
        public string AccName { get; set; }
        public string AccGroup { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }
}
