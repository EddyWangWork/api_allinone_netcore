using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.DS.Transactions
{
    public class DSTransaction
    {
        [Key]
        public int ID { get; set; }
        public int DSTypeID { get; set; }
        public int DSAccountID { get; set; }
        public int DSTransferOutID { get; set; }
        public int DSItemID { get; set; }
        public int DSItemSubID { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public int MemberID { get; set; }
        public Member Member { get; set; }
    }
}
