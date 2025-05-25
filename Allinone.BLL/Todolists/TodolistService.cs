using Allinone.DLL.Repositories;
using Allinone.Domain.DS.Accounts;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Todolists;
using Allinone.Helper.Cache;
using Allinone.Helper.Mapper;

namespace Allinone.BLL.Todolists
{
    public class TodolistService(
        ITodolistRepository todolistRepository,
        MemoryCacheHelper memoryCacheHelper,
        IMapModel mapper) : BaseBLL, ITodolistService
    {
        public IEnumerable<TodolistDto> GetTodolistsUndone()
        {
            var todolists = todolistRepository.GetTodolistsUndone(MemberId);
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
            return await todolistRepository.GetAsync(id) ?? throw new TodolistNotFoundException();
        }

        public async Task<Todolist> Add(TodolistAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = mapper.MapDto<TodolistAddReq, Todolist>(req);
            entity.MemberID = MemberId;

            await todolistRepository.Add(entity);

            return entity;
        }

        public async Task<Todolist> Update(int id, TodolistAddReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await todolistRepository.GetAsync(id) ?? throw new TodolistNotFoundException();

            mapper.Map(req, entity);

            todolistRepository.Update(entity);

            return entity;
        }

        public async Task<Todolist> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await todolistRepository.GetAsync(id) ?? throw new TodolistNotFoundException();

            todolistRepository.Delete(entity);

            return entity;
        }
    }
}
