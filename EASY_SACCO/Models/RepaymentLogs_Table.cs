using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EASY_SACCO.Models
{
    public class RepaymentLogs_Table
    {
     
            [Key]
            [MaxLength(50)]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int strId { get; set; }
            [Required]

            public string strLoanId { get; set; }
            [Required]

            public DateTime strExpDate { get; set; }
            [Required]
            public double strExpAmount { get; set; }
            [Required]
            public double strPenaltyAmount { get; set; }
            [Required]
            public double strTotalAmountwithPenalty { get; set; }
            [Required]
            public bool strOverdueStatus { get; set; }
            [Required]
            public DateTime strDatePaid { get; set; }
            [Required]
            public double strInterest { get; set; }
        
    }
}
