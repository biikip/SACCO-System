using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EASY_SACCO.Models
{
    public class MEMBERS
    {
        [Key]
        public int ID { get; set; }
        public string? PhoneNo { get; set; }
        public string? SurName { get; set; }
        public string? OtherNames { get; set; }
        public string? Email { get; set; }
        public string? IDNO { get; set; }
        public string? RegistrationToken { get; set; }
    }
}