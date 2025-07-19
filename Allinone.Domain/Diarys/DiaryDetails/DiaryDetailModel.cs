using Allinone.Domain.Diarys.DiaryTypes;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Diarys.DiaryDetails
{
    public class DiaryDetailAddReq
    {
        [Required(ErrorMessage = "DiaryID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "DiaryID must be greater than 0.")]
        public int DiaryID { get; set; }

        [Required(ErrorMessage = "DiaryTypeID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "DiaryTypeID must be greater than 0.")]
        public int DiaryTypeID { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; } = string.Empty;
    }

    public class DiaryDetailDto
    {
        public int ID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UpdateDate { get; set; }

        public int DiaryID { get; set; }
        public int DiaryTypeID { get; set; }

        public string DiaryType { get; set; }
        public DateTime DiaryDate { get; set; }
    }
}
