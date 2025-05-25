using Allinone.BLL;
using Allinone.BLL.DS.Accounts;
using Allinone.Helper.Cache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.Tests.Services
{
    public class CommonServiceTest
    {
        private readonly CommonService _commonService;


        public CommonServiceTest()
        {
            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            var memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();

            _commonService = new CommonService(memoryCacheHelper);
        }

        [Fact]
        public async Task GetDSTransTypes_Returns_Success()
        {
            // Act
            var result = await _commonService.GetDSTransTypes();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetTodolistTypes_Returns_Success()
        {
            // Act
            var result = await _commonService.GetTodolistTypes();

            // Assert
            Assert.NotNull(result);
        }
    }
}
