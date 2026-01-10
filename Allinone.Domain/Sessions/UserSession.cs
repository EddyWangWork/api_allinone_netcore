using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Allinone.Domain.Sessions
{
    public class UserSession
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int MemberID { get; set; }

        [Required]
        public DateTime LastActivityTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        [MaxLength(100)]
        public string? TokenIdentifier { get; set; }
    }
}
