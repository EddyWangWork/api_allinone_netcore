using Allinone.Domain.Enums;

namespace Allinone.BLL
{
    public interface ICommonService
    {
        IReadOnlyList<EnumModel> GetDSTransTypes();
        IReadOnlyList<EnumModel> GetTodolistTypes();
    }

    public class CommonService : ICommonService
    {
        private static readonly IReadOnlyList<EnumModel> DSTransTypes =
            CreateEnumModels<EnumDSTranType>();

        private static readonly IReadOnlyList<EnumModel> TodolistTypes =
            CreateEnumModels<EnumTodolistType>();

        public IReadOnlyList<EnumModel> GetDSTransTypes() => DSTransTypes;

        public IReadOnlyList<EnumModel> GetTodolistTypes() => TodolistTypes;

        private static IReadOnlyList<EnumModel> CreateEnumModels<TEnum>()
            where TEnum : struct, Enum =>
            Enum.GetValues<TEnum>()
                .Select(value => new EnumModel
                {
                    ID = Convert.ToInt32(value),
                    Name = value.ToString()
                })
                .ToArray();
    }
}
