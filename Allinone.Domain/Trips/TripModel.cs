using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Trips
{
    public class TripAddReq
    {
        public string Name { get; set; }

        [Required]
        [DateEarlierThan("ToDate", ErrorMessage = "FromDate must be earlier than ToDate.")]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
    }
}
