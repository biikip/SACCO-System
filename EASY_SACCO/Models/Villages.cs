using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class Villages
    {
        [Key]
        [MaxLength(50)]
        public int strId { get; set; }
        [Required]
        [DisplayName("County")]
        public string strCountyId { get; set; }
        [Required]
        [DisplayName("Sub-County")]
        public string strSubCountId { get; set; }
        [Required]
        [DisplayName("Ward")]
        public string strWardId { get; set; }
        [Required]
        [DisplayName("Village")]
        public string strVillageName { get; set; }
    }
}
