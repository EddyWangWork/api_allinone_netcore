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
        Task<TodolistDone?> GetByIdAsync(int id, int memberId);
        Task<TodolistDone> AddAsync(TodolistDone entity, int memberId);
        Task UpdateAsync(TodolistDone entity);
        Task DeleteAsync(TodolistDone entity);
    }

    public class TodolistDoneRepository(DSContext context) : ITodolistDoneRepository
    {
        public async Task<List<TodolistDoneDto>> GetAllAsync(int memberId)
        {
            var rows = await (
                from a in context.TodolistDone.AsNoTracking()
                join b in context.Todolist on a.TodolistID equals b.ID
                where b.MemberID == memberId
                orderby a.UpdateDate descending
                select new
                {
                    a.ID,
                    a.Remark,
                    a.UpdateDate,
                    b.CategoryID,
                    b.Description,
                    b.Name,
                    TodolistID = b.ID
                }).ToListAsync();

            return rows.Select(x => new TodolistDoneDto
            {
                ID = x.ID,
                Remark = x.Remark,
                UpdateDate = x.UpdateDate,
                TodolistCategory = ((EnumTodolistType)x.CategoryID).ToString(),
                TodolistDescription = x.Description,
                TodolistName = x.Name,
                TodolistID = x.TodolistID
            }).ToList();
        }

        public Task<TodolistDone?> GetByIdAsync(int id, int memberId) =>
            context.TodolistDone
                .Include(x => x.Todolist)
                .FirstOrDefaultAsync(x => x.ID == id && x.Todolist.MemberID == memberId);

        public async Task<TodolistDone> AddAsync(TodolistDone entity, int memberId)
        {
            var todolist = await context.Todolist
                .FirstOrDefaultAsync(x => x.ID == entity.TodolistID && x.MemberID == memberId)
                ?? throw new TodolistNotFoundException();

            var latestDone = await context.TodolistDone
                .Where(x => x.TodolistID == todolist.ID)
                .OrderByDescending(x => x.UpdateDate)
                .Select(x => (DateTime?)x.UpdateDate)
                .FirstOrDefaultAsync();

            if (latestDone.HasValue && IsAlreadyDone(todolist.CategoryID, latestDone.Value))
            {
                throw new TodolistAlreadyDoneException("Todolist already done");
            }

            entity.Todolist = todolist;
            await context.TodolistDone.AddAsync(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(TodolistDone entity)
        {
            context.TodolistDone.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TodolistDone entity)
        {
            context.TodolistDone.Remove(entity);
            await context.SaveChangesAsync();
        }

        private static bool IsAlreadyDone(int categoryId, DateTime latestDone)
        {
            var now = DateTime.Now;
            return categoryId switch
            {
                (int)EnumTodolistType.Normal => true,
                (int)EnumTodolistType.Monthly => now.Year == latestDone.Year && now.Month == latestDone.Month,
                _ => false
            };
        }
    }
}
