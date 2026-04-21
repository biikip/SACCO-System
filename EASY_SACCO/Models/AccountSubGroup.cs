using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class AccountSubGroup
    {
        [Key]
        [MaxLength(50)]
        public int strId { get; set; }
        [Required]
        [DisplayName("Account SubGroup")]
        public string strAccountSubGroup { get; set; }
    }
}
