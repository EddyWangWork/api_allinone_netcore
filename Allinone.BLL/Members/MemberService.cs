using Allinone.BLL.Auditlogs;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Members;
using Allinone.Domain.Sessions;
using Allinone.Helper.JWT;
using Allinone.Helper.Mapper;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace Allinone.BLL.Members
{
    public class MemberService(
        IAuditlogService _auditlogService,
        IMemberRepository _memberRepository,
        IUserSessionRepository _userSessionRepository,
        IMapModel mapper,
        IConfiguration configuration) : BaseBLL, IMemberService
    {
        public async Task<MemberDto> LoginV2(string name, string password)
        {
            var memberDto = new MemberDto();

            var member = await _memberRepository.GetAsync(name, password) ??
                throw new NotFoundException($"Member record not found");

            var token = JWTHelper.GenerateJwtToken(name, member.ID, configuration, member.Role);

            member.Token = token;
            member.LastLoginDate = DateTime.UtcNow.AddHours(8);
            _memberRepository.Update(member);

            memberDto = mapper.MapDto<Member, MemberDto>(member);

            await _auditlogService.LogLoginNew(name, member.ID);

            // Create or update user session in database
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var tokenIdentifier = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            var expireMinutes = 60;
            if (configuration != null && int.TryParse(configuration["Jwt:ExpireMinutes"], out var configMinutes))
            {
                expireMinutes = configMinutes;
            }
            var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

            var existingSession = await _userSessionRepository.GetByMemberIdAsync(member.ID);

            if (existingSession != null)
            {
                // Update existing session with new token info
                existingSession.TokenIdentifier = tokenIdentifier;
                existingSession.LastActivityTime = DateTime.UtcNow;
                existingSession.ExpiresAt = expiresAt;
                await _userSessionRepository.UpdateAsync(existingSession);
            }
            else
            {
                // Create new session
                var newSession = new UserSession
                {
                    MemberID = member.ID,
                    TokenIdentifier = tokenIdentifier,
                    LastActivityTime = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt
                };
                await _userSessionRepository.CreateAsync(newSession);
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
            var token = JWTHelper.GenerateJwtToken(name, newMember.ID, configuration, newMember.Role);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var tokenIdentifier = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            var expireMinutes = 60;
            if (configuration != null && int.TryParse(configuration["Jwt:ExpireMinutes"], out var configMinutes))
            {
                expireMinutes = configMinutes;
            }
            var expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

            var newSession = new UserSession
            {
                MemberID = newMember.ID,
                TokenIdentifier = tokenIdentifier,
                LastActivityTime = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };
            await _userSessionRepository.CreateAsync(newSession);

            memberDto.Token = token;

            return memberDto;
        }
    }
}
