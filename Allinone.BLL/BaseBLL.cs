namespace Allinone.BLL
{
    public class BaseBLL
    {
        // AsyncLocal gives each request its own value, so concurrent users cannot see each other's id
        private static readonly AsyncLocal<int> _memberId = new();

        public static int MemberId
        {
            get => _memberId.Value;
            set => _memberId.Value = value;
        }
    }
}
