namespace Allinone.Domain.Shops
{
    public class ShopDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<string> TypeList { get; set; }
        public string Location { get; set; }
        public string? Remark { get; set; }
        public string? Comment { get; set; }
        public int Star { get; set; }
        public bool IsVisited { get; set; }
        public int VisitedCount { get; set; }
    }
}
