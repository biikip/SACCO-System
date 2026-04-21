using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class Rate
    {
        [Key]
        [MaxLength(50)]
        public int strId { get; set; }
        [Required]
        [DisplayName("Rate")]
        public string strRate { get; set; }
    }
}
