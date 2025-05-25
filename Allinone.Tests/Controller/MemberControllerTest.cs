using Allinone.API.Controllers;
using Allinone.API.Filters;
using Allinone.BLL.Members;
using Allinone.DLL.Data;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Members;
using Allinone.Helper.Mapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Controller
{
    public class MemberControllerTest
    {
        private readonly MemberController _memberController;

        private readonly int _id = 1;
        private readonly string _name = "user";
        private readonly string _password = "user";
        private readonly string _password2 = "user2";

        public MemberControllerTest()
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

            services.AddScoped<ValidateModelFilter>();

            var mapModel = services.BuildServiceProvider().GetRequiredService<IMapModel>();

            var memberRepository = new MemberRepository(context);

            var memberService = new MemberService(memberRepository, mapModel);

            _memberController = new MemberController(memberService);
        }

        [Fact]
        public async Task Register_ReturnsSuccess()
        {
            // Arrange
            var input = new MemberLoginReq { Name = "newUser", Password = "newUser" };

            // Act
            var result = await _memberController.Register(input);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal("newUser", ((MemberDto)okResult.Value).Name);
        }

        [Fact]
        public async Task Register_ReturnsMemberExist()
        {
            // Arrange
            var input = new MemberLoginReq { Name = _name, Password = _password };

            // Act & Assert
            await Assert.ThrowsAsync<MemberExistException>(async () =>
            {
                await _memberController.Register(input);
            });
        }

        [Fact]
        public async Task MemberLoginV2_ReturnsSuccess()
        {
            // Arrange
            var req = new MemberLoginReq { Name = _name, Password = _password };

            // Act
            var result = await _memberController.LoginV2(req);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(_name, ((MemberDto)okResult.Value).Name);
        }

        [Fact]
        public async Task MemberLoginV2_ReturnsNotFound()
        {
            // Arrange
            var req = new MemberLoginReq { Name = _name, Password = _password2 };

            // Assert
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await _memberController.LoginV2(req);
            });
        }

        //[Fact]
        //public async Task MemberLoginV2_ReturnsCreatedAtAction()
        //{
        //    // Arrange
        //    var mockService = new Mock<IMemberService>();
        //    var input = new MemberLoginReq { Name = "miko", Password = "miko" };
        //    var created = new MemberDto { Name = "miko" };

        //    mockService.Setup(s => s.LoginV2(input.Name, input.Password)).ReturnsAsync(created);
        //    var controller = new MemberController(mockService.Object);

        //    // Act
        //    var result = await controller.LoginV2(input);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(200, okResult.StatusCode);

        //    Assert.Equal("miko", ((MemberDto)okResult.Value).Name);
        //}
    }
}
