using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class AccountCategory
    { 

    [Key]
     public int strId { get; set; }
    [Required]
    [DisplayName("Account Category")]
    public string strAcCategory { get; set; }
    public string Saccocode { get; set; }
}
}
