using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Allinone.Domain.Shops.ShopDiarys
{
    public class ShopDiary
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string? Remark { get; set; }
        public string? Comment { get; set; }

        public int ShopID { get; set; }

        [JsonIgnore] // from System.Text.Json
        public Shop Shop { get; set; }
    }
}
