namespace Allinone.Domain.TokenBlacklist
{
    public class TokenBlacklist
    {
        public int ID { get; set; }
        public string TokenIdentifier { get; set; } = string.Empty;
        public DateTime BlacklistedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int? MemberID { get; set; }
    }
}
