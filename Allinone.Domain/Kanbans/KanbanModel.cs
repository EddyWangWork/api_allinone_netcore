using Allinone.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Kanbans
{
    public class KanbanAddReq
    {
        [EnumDataType(typeof(EnumKanbanType), ErrorMessage = "KanbanType invalid")]
        public int Type { get; set; }

        [EnumDataType(typeof(EnumKanbanStatus), ErrorMessage = "KanbanStatus invalid")]
        public int Status { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        public string? Content { get; set; }

        public int Priority { get; set; }
    }
}
