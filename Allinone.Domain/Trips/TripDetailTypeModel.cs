using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Trips
{
    public class TripDetailTypeAddReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}
