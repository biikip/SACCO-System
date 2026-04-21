using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EASY_SACCO.Models
{
    public class System_Users
    {
        [Key]
      
        public int strId { get; set; }
     
        [Required]
        [DisplayName("Company")]
        public string strCompanyId { get; set; }
        [Required]
        [DisplayName("User Type")]
        public string strUserType { get; set; }
        [Required]
        [DisplayName("Full Name")]
        public string strFirstName { get; set; }
        [Required]
        [DisplayName("Full Name")]
        public string strLastName { get; set; }
        [Required]
        [DisplayName("Username")]
        public string strUserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [MinLength(6, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string strPassword { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("strPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]

        [DisplayName("Phone No")]
        public string strPhoneNo { get; set; }
        [Required]
        [DisplayName("Email")]
        public string strEmail { get; set; }
        public string? MemberNo { get; set; }
        public DateTime? DateCreated { get; set; }
        //[Required]
        //[DisplayName("Sub-County")]
        //public string strSubCountId { get; set; }
        //[Required]
        //[DisplayName("Ward")]
        //public string strWardId { get; set; }
    }
}
