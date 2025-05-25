namespace Allinone.Domain.DS.Accounts
{
    public class DSAccountAddReq
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

    public class DSAccountDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public bool IsActive { get; set; }
    }
}
