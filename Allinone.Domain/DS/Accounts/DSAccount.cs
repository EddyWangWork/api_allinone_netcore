using Allinone.Domain.Audits;
using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.DS.Accounts
{
    public class DSAccount : IAuditableMember
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int MemberID { get; set; }

        public Member Member { get; set; }
    }
}
