namespace Allinone.Domain.Kanbans
{
    public class KanbanDto
    {
        public int Status { get; set; }
        public List<KanbanDetailDto> KanbanDetails { get; set; } = [];
    }

    public class KanbanDetailDto
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Priority { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
