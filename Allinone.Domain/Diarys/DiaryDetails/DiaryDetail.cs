using Allinone.Domain.Diarys.DiaryTypes;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Diarys.DiaryDetails
{
    public class DiaryDetail : IID
    {
        [Key]
        public int ID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime UpdateDate { get; set; } = DateTime.Now;

        public int DiaryID { get; set; }
        public int DiaryTypeID { get; set; }

        public Diary Diary { get; set; }
        public DiaryType DiaryType { get; set; }
    }
}
