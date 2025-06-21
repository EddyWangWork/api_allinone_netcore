namespace Allinone.Domain
{
    public interface IMember
    {
        public int ID { get; set; }

        public int MemberID { get; set; }
    }

    public interface IMemberItem
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int MemberID { get; set; }
    }
}
