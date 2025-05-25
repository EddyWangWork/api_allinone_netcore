using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Shops.ShopDiarys
{
    public class ShopDiaryAddReq
    {
        [Required(ErrorMessage = "Date is required")]
        [NotMinDate(ErrorMessage = "Date is invalid")]
        public DateTime? Date { get; set; }

        public string? Remark { get; set; }
        public string? Comment { get; set; }

        [Required(ErrorMessage = "ShopID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ShopID must be greater than 0.")]
        public int ShopID { get; set; }
    }
}
