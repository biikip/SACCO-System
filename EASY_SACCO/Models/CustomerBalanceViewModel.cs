using System;

namespace EASY_SACCO.Models
{
    public class CustomerBalanceViewModel
    {
        public string DocumentNumber { get; set; }
        public string MemberNo { get; set; }
        public string MemberName { get; set; }
        public decimal Amount { get; set; }
        public string CRAccNo { get; set; }
        public string DRAccNo { get; set; }
        public string AccountNo { get; set; }
        public DateTime TransDate { get; set; }
    }
}
