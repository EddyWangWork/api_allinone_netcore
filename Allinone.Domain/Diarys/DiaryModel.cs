using Allinone.Domain.Diarys.DiaryDetails;
using Allinone.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Diarys
{
    public class DiaryAddReq
    {
        [Required(ErrorMessage = "Date is required")]
        [NotMinDate(ErrorMessage = "Date is invalid")]
        public DateTime? Date { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public List<int>? Activitys { get; set; }
        public List<int>? Emotions { get; set; }
        public List<int>? Foods { get; set; }
        public List<int>? Locations { get; set; }
        public List<int>? Books { get; set; }
        public List<int>? Weathers { get; set; }
    }

    public class DiaryDto
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<EnumModel> Activitys { get; set; }
        public List<EnumModel> Emotions { get; set; }
        public List<EnumModel> Foods { get; set; }
        public List<EnumModel> Locations { get; set; }
        public List<EnumModel> Books { get; set; }
        public List<EnumModel> Weathers { get; set; }
        public List<DiaryDetailDto> DiaryDetailDtos { get; set; } = new List<DiaryDetailDto>();
    }

    public class DiaryInfoDto
    {
        public List<EnumModel> Activitys { get; set; }
        public List<EnumModel> Emotions { get; set; }
        public List<EnumModel> Foods { get; set; }
        public List<EnumModel> Locations { get; set; }
        public List<EnumModel> Books { get; set; }
        public List<EnumModel> Weathers { get; set; }
    }
}
