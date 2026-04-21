using System;

namespace EASY_SACCO.Models
{
    public class FundDrive
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public decimal TotalCollected { get; set; }
        public DateTime DateHeld { get; set; }
        public Project Project { get; set; }
    }
}
