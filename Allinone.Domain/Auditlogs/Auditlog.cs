using Allinone.Domain.Audits;
using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Auditlogs
{
    public class Auditlog : IMember
    {
        [Key]
        public int ID { get; set; }
        public int TypeID { get; set; }
        public int ActionTypeID { get; set; }

        public string Name { get; set; }
        public DateTime CreatedTime { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? Remark { get; set; }

        public int MemberID { get; set; }
        public Member Member { get; set; }
    }
}
