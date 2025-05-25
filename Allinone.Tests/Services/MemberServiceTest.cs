using Allinone.BLL.Members;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Members;
using Allinone.Helper.Mapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class MemberServiceTest
    {
        private readonly MemberService _memberService;

        private readonly int _id = 1;
        private readonly string _name = "user";
        private readonly string _password = "user";
        private readonly string _password2 = "user2";

        public MemberServiceTest()
        {
            var services = new ServiceCollection();

            // Setup InMemory DbContext with preset data
            var options = new DbContextOptionsBuilder<DSContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new DSContext(options);

            context.Member.AddRange(
                new Member { ID = _id, Name = _name, Password = _password }
            );
            context.SaveChanges();

            // Register Dependencies
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddTransient<IMapModel, MapModel>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();

            var memberRepository = new MemberRepository(context);

            _memberService = new MemberService(memberRepository, mapModel);
        }

        [Fact]
        public async Task Add_Returns_Success()
        {
            // Act
            var result = await _memberService.Add("newUser", "newUser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newUser", result!.Name);
        }

        [Fact]
        public async Task Add_Returns_MemberExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<MemberExistException>(async () =>
            {
                await _memberService.Add(_name, _password2);
            });
        }

        [Fact]
        public async Task Login_Returns_Success()
        {
            // Act
            var result = await _memberService.LoginV2(_name, _password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user", result!.Name);
        }

        [Fact]
        public async Task Login_Returns_MemberExist()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _memberService.LoginV2(_name, _password2);
            });
        }
    }
}
