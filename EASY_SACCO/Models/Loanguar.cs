using System;
using System.ComponentModel.DataAnnotations;

namespace EASY_SACCO.Models
{
    public class Loanguar
    {
        [Key]
        public int Id { get; set; }
        public string MemberNo { get; set; }
       
        public string LoanNo { get; set; }
        public decimal AmountGuaranteed { get; set; }
        public decimal? Balance { get; set; }
        public decimal AmountDeposit { get; set; }
        public string FullNames { get; set; }
        public DateTime Transdate { get; set; }
       // public bool transfered { get; set; }
        public string AuditId { get; set; }
       // public int status { get; set; }
        public DateTime AuditTime { get; set; }
       // public string Description { get; set; }
        public string CompanyCode { get; set; }

       // public decimal tguaranto { get; set; }
       public Loanguar()
        {
            AuditTime=DateTime.Now;
            Transdate=DateTime.Now; 
        }
    }
}
