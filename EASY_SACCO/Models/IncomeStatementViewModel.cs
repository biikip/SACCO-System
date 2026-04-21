using System;
using System.Collections.Generic;

namespace EASY_SACCO.Models
{
    public class IncomeStatementViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public List<IncomeStatementItem> IncomeStatementItems { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetIncome { get; set; }
    }

    public class IncomeStatementItem
    {
        public string AccountName { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // Revenue or Expense
    }
}
