using Allinone.DLL.Repositories;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Todolists;
using Allinone.Helper.Mapper;

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

            return await todolistDoneRepository.Add(entity);
        }

        public async Task<TodolistDone> Update(int id, TodolistDoneUpdateReq req)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await todolistDoneRepository.GetByIdAsync(id) ?? throw new TodolistDoneNotFoundException();

            mapper.Map(req, entity);

            todolistDoneRepository.Update(entity);

            return entity;
        }

        public async Task<TodolistDone> Delete(int id)
        {
            if (MemberId == 0) throw new MemberNotFoundException();

            var entity = await todolistDoneRepository.GetByIdAsync(id) ?? throw new TodolistDoneNotFoundException();

            todolistDoneRepository.Delete(entity);

            return entity;
        }
    }
}
