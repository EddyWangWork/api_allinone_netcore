namespace Allinone.Domain.Todolists
{
    public class TodolistDoneDto
    {
        public int ID { get; set; }
        public string Remark { get; set; }
        public DateTime UpdateDate { get; set; }
        public int TodolistID { get; set; }
        public string TodolistName { get; set; }
        public string TodolistDescription { get; set; }
        public string TodolistCategory { get; set; }
    }
}
