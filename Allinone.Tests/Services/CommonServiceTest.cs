using Allinone.BLL;
using Allinone.Domain.Enums;

namespace Allinone.Tests.Services
{
    public class CommonServiceTest
    {
        private readonly ICommonService _commonService = new CommonService();

        [Fact]
        public void GetDSTransTypes_Returns_AllTransactionTypes()
        {
            var result = _commonService.GetDSTransTypes();

            Assert.Equal(
                Enum.GetValues<EnumDSTranType>().Select(value => (int)value),
                result.Select(type => type.ID));
            Assert.Equal(
                Enum.GetValues<EnumDSTranType>().Select(value => value.ToString()),
                result.Select(type => type.Name));
        }

        [Fact]
        public void GetTodolistTypes_Returns_AllTodolistTypes()
        {
            var result = _commonService.GetTodolistTypes();

            Assert.Equal(
                Enum.GetValues<EnumTodolistType>().Select(value => (int)value),
                result.Select(type => type.ID));
            Assert.Equal(
                Enum.GetValues<EnumTodolistType>().Select(value => value.ToString()),
                result.Select(type => type.Name));
        }
    }
}
