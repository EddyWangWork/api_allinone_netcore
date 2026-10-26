using Allinone.BLL.Auditlogs;
using Allinone.DLL.Repositories;
using Allinone.Domain;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Members;
using Allinone.Domain.Sessions;
using Allinone.Helper.JWT;
using Allinone.Helper.Mapper;
using Microsoft.Extensions.Options;

namespace Allinone.BLL.Members
{
    public class MemberService(
        IAuditlogService _auditlogService,
        IMemberRepository _memberRepository,
        IUserSessionRepository _userSessionRepository,
        IMapModel mapper,
        IOptions<JwtSettings> jwtOptions) : BaseBLL, IMemberService
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public async Task<MemberDto> LoginV2(string name, string password)
        {
            var member = await _memberRepository.GetAsync(name, password) ??
                throw new NotFoundException($"Member record not found");

            var jwt = JWTHelper.GenerateJwtToken(name, member.ID, member.Role, _jwtSettings);

            member.Token = jwt.Token;
            member.LastLoginDate = DateTime.UtcNow.AddHours(8);
            _memberRepository.Update(member);

            var memberDto = mapper.MapDto<Member, MemberDto>(member);

            await _auditlogService.LogLoginNew(name, member.ID);

            var existingSession = await _userSessionRepository.GetByMemberIdAsync(member.ID);

            if (existingSession != null)
            {
                existingSession.TokenIdentifier = jwt.TokenIdentifier;
                existingSession.LastActivityTime = DateTime.UtcNow;
                existingSession.ExpiresAt = jwt.ExpiresAt;
                await _userSessionRepository.UpdateAsync(existingSession);
            }
            else
            {
                await _userSessionRepository.CreateAsync(NewSession(member.ID, jwt));
            }

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

            // Create session for new user with auto-login
            var jwt = JWTHelper.GenerateJwtToken(name, newMember.ID, newMember.Role, _jwtSettings);
            await _userSessionRepository.CreateAsync(NewSession(newMember.ID, jwt));

            memberDto.Token = jwt.Token;

            return memberDto;
        }

        private static UserSession NewSession(int memberId, JwtTokenResult jwt) => new()
        {
            MemberID = memberId,
            TokenIdentifier = jwt.TokenIdentifier,
            LastActivityTime = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = jwt.ExpiresAt
        };
    }
}
