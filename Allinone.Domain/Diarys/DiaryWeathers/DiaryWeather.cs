using Allinone.Domain.Members;
using System.ComponentModel.DataAnnotations;

namespace Allinone.Domain.Diarys.DiaryWeathers
{
    public class DiaryWeather : IMember
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int MemberID { get; set; }
        public Member Member { get; set; }
    }
}
