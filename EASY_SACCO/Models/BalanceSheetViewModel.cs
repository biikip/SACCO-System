using System;
using System.Collections.Generic;

namespace EASY_SACCO.Models
{
    public class BalanceSheetViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedDate { get; set; }

        public List<BalanceSheetItem> Assets { get; set; }
        public List<BalanceSheetItem> Liabilities { get; set; }
        public List<BalanceSheetItem> Equity { get; set; }

        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public decimal NetBalance { get; set; }
        public decimal TotalLiabilitiesAndEquity { get; set; }
    }
    public class BalanceSheetItem
    {
        public string AccNo { get; set; }
        public string AccName { get; set; }
        public decimal Balance { get; set; }
        public decimal TotalLiabilitiesAndEquity { get; set; } 
    }
}
