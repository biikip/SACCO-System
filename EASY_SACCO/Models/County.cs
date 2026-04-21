using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class County
    {
        [Key]
        [MaxLength(50)]
        public int strId { get; set; }
        [Required]
        [DisplayName("County")]
        public string strCountyName { get; set; }
        [Required]
        [DisplayName("Location Code")]
        public string strCountyCode { get; set; }
    }
}
