namespace Allinone.Domain.Todolists
{
    public class TodolistDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
