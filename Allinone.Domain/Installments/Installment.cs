using Allinone.Domain.Audits;
using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Installments
{
    public class Installment : IAuditableMemberUpdatedTime
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalMonths { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
        public int MemberID { get; set; }
        public DateTime UpdatedTime { get; set; }

        public Member Member { get; set; }
    }
}
