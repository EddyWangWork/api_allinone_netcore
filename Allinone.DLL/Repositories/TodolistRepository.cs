using Allinone.DLL.Data;
using Allinone.Domain.Enums;
using Allinone.Domain.Todolists;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITodolistRepository
    {
        Task<List<TodolistDto>> GetTodolistsUndoneAsync(int memberId, DateTime date);
        Task<bool> IsExist(int id, int memberId);
        Task<Todolist?> GetAsync(int id, int memberId);
        Task<List<Todolist>> GetAllAsync(int memberId);
        Task Add(Todolist todolist);
        Task Update(Todolist todolist);
        Task Delete(Todolist todolist);
    }

    public class TodolistRepository(DSContext context) : ITodolistRepository
    {
        public async Task<List<TodolistDto>> GetTodolistsUndoneAsync(int memberId, DateTime date)
        {
            var todolists = await context.Todolist
                .AsNoTracking()
                .Where(x => x.MemberID == memberId)
                .ToListAsync();

            var dones = await context.TodolistDone
                .AsNoTracking()
                .Where(x => x.Todolist.MemberID == memberId)
                .Select(x => new { x.TodolistID, x.UpdateDate })
                .ToListAsync();

            var donesByTodolist = dones.ToLookup(x => x.TodolistID, x => x.UpdateDate);

            var result = new List<TodolistDto>();

            // Todolists sharing a name are treated as one logical item
            foreach (var group in todolists.GroupBy(x => x.Name))
            {
                var isDone = group.Any(t =>
                    (t.CategoryID == (int)EnumTodolistType.Monthly
                        && donesByTodolist[t.ID].Any(d => d.Year == date.Year && d.Month == date.Month))
                    || (t.CategoryID == (int)EnumTodolistType.Normal
                        && donesByTodolist[t.ID].Any()));

                if (isDone) continue;

                var first = group.First();
                result.Add(new TodolistDto
                {
                    ID = first.ID,
                    Name = first.Name,
                    CategoryID = first.CategoryID,
                    Description = first.Description ?? string.Empty,
                    UpdateDate = first.UpdateDate
                });
            }

            return result;
        }

        public Task<bool> IsExist(int id, int memberId) =>
            context.Todolist.AnyAsync(x => x.ID == id && x.MemberID == memberId);

        public Task<Todolist?> GetAsync(int id, int memberId) =>
            context.Todolist.FirstOrDefaultAsync(x => x.ID == id && x.MemberID == memberId);

        public Task<List<Todolist>> GetAllAsync(int memberId) =>
            context.Todolist.Where(x => x.MemberID == memberId).ToListAsync();

        public async Task Add(Todolist todolist)
        {
            await context.Todolist.AddAsync(todolist);
            await context.SaveChangesAsync();
        }

        public async Task Update(Todolist todolist)
        {
            context.Todolist.Update(todolist);
            await context.SaveChangesAsync();
        }

        public async Task Delete(Todolist todolist)
        {
            context.Todolist.Remove(todolist);
            await context.SaveChangesAsync();
        }
    }
}

