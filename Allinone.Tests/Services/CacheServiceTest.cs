using Allinone.Helper.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Allinone.Tests.Services
{
    public class CacheServiceTest
    {
        private MemoryCacheHelper _memoryCacheHelper;

        public CacheServiceTest()
        {
            var services = new ServiceCollection();

            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheHelper>();

            _memoryCacheHelper = services.BuildServiceProvider().GetRequiredService<MemoryCacheHelper>();
        }

        [Fact]
        public async Task GetTodolistTypeList_ReturnsSuccess()
        {
            //Assign
            var cacheTodolistTypeList = MemoryCacheHelper.CacheTodolistTypeList;

            // Assert
            Assert.NotNull(cacheTodolistTypeList);
        }

        [Fact]
        public async Task GetDSTranTypeList_ReturnsSuccess()
        {
            //Assign
            var cacheDSTranTypeList = MemoryCacheHelper.CacheDSTranTypeList;

            // Assert
            Assert.NotNull(cacheDSTranTypeList);
        }

        [Fact]
        public async Task GetTodolistTypes_ReturnsSuccess()
        {
            //Assign
            var cacheTodolistType = MemoryCacheHelper.CacheTodolistType;

            // Assert
            Assert.NotNull(cacheTodolistType);
            Assert.Contains("Monthly", cacheTodolistType[1]);
        }

        [Fact]
        public async Task GetDSTranTypes_ReturnsSuccess()
        {
            //Assign
            var cacheDSTranType = MemoryCacheHelper.CacheDSTranType;

            // Assert
            Assert.NotNull(cacheDSTranType);
            Assert.Contains("Income", cacheDSTranType[1]);
        }

        [Fact]
        public async Task GetCacheKanbanTypeList_ReturnsSuccess()
        {
            //Assign
            var cacheKanbanTypeList = MemoryCacheHelper.CacheKanbanTypeList;

            // Assert
            Assert.NotNull(cacheKanbanTypeList);
            Assert.Contains("Normal", cacheKanbanTypeList.FirstOrDefault().Name);
        }

        [Fact]
        public async Task GetCacheKanbanStatusList_ReturnsSuccess()
        {
            //Assign
            var cacheKanbanStatusList = MemoryCacheHelper.CacheKanbanStatusList;

            // Assert
            Assert.NotNull(cacheKanbanStatusList);
            Assert.Contains("Todo", cacheKanbanStatusList.FirstOrDefault().Name);
        }
    }
}
