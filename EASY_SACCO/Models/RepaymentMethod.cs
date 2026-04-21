using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class RepaymentMethod
    {
        [Key]
        [MaxLength(50)]
        public int strId { get; set; }
        [Required]
        [DisplayName("Method")]
        public string strMethod { get; set; }
    }
}
