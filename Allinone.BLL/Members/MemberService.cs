using Allinone.BLL.Auditlogs;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Members;
using Allinone.Helper.JWT;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Members
{
    public class MemberService(
        //IOptions<JwtSettings> jwtSettings,
        IAuditlogService _auditlogService,
        IMemberRepository _memberRepository,
        IMapModel mapper) : BaseBLL, IMemberService
    {
        //private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public async Task<MemberDto> LoginV2(string name, string password)
        {
            var memberDto = new MemberDto();

            var member = await _memberRepository.GetAsync(name, password) ??
                throw new NotFoundException($"Member record not found");

            var token = JWTHelper.GenerateJwtToken(name, member.ID);

            member.Token = token;
            member.LastLoginDate = DateTime.UtcNow.AddHours(8);
            _memberRepository.Update(member);

            memberDto = mapper.MapDto<Member, MemberDto>(member);

            await _auditlogService.LogLoginNew(name, member.ID);

            return memberDto;
        }

        public async Task<MemberDto> Add(string name, string password)
        {
            if (await _memberRepository.IsExist(name)) throw new MemberExistException();

            var newMember = new Member
            {
                Name = name,
                Password = password
            };

            await _memberRepository.Add(newMember);

            var memberDto = mapper.MapDto<Member, MemberDto>(newMember);

            return memberDto;
        }
    }
}
