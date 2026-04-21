using System;

namespace EASY_SACCO.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ActionType { get; set; } // e.g., "Login", "LoanApplication"
        public string Description { get; set; } // Optional details
        public DateTime Timestamp { get; set; }
        public string IPAddress { get; set; }
    }
}
