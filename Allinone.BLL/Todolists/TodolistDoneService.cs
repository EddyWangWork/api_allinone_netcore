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
        public async Task<List<TodolistDoneDto>> GetAsync()
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            return await todolistDoneRepository.GetAllAsync(MemberId);
        }

        public async Task<TodolistDone> GetByIdAsync(int id)
        {
            return await todolistDoneRepository.GetByIdAsync(id) ?? throw new TodolistDoneNotFoundException();
        }

        public async Task<TodolistDone> Add(TodolistDoneAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<TodolistDoneAddReq, TodolistDone>(req);

            var result = await todolistDoneRepository.Add(entity);
            await _auditlogService.LogTodolistDoneNew(
                entity.Todolist.Name, MemberId, JsonConvert.SerializeObject(entity));
            return result;
        }

        public async Task<TodolistDone> Update(int id, TodolistDoneUpdateReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await todolistDoneRepository.GetByIdAsync(id) ?? throw new TodolistDoneNotFoundException();

            mapper.Map(req, entity);

            todolistDoneRepository.Update(entity);

            await _auditlogService.LogTodolistDoneUpdate(
                entity.Todolist.Name, MemberId, JsonConvert.SerializeObject(entity), JsonConvert.SerializeObject(req));
            return entity;
        }

        public async Task<TodolistDone> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await todolistDoneRepository.GetByIdAsync(id) ?? throw new TodolistDoneNotFoundException();

            todolistDoneRepository.Delete(entity);
            await _auditlogService.LogTodolistDoneDelete(
                entity.Todolist.Name, MemberId, JsonConvert.SerializeObject(entity));
            return entity;
        }
    }
}
