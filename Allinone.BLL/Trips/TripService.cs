using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
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
        public Task<IEnumerable<TripResultDto>> GetAllDetailsV2Async() =>
            tripRepository.GetAllDetailsV2Async();

        public Task<IEnumerable<Trip>> GetAllAsync() =>
            tripRepository.GetAllAsync();

        public async Task<Trip> Get(int id) =>
            await tripRepository.GetAsync(id) ?? throw new TripNotFoundException();

        public async Task<Trip> Add(TripAddReq req)
        {
            EnsureMember();

            var entity = mapper.MapDto<TripAddReq, Trip>(req);
            await tripRepository.Add(entity);

            return entity;
        }

        public async Task<Trip> Update(int id, TripAddReq req)
        {
            EnsureMember();

            var entity = await Get(id);
            mapper.Map(req, entity);
            await tripRepository.Update(entity);

            return entity;
        }

        public async Task<Trip> Delete(int id)
        {
            EnsureMember();

            var entity = await Get(id);
            await tripRepository.Delete(entity);

            return entity;
        }

        private static void EnsureMember()
        {
            if (MemberId == 0) throw new MemberNotFoundException();
        }
    }
}
