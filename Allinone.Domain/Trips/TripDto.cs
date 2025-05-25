namespace Allinone.Domain.Trips
{
    public class TripDto
    {
        public DateTime Date { get; set; }
        public TripDetailDto TripDetailDto { get; set; }
    }

    public class TripResultDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<TripDto> TripDtos { get; set; }

        public TripResultDto()
        {
            TripDtos = new List<TripDto>();
        }
    }

    public class TripDetailFlatDto
    {
        public int TripID { get; set; }
        public string TripName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime Date { get; set; }
        public int TripDetailTypeID { get; set; }
        public string TripDetailTypeName { get; set; }
        public int TripDetailID { get; set; }
        public string TripDetailName { get; set; }
        public string TripDetailLink { get; set; }
    }

}
