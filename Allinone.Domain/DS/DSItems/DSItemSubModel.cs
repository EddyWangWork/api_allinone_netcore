namespace Allinone.Domain.DS.DSItems
{
    public class DSItemSubAddReq
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public int DSItemID { get; set; }
    }

    public class DSItemSubDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public int DSItemID { get; set; }
        public string DSItemName { get; set; }

    }
}
