using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace EASY_SACCO.Models
{
    public class ShareTransaction
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int strId { get; set; }
        [Required]
        [DisplayName("Doc Number")]
        public string? strDocnumber { get; set; }
        [Required]
        [DisplayName("Member No")]
        public string? strMemberNo { get; set; }
        [Required]
        [DisplayName("Bank Name")]
        public string? strBankAc { get; set; }
        [Required]
        [DisplayName("Shares Code")]
        public string? strShareCode { get; set; }
        //[Required]
        //[DisplayName("Payment Mode ")]
        //public string strPaymentMode { get; set; }
        [Required]
        [DisplayName("Description")]
        public string? strDescription { get; set; }
        //[Required]
        //[DisplayName("Date Deposited")]
        //public DateTime strDateDeposited { get; set; }
        //[Required]
        //[DisplayName("Date Contribution")]
        //public DateTime strDateContribution { get; set; }
      
        [Required]
        [DisplayName("Balance")]
        public string? strBalance { get; set; }

        [Required]
        [DisplayName("Amount")]
        public string? strAmount { get; set; }
        [Required]
        [DisplayName("Credit")]
        public string? strCredit { get; set; }
        [Required]
        [DisplayName("Debit")]
        public string? strDebit { get; set; }       
        //[Required]
        [DisplayName("Audit Id")]
        public string? strAuditId { get; set; }  
       // [Required]
        [DisplayName("Date")]
        public DateTime? strDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
