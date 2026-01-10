using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Members
{
    public class Member
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string? Token { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Role { get; set; } = "User"; // User, Admin
        public bool IsActive { get; set; } = true;
    }
}
