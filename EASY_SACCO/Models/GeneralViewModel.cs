using System.Collections.Generic;

namespace EASY_SACCO.Models
{
    public class GeneralViewModel
    {
        public List<User_Group> user_Groups { get; set; }
        public List<System_Users> system_Users { get; set; }
        public List<Co_operative> co_Operatives { get; set; }
        public System_Users NewUser { get; set; }
    }
}
