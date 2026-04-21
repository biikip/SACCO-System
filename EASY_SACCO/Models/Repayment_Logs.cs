using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EASY_SACCO.Models
{
    public class Repayment_Logs
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int strId { get; set; }

        public string strLoanId { get; set; }
        public DateTime strExDate { get; set; }
        public double strExpAmount { get; set; }
        public DateTime strDatePaid { get; set; }
        public double strInterest { get; set; }
    }
}
