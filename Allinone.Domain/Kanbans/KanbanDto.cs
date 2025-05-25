namespace Allinone.Domain.Kanbans
{
    public class KanbanDto
    {
        public int Status { get; set; }
        public List<Kanban> KanbanDetails { get; set; }
    }

    public class KanbanDetailDto
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
