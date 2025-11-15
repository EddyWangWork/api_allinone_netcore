using Allinone.BLL.Auditlogs;
using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Todolists;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;
using Newtonsoft.Json;

namespace Allinone.BLL.Todolists
{
    public class TodolistService(
        IAuditlogService _auditlogService,
        ITodolistRepository _todolistRepository,
        MemoryCacheHelper _memoryCacheHelper,
        IMapModel mapper) : BaseBLL, ITodolistService
    {
        public IEnumerable<TodolistDto> GetTodolistsUndone()
        {
            var todolists = _todolistRepository.GetTodolistsUndone(MemberId);
            foreach (var item in todolists)
            {
                if (MemoryCacheHelper.CacheTodolistType.TryGetValue(item.CategoryID, out var name))
                {
                    item.CategoryName = name;
                }
            }
            return todolists;
        }

        public async Task<Todolist> Get(int id)
        {
            return await _todolistRepository.GetAsync(id) ?? throw new TodolistNotFoundException();
        }

        public async Task<Todolist> Add(TodolistAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<TodolistAddReq, Todolist>(req);
            entity.MemberID = MemberId;

            await _todolistRepository.Add(entity);

            await _auditlogService.LogTodolistNew(req.Name, MemberId, JsonConvert.SerializeObject(entity));

            return entity;
        }

        public async Task<Todolist> Update(int id, TodolistAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _todolistRepository.GetAsync(id) ?? throw new TodolistNotFoundException();

            mapper.Map(req, entity);

            _todolistRepository.Update(entity);

            await _auditlogService.LogTodolistUpdate(
                req.Name, MemberId, JsonConvert.SerializeObject(entity), JsonConvert.SerializeObject(req));

            return entity;
        }

        public async Task<Todolist> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await _todolistRepository.GetAsync(id) ?? throw new TodolistNotFoundException();

            _todolistRepository.Delete(entity);

            await _auditlogService.LogTodolistDelete(entity.Name, MemberId, JsonConvert.SerializeObject(entity));

            return entity;
        }
    }
}
