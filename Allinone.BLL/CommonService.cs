using Allinone.Domain.Enums;
using Allinone.Helper.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
