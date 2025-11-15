using Allinone.DLL.Data;
using Allinone.Domain.Enums;
using Allinone.Domain.Exceptions;
using Allinone.Domain.Todolists;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITodolistDoneRepository
    {
        Task<List<TodolistDoneDto>> GetAllAsync(int memberId);
        Task<TodolistDone> Add(TodolistDone req);
        Task<bool> IsExist(int id);
        Task<IEnumerable<TodolistDone>> GetAllAsync();
        Task<TodolistDone>? GetByIdAsync(int id);
        //Task Add(TodolistDone todolistDone);
        void Update(TodolistDone todolistDone);
        void Delete(TodolistDone todolistDone);
    }

    public class TodolistDoneRepository(DSContext context) : ITodolistDoneRepository
    {
        public async Task<List<TodolistDoneDto>> GetAllAsync(int memberId)
        {
            var responses = (
                 from a in context.TodolistDone
                 join b in context.Todolist on a.TodolistID equals b.ID
                 where b.MemberID == memberId
                 select new TodolistDoneDto
                 {
                     ID = a.ID,
                     Remark = a.Remark,
                     UpdateDate = a.UpdateDate,
                     TodolistCategory = ((EnumTodolistType)b.CategoryID).ToString(),
                     TodolistDescription = b.Description,
                     TodolistName = b.Name,
                     TodolistID = b.ID
                 });

            return await responses.OrderByDescending(x => x.UpdateDate).ToListAsync();
        }

        public async Task<TodolistDone> Add(TodolistDone req)
        {
            var responses = (
                 from a in context.Todolist
                 join b in context.TodolistDone on a.ID equals b.TodolistID into bb
                 from b2 in bb.DefaultIfEmpty()
                 where
                    a.ID == req.TodolistID
                 select new
                 {
                     Todolist = a,
                     TodolistName = a.Name,
                     TodolistID = a.ID,
                     TodolistDoneID = b2.ID != null ? b2.ID : 0,
                     a.CategoryID,
                     UpdateDate = b2.UpdateDate != null ? b2.UpdateDate : DateTime.MinValue,
                 }).FirstOrDefaultAsync();

            var todolist = await responses;

            if (todolist == null)
            {
                throw new TodolistNotFoundException();
            }
            else if (todolist.TodolistDoneID == 0)
            {
                // no done record
            }
            else if (todolist?.CategoryID == (int)EnumTodolistType.Normal ||
                (todolist?.CategoryID == (int)EnumTodolistType.Monthly &&
                DateTime.Now.Year == todolist?.UpdateDate.Year &&
                DateTime.Now.Month == todolist?.UpdateDate.Month))
            {
                throw new TodolistAlreadyDoneException("Todolist already done");
            }

            await context.TodolistDone.AddAsync(req);
            await context.SaveChangesAsync();

            req.Todolist = todolist.Todolist;

            return req;
        }

        public async Task<bool> IsExist(int id) =>
            await context.TodolistDone.AnyAsync(x => x.ID == id);

        public async Task<IEnumerable<TodolistDone>> GetAllAsync() =>
            await context.TodolistDone.ToListAsync();

        //public async Task<TodolistDone>? GetByIdAsync(int id) =>
        //    await context.TodolistDone.FindAsync(id);

        public async Task<TodolistDone>? GetByIdAsync(int id) =>
            await context.TodolistDone
            .Include(x => x.Todolist)
            .FirstOrDefaultAsync(x => x.ID == id);

        public void Update(TodolistDone todolistDone)
        {
            context.TodolistDone.Update(todolistDone);
            context.SaveChanges();
        }

        public void Delete(TodolistDone todolistDone)
        {
            context.TodolistDone.Remove(todolistDone);
            context.SaveChanges();
        }
    }
}
