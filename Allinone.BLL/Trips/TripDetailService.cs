using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Trips;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Trips
{
    public interface ITripDetailService
    {
        Task<IEnumerable<TripDetail>> GetAllAsync();
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

        public async Task<TripDetail> Get(int id)
        {
            return await tripDetailRepository.GetAsync(id) ?? throw new TripDetailNotFoundException();
        }

        public async Task<TripDetail> Add(TripDetailAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            if (!await tripRepository.IsExist(req.TripID)) throw new TripBadRequestException();
            if (!await tripDetailTypeRepository.IsExist(req.TripDetailTypeID)) throw new TripDetailTypeBadRequestException();

            var entity = mapper.MapDto<TripDetailAddReq, TripDetail>(req);

            await tripDetailRepository.Add(entity);

            return entity;
        }

        public async Task<TripDetail> Update(int id, TripDetailAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();
            if (!await tripRepository.IsExist(req.TripID)) throw new TripBadRequestException();
            if (!await tripDetailTypeRepository.IsExist(req.TripDetailTypeID)) throw new TripDetailTypeBadRequestException();

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
