using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EASY_SACCO.Models
{
    public class Society_parameters
    {
        [Key]
        [MaxLength(50)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int strId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }
 
        [DisplayName("Address")]
        public string Address { get; set; }
       
        [DisplayName("Website")]
        public string 	Website { get; set; }
    
        [DisplayName("Member maturity")]
        public string 	Membermaturity { get; set; }
 
        [DisplayName("Withdrawal Notice")]
        public string WithdrawalNotice { get; set; }
        
        [DisplayName("Phone No")]
        public string 	Phoneno { get; set; }
       
        [DisplayName("Email")]
        public string Email { get; set; }
    
        [DisplayName("Rounding off")]
        public int 	Roundingoff { get; set; }
    
        [DisplayName("Action on defaulted interest")]
        public string Actionondefaultedinterest { get; set; }
   
        public int MinimumGuarantors { get; set; }
        public int MaximumGuarantors { get; set; }
        public int Minimumage { get; set; }
        public int Retirementage { get; set; }
        	
    }
}
