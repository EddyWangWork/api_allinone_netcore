using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Shops
{
    public class ShopAddReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "TypeList is required")]
        public List<int>? TypeList { get; set; }

        public string? Remark { get; set; }
        public string? Comment { get; set; }
        public int Star { get; set; }
        public bool IsVisited { get; set; }
    }
}
