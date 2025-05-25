namespace Allinone.Domain.Shops.ShopDiarys
{
    public class ShopDiaryDto
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string? Remark { get; set; }
        public string? Comment { get; set; }

        public int ShopID { get; set; }
        public string ShopName { get; set; }
    }
}
