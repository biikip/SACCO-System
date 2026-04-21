using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace EASY_SACCO.Models
{
    public class GLTransaction
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int strId { get; set; }
        [Required]
        [DisplayName("Doc Number")]
        public string strDocnumber { get; set; }
        [Required]
        [DisplayName("Share Type")]
        public string strShareType { get; set; }
        [Required]
        [DisplayName("Description")]
        public string strDescription { get; set; }
        [Required]
        [DisplayName("Date")]
        public DateTime strDate { get; set; }
        [Required]
        [DisplayName("Credit")]
        public bool strCredit { get; set; }
        [Required]
        [DisplayName("Debit")]
        public bool strDebit { get; set; }
        [Required]
        [DisplayName("Balance")]
        public string strBalance { get; set; }
    }
}
