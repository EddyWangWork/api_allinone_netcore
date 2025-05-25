using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Shops.ShopDiarys;
using Allinone.Domain.Trips;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Trips
{
    public interface ITripService
    {
        Task<IEnumerable<TripResultDto>> GetAllDetailsV2Async();

        Task<IEnumerable<Trip>> GetAllAsync();
        Task<Trip> Get(int id);
        Task<Trip> Add(TripAddReq req);
        Task<Trip> Update(int id, TripAddReq req);
        Task<Trip> Delete(int id);
    }

    public class TripService(
        ITripRepository tripRepository,
        IMapModel mapper) : BaseBLL, ITripService
    {
        public async Task<IEnumerable<TripResultDto>> GetAllDetailsV2Async()
        {
            return await tripRepository.GetAllDetailsV2Async();
        }


        public async Task<IEnumerable<Trip>> GetAllAsync()
        {
            return await tripRepository.GetAllAsync();
        }

        public async Task<Trip> Get(int id)
        {
            return await tripRepository.GetAsync(id) ?? throw new TripNotFoundException();
        }

        public async Task<Trip> Add(TripAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<TripAddReq, Trip>(req);

            await tripRepository.Add(entity);

            return entity;
        }

        public async Task<Trip> Update(int id, TripAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await tripRepository.GetAsync(id) ?? throw new TripNotFoundException();

            mapper.Map(req, entity);

            tripRepository.Update(entity);

            return entity;
        }

        public async Task<Trip> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await tripRepository.GetAsync(id) ?? throw new TripNotFoundException();

            tripRepository.Delete(entity);

            return entity;
        }
    }
}
