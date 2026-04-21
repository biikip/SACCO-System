using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace EASY_SACCO.Models
{
    public class LoanAplication
    {
        [Key]
        public int Id { get; set; }
        public string MemberNo { get; set; }      
        public string? LoanNo { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? AppliedAmount { get; set; }
        public decimal? Deposit { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? ShareCapital { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? OutstandingLoan { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? LoanBalance { get; set; }   
        public DateTime ApplicationDate { get; set; }
        public DateTime AuditTime { get; set; }
        public int? RepaymentPeriod { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal InterestRate { get; set; }   
        public string? RepaymentMethod { get; set; }
        public string? LoanCode { get; set; }
        public string? AuditId { get; set; }
        public string? CompanyCode { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? ApprovedAmount { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? PrincipalRepayment { get; set; }
        public decimal? InterestRepayment { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? TotalAmount { get; set; }
        public decimal? RecommendedAmount { get; set; }

        public DateTime LastRepayDate { get; set; }
        public DateTime DateApproved { get; set; }
        public string? Approver { get; set; }
        public int? status { get; set; }
        public bool? Reject { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal? TotalCharges { get; set; } 
        public decimal?  NetPay { get; set; } 
        public decimal? DisburseAmount { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public DateTime DisburseDate { get; set; }
        public string MinuteNumber { get; set; } = string.Empty;
        public string BankAccount { get; set; } = string.Empty;

        public string ReferenceNumber { get; set; } = string.Empty;

        public LoanAplication()
        {
            AuditTime = DateTime.Now;
            
        }


    }
}
