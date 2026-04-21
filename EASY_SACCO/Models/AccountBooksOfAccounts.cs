using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class AccountBooksOfAccounts
    {

        [Key]
        public int Id { get; set; }
        public string accno { get; set; }
        public string accname { get; set; }
        public decimal amount { get; set; }
        public string transtype { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string auditid { get; set; }
        public DateTime auditdatetime { get; set; }
        public string AccType { get; set; }
        public string GlAccMainGroup { get; set; }
        public string AccGroup { get; set; }
        public decimal DR { get; set; }
        public decimal CR { get; set; }
        public string CompanyCode { get; set; }
    }
}
