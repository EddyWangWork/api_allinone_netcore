using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Trips;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Trips
{
    public interface ITripDetailTypeService
    {
        Task<IEnumerable<TripDetailType>> GetAllAsync();
        Task<TripDetailType> Get(int id);
        Task<TripDetailType> Add(TripDetailTypeAddReq req);
        Task<TripDetailType> Update(int id, TripDetailTypeAddReq req);
        Task<TripDetailType> Delete(int id);
    }

    public class TripDetailTypeService(
        ITripDetailTypeRepository tripDetailTypeRepository,
        IMapModel mapper) : BaseBLL, ITripDetailTypeService
    {
        public async Task<IEnumerable<TripDetailType>> GetAllAsync()
        {
            return await tripDetailTypeRepository.GetAllAsync();
        }

        public async Task<TripDetailType> Get(int id)
        {
            return await tripDetailTypeRepository.GetAsync(id) ?? throw new TripDetailTypeNotFoundException();
        }

        public async Task<TripDetailType> Add(TripDetailTypeAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<TripDetailTypeAddReq, TripDetailType>(req);

            await tripDetailTypeRepository.Add(entity);

            return entity;
        }

        public async Task<TripDetailType> Update(int id, TripDetailTypeAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await tripDetailTypeRepository.GetAsync(id) ?? throw new TripDetailTypeNotFoundException();

            mapper.Map(req, entity);

            tripDetailTypeRepository.Update(entity);

            return entity;
        }

        public async Task<TripDetailType> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await tripDetailTypeRepository.GetAsync(id) ?? throw new TripDetailTypeNotFoundException();

            tripDetailTypeRepository.Delete(entity);

            return entity;
        }
    }
}
