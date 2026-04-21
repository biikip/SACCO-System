using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class ProgrationProcessing
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string MNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Ration { get; set; }
        public decimal Deposits { get; set; }
        public decimal PercentageUsed { get; set; }
        public string ShareType { get; set; }
        public decimal Weighted { get; set; }
        public decimal InterestDeposit { get; set; }
        public decimal ShareCap { get; set; }
        public decimal Gross { get; set; }
        public decimal Tax { get; set; }
        public decimal Netpay { get; set; }
        public DateTime AuditDate { get; set; } = DateTime.Now;
        public string Saccocode { get; set; }
    }
}
