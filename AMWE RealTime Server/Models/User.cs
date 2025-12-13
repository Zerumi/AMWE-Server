// This code & software is licensed under the Creative Commons license. You can't use AMWE trademark 
// You can use & improve this code by keeping this comments
// (or by any other means, with saving authorship by Zerumi and PizhikCoder retained)
namespace AMWE_RealTime_Server.Models
{
    public class User
    {
        public int Id { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public PasswordType PasswordType { get; set; }
    }

    public enum PasswordType
    {
        LegacyAESEncrypted = 1,
        HashedBCrypt = 2
    }
}