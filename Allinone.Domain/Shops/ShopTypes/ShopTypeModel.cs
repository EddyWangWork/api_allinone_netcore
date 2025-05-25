using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Shops.ShopTypes
{
    public class ShopTypeAddReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }
    }
}
