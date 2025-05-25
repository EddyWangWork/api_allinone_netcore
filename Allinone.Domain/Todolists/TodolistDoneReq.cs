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
        private static double _unixUpdateTime;
        public double UnixUpdateTime
        {
            get => _unixUpdateTime;
            set => _unixUpdateTime = value != 0 ? value : (DateTime.UtcNow.AddHours(8) - DateTime.UnixEpoch).TotalSeconds;
        }

        public DateTime UpdateDate => DateTimeHelper.UnixToDateTimeMSec(_unixUpdateTime);

        public string? Remark { get; set; }
    }
}
