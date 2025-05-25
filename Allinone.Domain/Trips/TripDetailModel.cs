using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Trips
{
    public class TripDetailAddReq
    {
        [Required(ErrorMessage = "TripID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "TripID must be greater than 0.")]
        public int TripID { get; set; }

        [Required(ErrorMessage = "TripDetailTypeID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "TripDetailTypeID must be greater than 0.")]
        public int TripDetailTypeID { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [NotMinDate(ErrorMessage = "Date is invalid")]
        public DateTime Date { get; set; }

        public string Name { get; set; }

        public string? LinkName { get; set; } = string.Empty;
    }
}
