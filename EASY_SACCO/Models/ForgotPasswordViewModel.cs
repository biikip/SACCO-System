using System.ComponentModel.DataAnnotations;

namespace EASY_SACCO.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Username or Email")]
        public string UsernameOrEmail { get; set; }
    }
}
