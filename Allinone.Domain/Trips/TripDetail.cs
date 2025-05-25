namespace Allinone.Domain.Trips
{
    public class TripDetail
    {
        public int ID { get; set; }
        public int TripID { get; set; }
        public int TripDetailTypeID { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string? LinkName { get; set; }

        public Trip Trip { get; set; }
        public TripDetailType TripDetailType { get; set; }
    }
}
