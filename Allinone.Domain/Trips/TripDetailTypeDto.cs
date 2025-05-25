namespace Allinone.Domain.Trips
{
    public class TripDetailTypeDto
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public List<TripDetailTypeValueDto> TypeValues { get; set; }

        public TripDetailTypeDto()
        {
            TypeValues = new List<TripDetailTypeValueDto>();
        }
    }

    public class TripDetailTypeValueDto
    {
        public int TypeValueID { get; set; }
        public string TypeValue { get; set; }
        public string TypeVTypeLink { get; set; }
    }
}
