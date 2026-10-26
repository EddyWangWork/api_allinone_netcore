using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Trips;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Trips
{
    public interface ITripDetailService
    {
        Task<IEnumerable<TripDetail>> GetAllAsync();
        Task<IEnumerable<TripDetail>> GetByTripIdAsync(int tripId);
        Task<TripDetail> Get(int id);
        Task<TripDetail> Add(TripDetailAddReq req);
        Task<TripDetail> Update(int id, TripDetailAddReq req);
        Task<TripDetail> Delete(int id);
    }

    public class TripDetailService(
        ITripRepository tripRepository,
        ITripDetailTypeRepository tripDetailTypeRepository,
        ITripDetailRepository tripDetailRepository,
        IMapModel mapper) : BaseBLL, ITripDetailService
    {
        public async Task<IEnumerable<TripDetail>> GetAllAsync()
        {
            return await tripDetailRepository.GetAllAsync();
        }

        public async Task<IEnumerable<TripDetail>> GetByTripIdAsync(int tripId)
        {
            if (!await tripRepository.IsExist(tripId)) throw new TripBadRequestException();

            return await tripDetailRepository.GetByTripIdAsync(tripId);
        }

        public async Task<TripDetail> Get(int id)
        {
            return await tripDetailRepository.GetAsync(id) ?? throw new TripDetailNotFoundException();
        }

        private async Task ValidateAsync(TripDetailAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            var trip = await tripRepository.GetAsync(req.TripID) ?? throw new TripBadRequestException();
            if (!await tripDetailTypeRepository.IsExist(req.TripDetailTypeID)) throw new TripDetailTypeBadRequestException();

            if (req.Date.Date < trip.FromDate.Date || req.Date.Date > trip.ToDate.Date)
                throw new TripDetailBadRequestException();
        }

        public async Task<TripDetail> Add(TripDetailAddReq req)
        {
            await ValidateAsync(req);

            var entity = mapper.MapDto<TripDetailAddReq, TripDetail>(req);

            await tripDetailRepository.Add(entity);

            return entity;
        }

        public async Task<TripDetail> Update(int id, TripDetailAddReq req)
        {
            await ValidateAsync(req);

            var entity = await tripDetailRepository.GetAsync(id) ?? throw new TripDetailNotFoundException();

            mapper.Map(req, entity);

            tripDetailRepository.Update(entity);

            return entity;
        }

        public async Task<TripDetail> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await tripDetailRepository.GetAsync(id) ?? throw new TripDetailNotFoundException();

            tripDetailRepository.Delete(entity);

            return entity;
        }
    }
}
