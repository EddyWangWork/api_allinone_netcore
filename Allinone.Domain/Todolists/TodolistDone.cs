using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Todolists
{
    public class TodolistDone
    {
        [Key]
        public int ID { get; set; }
        public DateTime UpdateDate { get; set; }
        public string? Remark { get; set; }

        public int TodolistID { get; set; }
        public Todolist Todolist { get; set; }
    }
}
