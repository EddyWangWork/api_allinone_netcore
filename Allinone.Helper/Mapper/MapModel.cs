using AutoMapper;

namespace Allinone.Helper.Mapper
{
    public interface IMapModel
    {
        TDestination MapDto<TSource, TDestination>(TSource model);
        TDestination MapDto<TDestination>(object model);
        void Map<TSource, TDestination>(TSource modelS, TDestination modelD);
    }

    public class MapModel(IMapper mapper) : IMapModel
    {
        // Generic MapDto method
        public TDestination MapDto<TSource, TDestination>(TSource model)
        {
            return mapper.Map<TDestination>(model);
        }

        public TDestination MapDto<TDestination>(object model)
        {
            return mapper.Map<TDestination>(model);
        }

        public void Map<TSource, TDestination>(TSource modelS, TDestination modelD)
        {
            mapper.Map(modelS, modelD);
        }
    }
}
