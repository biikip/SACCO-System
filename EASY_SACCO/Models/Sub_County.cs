using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class Sub_County
    {
        [Key]
        [MaxLength(50)]
        public int strId { get; set; }
        [Required]
        [DisplayName("County")]
        public string strCountyId { get; set;}
        [Required]
        [DisplayName("Sub-County")]
        public string strSubCountyName { get; set; }
      

    }
}
