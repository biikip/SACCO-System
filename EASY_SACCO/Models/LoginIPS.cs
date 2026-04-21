using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class LoginIPS
    {
        [Key]
        [MaxLength(50)]
        public int strId { get; set; }
     
        public DateTime strDate { get; set; }
       
        public string strUserId { get; set; }
      
        public string strName { get; set; }
        public string strMachineName { get; set; }
        public string strMachineIP { get; set; }
    }
}
