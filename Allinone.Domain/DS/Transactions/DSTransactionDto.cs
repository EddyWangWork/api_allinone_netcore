namespace Allinone.Domain.DS.Transactions
{
    public class DSTransactionDto
    {
        public int ID { get; set; }
        public int DSTypeID { get; set; }
        public int DSAccountID { get; set; }
        public int DSItemID { get; set; }
        public int DSItemSubID { get; set; }
        public int TransferOutTransactionID { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string DSTypeName { get; set; }
        public string DSAccountName { get; set; }
        public string DSItemName { get; set; }
        public string DSItemNameMain { get; set; }
        public string DSItemNameSub { get; set; }
        public DateTime CreatedDateTimeYearMonth { get; set; }
    }

    public class DSTransactionDtoV2
    {
        public int RowID { get; set; }
        public int ID { get; set; }
        public int DSTypeID { get; set; }
        public string DSTypeName { get; set; }
        public int DSAccountID { get; set; }
        public string DSAccountName { get; set; }
        public int DSAccountToID { get; set; }
        public int TransferOutTransactionID { get; set; }
        public string DSItemName { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public decimal Balance { get; set; }
    }
}
