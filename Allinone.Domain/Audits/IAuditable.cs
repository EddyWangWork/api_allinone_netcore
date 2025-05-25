namespace Allinone.Domain.Audits
{
    public interface IAuditableMember
    {
        int MemberID { get; set; }
    }

    public interface IAuditableUpdatedTime
    {
        int MemberID { get; set; }
        DateTime UpdatedTime { get; set; }
    }

    public interface IAuditableMemberUpdatedTime
    {
        int MemberID { get; set; }
        DateTime UpdatedTime { get; set; }
    }
}
