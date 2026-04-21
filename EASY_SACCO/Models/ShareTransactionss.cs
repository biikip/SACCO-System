using System;
using System.ComponentModel.DataAnnotations;

namespace EASY_SACCO.Models
{
    public class ShareTransactionss
    {
        [Key]
        public int Id { get; set; }
        public string MemberNo { get; set; }
        public string Docnumber { get; set; }
        public decimal TransAmount { get; set; }
        public DateTime TransDate { get; set; }
        public string TransDescription { get; set; }

    }
}