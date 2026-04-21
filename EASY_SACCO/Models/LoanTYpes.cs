using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class LoanTYpes
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [DisplayName("Loan Code")]
        public string strloancode { get; set; }
        [Required]
        [DisplayName("R.Period(Months)")]
        public int strRPeriod { get; set; }
   
        
        [DisplayName("CompanyCode")]
        public string CompanyCode { get; set; }
        [Required]
        [DisplayName("Loan Control A/C")]
        public string strLoanAccount { get; set; }
        [Required]
        [DisplayName("Interest Control A/C")]
        public string strInterestAccount { get; set; }
        [Required]
        [DisplayName("Penalty Control Account")]
        public string strPenaltyAccount { get; set; }
        [Required]
        [DisplayName("Loan Type")]
        public string strLoanType { get; set; }
        [Required]
        [DisplayName("Repayment Method")]
        public string strRepaymentMethod { get; set; }
        [Required]
        [DisplayName("InterestRate")]
        public string strInterestRate { get; set; }      
        [Required]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        [DisplayName("Max.Loans")]
        public decimal? strMaxloan { get; set; }
        public decimal? LoanToShareRatio { get; set; }
      
        [Required]
        [DisplayName("Attracts Penalty")]
        public bool strAttractsPenalty { get; set; }
        [Required]
        [DisplayName("Can it Refinance")]
        public bool strCanitRefinance { get; set; }
        [Required]
        [DisplayName("Requires Guarantors")]
        public bool strRequiresGuarantors { get; set; }
        [Required]
        [DisplayName("Guarantee own loan")]
        public bool strGuaranteeownloan { get; set; }
        [Required]
        [DisplayName("Rate")]
        public string? strRate { get; set; }
        [Required]
        [DisplayName("What To Charge")]
        public string? strwhatToChange { get; set; }
        [Required]
        [DisplayName("Penalty Type")]
        public string? str { get; set; }
        public decimal Amount { get; set; }
        public string AuditId { get; set; } = string.Empty; 
        public DateTime? AuditTime { get; set; }
       public LoanTYpes()
        {
            AuditTime = DateTime.Now;

        }
    }
}
