using System;
using System.ComponentModel.DataAnnotations;

namespace EASY_SACCO.Models
{
    public class Customer_Balance
    {
        [Key]
        public int Id { get; set; }
        public string Memberno { get; set; }
        public string MemberName { get; set; }

        public decimal Bookbalance { get; set; }
        public decimal Availablebalance { get; set; }
        public string Accountno { get; set; }
        public decimal Amount { get; set; }

        public string CRaccno { get; set; }
        public string DRaccno { get; set; }
        public string Transactioncode { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public DateTime TransDate { get; set; }

        public string Transdescription { get; set; }
        public string Documentnumber { get; set; }
        public bool Reversed { get; set; }
        public string Transactionno { get; set; }
        public string Loanno { get; set; }
        public string Auditid { get; set; }
        public DateTime AuditTime { get; set; }
        public string Soure { get; set; }
        public string CompanyCode { get; set; }
        public Customer_Balance()
        {
            AuditTime = DateTime.Now;

        }
    }
}
