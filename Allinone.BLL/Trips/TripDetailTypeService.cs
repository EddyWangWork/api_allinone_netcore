using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Trips;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Trips
{
    public interface ITripDetailTypeService
    {
        Task<IEnumerable<TripDetailType>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TripDetailType> Get(int id, CancellationToken cancellationToken = default);
        Task<TripDetailType> Add(TripDetailTypeAddReq req, CancellationToken cancellationToken = default);
        Task<TripDetailType> Update(
            int id,
            TripDetailTypeAddReq req,
            CancellationToken cancellationToken = default);
        Task<TripDetailType> Delete(int id, CancellationToken cancellationToken = default);
    }

    public class TripDetailTypeService(
        ITripDetailTypeRepository tripDetailTypeRepository,
        IMapModel mapper) : BaseBLL, ITripDetailTypeService
    {
        public async Task<IEnumerable<TripDetailType>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await tripDetailTypeRepository.GetAllAsync(cancellationToken);
        }

        public async Task<TripDetailType> Get(int id, CancellationToken cancellationToken = default)
        {
            return await tripDetailTypeRepository.GetAsync(id, cancellationToken)
                ?? throw new TripDetailTypeNotFoundException();
        }

        public async Task<TripDetailType> Add(
            TripDetailTypeAddReq req,
            CancellationToken cancellationToken = default)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<TripDetailTypeAddReq, TripDetailType>(req);

            await tripDetailTypeRepository.Add(entity, cancellationToken);

            return entity;
        }

        public async Task<TripDetailType> Update(
            int id,
            TripDetailTypeAddReq req,
            CancellationToken cancellationToken = default)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await tripDetailTypeRepository.GetAsync(id, cancellationToken)
                ?? throw new TripDetailTypeNotFoundException();

            mapper.Map(req, entity);

            await tripDetailTypeRepository.Update(entity, cancellationToken);

            return entity;
        }

        public async Task<TripDetailType> Delete(int id, CancellationToken cancellationToken = default)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await tripDetailTypeRepository.GetAsync(id, cancellationToken)
                ?? throw new TripDetailTypeNotFoundException();

            await tripDetailTypeRepository.Delete(entity, cancellationToken);

            return entity;
        }
    }
}
