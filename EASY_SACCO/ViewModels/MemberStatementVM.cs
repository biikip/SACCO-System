using EASY_SACCO.Models;
using System.Collections.Generic;

namespace EASY_SACCO.ViewModels
{
    public class MemberStatementVM
    {
        public string MemberName { get; set; }
        public string MemberNo { get; set; }
        public List<Customer_Balance> Contributions { get; set; }
    }
}
