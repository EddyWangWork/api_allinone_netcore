using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Kanbans;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Kanbans
{
    public interface IKanbanService
    {
        Task<IEnumerable<KanbanDto>> GetKanbansAsync();
        Task<Kanban> Get(int id);
        Task<Kanban> Add(KanbanAddReq req);
        Task<Kanban> Update(int id, KanbanAddReq req);
        Task<Kanban> Delete(int id);
    }

    public class KanbanService(
        IKanbanRepository kanbanRepository,
        IMapModel mapper) : BaseBLL, IKanbanService
    {
        public async Task<IEnumerable<KanbanDto>> GetKanbansAsync()
        {
            var kanbans = await kanbanRepository.GetKanbansAsync(MemberId);
            return kanbans
                        .GroupBy(x => x.Status)
                        .Select(group => new KanbanDto
                        {
                            Status = group.Key,
                            KanbanDetails = [.. group.Select(ToDetailDto)]
                        })
                        .ToList();
        }

        private static KanbanDetailDto ToDetailDto(Kanban x) => new()
        {
            ID = x.ID,
            Type = x.Type,
            Title = x.Title,
            Content = x.Content,
            Status = x.Status,
            Priority = x.Priority,
            UpdatedTime = x.UpdatedTime
        };

        public async Task<Kanban> Get(int id)
        {
            return await kanbanRepository.GetByMemberAsync(MemberId, id) ?? throw new KanbanNotFoundException();
        }

        public async Task<Kanban> Add(KanbanAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<KanbanAddReq, Kanban>(req);
            entity = ServiceHelper.SetAuditAddMemberDateFields(entity, MemberId);

            await kanbanRepository.Add(entity);

            return entity;
        }

        public async Task<Kanban> Update(int id, KanbanAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await kanbanRepository.GetByMemberAsync(MemberId, id) ?? throw new KanbanNotFoundException();
            entity = ServiceHelper.SetAuditUpdateDateFields(entity);

            mapper.Map(req, entity);

            await kanbanRepository.Update(entity);

            return entity;
        }

        public async Task<Kanban> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await kanbanRepository.GetByMemberAsync(MemberId, id) ?? throw new KanbanNotFoundException();

            await kanbanRepository.Delete(entity);

            return entity;
        }
    }
}
