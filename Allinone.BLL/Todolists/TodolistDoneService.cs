using Allinone.BLL.Auditlogs;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Todolists;
using Allinone.Helper.Mapper;
using Newtonsoft.Json;

namespace Allinone.BLL.Todolists
{
    public interface ITodolistDoneService
    {
        Task<List<TodolistDoneDto>> GetAsync();

        Task<TodolistDone> GetByIdAsync(int id);
        Task<TodolistDone> Add(TodolistDoneAddReq req);
        Task<TodolistDone> Update(int id, TodolistDoneUpdateReq req);
        Task<TodolistDone> Delete(int id);
    }

    public class TodolistDoneService(
        IAuditlogService _auditlogService,
        ITodolistDoneRepository todolistDoneRepository,
        IMapModel mapper) : BaseBLL, ITodolistDoneService
    {
        public async Task<List<TodolistDoneDto>> GetAsync() =>
            await todolistDoneRepository.GetAllAsync(RequireMemberId());

        public async Task<TodolistDone> GetByIdAsync(int id) =>
            await FindOwnedAsync(id, RequireMemberId());

        public async Task<TodolistDone> Add(TodolistDoneAddReq req)
        {
            var memberId = RequireMemberId();

            var entity = mapper.MapDto<TodolistDoneAddReq, TodolistDone>(req);
            var result = await todolistDoneRepository.AddAsync(entity, memberId);

            await _auditlogService.LogTodolistDoneNew(
                result.Todolist.Name, memberId, JsonConvert.SerializeObject(result));
            return result;
        }

        public async Task<TodolistDone> Update(int id, TodolistDoneUpdateReq req)
        {
            var memberId = RequireMemberId();
            var entity = await FindOwnedAsync(id, memberId);

            mapper.Map(req, entity);
            await todolistDoneRepository.UpdateAsync(entity);

            await _auditlogService.LogTodolistDoneUpdate(
                entity.Todolist.Name, memberId, JsonConvert.SerializeObject(entity), JsonConvert.SerializeObject(req));
            return entity;
        }

        public async Task<TodolistDone> Delete(int id)
        {
            var memberId = RequireMemberId();
            var entity = await FindOwnedAsync(id, memberId);

            await todolistDoneRepository.DeleteAsync(entity);
            await _auditlogService.LogTodolistDoneDelete(
                entity.Todolist.Name, memberId, JsonConvert.SerializeObject(entity));
            return entity;
        }

        private static int RequireMemberId() =>
            MemberId == 0 ? throw new MemberNotFoundException() : MemberId;

        private async Task<TodolistDone> FindOwnedAsync(int id, int memberId) =>
            await todolistDoneRepository.GetByIdAsync(id, memberId) ?? throw new TodolistDoneNotFoundException();
    }
}
