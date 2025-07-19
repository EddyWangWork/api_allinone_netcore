using Allinone.Helper.Cache;

namespace Allinone.BLL
{
    public interface ICommonService
    {
        Task<object> GetDSTransTypes();
        Task<object> GetTodolistTypes();
    }

    public class CommonService(MemoryCacheHelper memoryCacheHelper) : ICommonService
    {
        public async Task<object> GetDSTransTypes()
        {
            return MemoryCacheHelper.CacheDSTranTypeList;
        }

        public async Task<object> GetTodolistTypes()
        {
            return MemoryCacheHelper.CacheTodolistTypeList;
        }
    }
}
