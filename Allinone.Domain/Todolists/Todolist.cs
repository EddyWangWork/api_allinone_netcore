using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Todolists
{
    public class Todolist
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public string? Description { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;

        public int MemberID { get; set; }
        public Member Member { get; set; }
    }
}
