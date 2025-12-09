// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
using System.Collections.Generic;

namespace AMWE_RealTime_Server.Models {
    public class Role
    {
        public const string GlobalAdminRole = "admin";
        public const string GlobalUserRole = "user";
        public const string GlobalAdminGroup = "Admin";
        public const string GlobalUserGroup = "User";
        public const string GlobalDeveloperRole = "developer";

        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
