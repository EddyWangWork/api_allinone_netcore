using Allinone.Domain.DS.Accounts;
using Allinone.Domain.DS.DSItems;
using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Allinone.Domain.DS.Transactions
{
    public class DSTransaction
    {
        [Key]
        public int ID { get; set; }
        public int DSTypeID { get; set; }
        public int DSAccountID { get; set; }
        public int? DSItemID { get; set; }
        public int? DSItemSubID { get; set; }

        // Set only on the TransferIn row; points to its TransferOut row.
        public int? TransferOutTransactionID { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public int MemberID { get; set; }

        [JsonIgnore] public Member? Member { get; set; }
        [JsonIgnore] public DSType? DSType { get; set; }
        [JsonIgnore] public DSAccount? DSAccount { get; set; }
        [JsonIgnore] public DSItem? DSItem { get; set; }
        [JsonIgnore] public DSItemSub? DSItemSub { get; set; }
        [JsonIgnore] public DSTransaction? TransferOutTransaction { get; set; }
    }
}
