using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.DS.DSItems
{
    public class DSItemSub
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int DSItemID { get; set; }

        public DSItem DSItem { get; set; }
    }
}
