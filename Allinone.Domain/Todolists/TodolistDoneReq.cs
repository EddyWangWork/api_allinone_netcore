using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Todolists
{
    public class TodolistDoneAddReq : TodolistDoneUpdateReq
    {
        [Required(ErrorMessage = "TodolistID is required")]
        public int? TodolistID { get; set; }
    }

    public class TodolistDoneUpdateReq
    {
        public double UnixUpdateTime { get; set; }

        // 0 falls back to the current time (UTC+8) inside the helper
        public DateTime UpdateDate => DateTimeHelper.UnixToDateTimeMSec(UnixUpdateTime);

        public string? Remark { get; set; }
    }
}
