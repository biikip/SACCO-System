using System;

namespace EASY_SACCO.Models
{
    public class LoginAttempt
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime AttemptedAt { get; set; }
        public string IPAddress { get; set; }
        public string Status { get; set; } // e.g. "Failed" or "Success"
    }
}
