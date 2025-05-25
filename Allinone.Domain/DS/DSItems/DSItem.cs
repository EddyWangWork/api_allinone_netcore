using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.DS.DSItems
{
    public class DSItem
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int MemberID { get; set; }

        public ICollection<DSItemSub> DSItemSubs { get; set; }
        public Member Member { get; set; }
    }
}
