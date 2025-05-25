using Allinone.Domain.Members;

namespace Allinone.BLL.Members
{
    public interface IMemberService
    {
        Task<MemberDto> LoginV2(string name, string password);
        Task<MemberDto> Add(string name, string password);
    }
}
