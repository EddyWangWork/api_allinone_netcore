using Allinone.Domain.Diarys.DiaryDetails;
using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Diarys
{
    public class Diary : IMember
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ActivityIDs { get; set; }
        public string? EmotionIDs { get; set; }
        public string? FoodIDs { get; set; }
        public string? LocationIDs { get; set; }
        public string? BookIDs { get; set; }
        public string? WeatherIDs { get; set; }

        public ICollection<DiaryDetail> DiaryDetails { get; set; }

        public int MemberID { get; set; }
        public Member Member { get; set; }
    }
}
