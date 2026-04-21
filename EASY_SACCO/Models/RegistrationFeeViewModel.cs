using System;

namespace EASY_SACCO.Models
{
    public class RegistrationFeeViewModel
    {
        public string MemberNo { get; set; }
        public string SurName { get; set; }
        public string OtherName { get; set; }
        public string PhoneNo { get; set; }
        public string IdNo { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransDate { get; set; }
    }
}
