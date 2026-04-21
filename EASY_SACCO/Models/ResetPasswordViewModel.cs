using System.ComponentModel.DataAnnotations;

namespace EASY_SACCO.Models
{
    public class ResetPasswordViewModel
    {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Token { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        

    }
}
