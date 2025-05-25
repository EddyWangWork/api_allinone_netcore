using Allinone.DLL.Data;
using Allinone.Domain.Enums;
using Allinone.Domain.Todolists;
using Microsoft.EntityFrameworkCore;

namespace Allinone.DLL.Repositories
{
    public interface ITodolistRepository
    {
        IEnumerable<TodolistDto> GetTodolistsUndone(int memberId);
        Task<bool>? IsExist(int id);
        Task<Todolist>? GetAsync(int id);
        Task<IEnumerable<Todolist>> GetAllAsync();
        Task Add(Todolist todolist);
        void Update(Todolist todolist);
        void Delete(Todolist todolist);
    }

    public class TodolistRepository(DSContext context) : ITodolistRepository
    {
        public IEnumerable<TodolistDto> GetTodolistsUndone(int memberId)
        {
            var todolists = new List<TodolistDto>();

            var responses = (
                 from a in context.Todolist
                 join b in context.TodolistDone on a.ID equals b.TodolistID into bb
                 from b2 in bb.DefaultIfEmpty()
                 where
                    a.MemberID == memberId
                 select new
                 {
                     ID = a.ID,
                     Name = a.Name,
                     CategoryID = a.CategoryID,
                     Description = a.Description ?? string.Empty,
                     UpdateDate = a.UpdateDate,
                     DoneDate = b2.UpdateDate != null ? b2.UpdateDate : DateTime.MinValue,
                     TodolistDoneID = b2.ID != null ? b2.ID : 0
                 }).AsEnumerable();

            var todolistsTemp = responses;

            var todolistsG = todolistsTemp.GroupBy(x => x.Name);

            foreach (var todolistG in todolistsG)
            {
                if ((todolistG.Any(x => x.CategoryID == (int)EnumTodolistType.Monthly
                        && x.DoneDate.Year == DateTime.Now.Year
                        && x.DoneDate.Month == DateTime.Now.Month)) ||
                    todolistG.Any(x => x.CategoryID == (int)EnumTodolistType.Normal
                        && x.TodolistDoneID > 0))
                    continue;

                var todolist = todolistG.FirstOrDefault();

                todolists.Add(new TodolistDto
                {
                    ID = todolist.ID,
                    Name = todolist.Name,
                    CategoryID = todolist.CategoryID,
                    Description = todolist.Description,
                    UpdateDate = todolist.UpdateDate
                });
            }

            return todolists;
        }

        public async Task<bool>? IsExist(int id) =>
            await context.Todolist.AnyAsync(x => x.ID == id);

        public async Task<Todolist>? GetAsync(int id) =>
            await context.Todolist.FindAsync(id);

        public async Task<IEnumerable<Todolist>> GetAllAsync() =>
            await context.Todolist.ToListAsync();

        public async Task Add(Todolist todolist)
        {
            await context.Todolist.AddAsync(todolist);
            await context.SaveChangesAsync();
        }

        public void Update(Todolist todolist)
        {
            context.Todolist.Update(todolist);
            context.SaveChanges();
        }

        public void Delete(Todolist todolist)
        {
            context.Todolist.Remove(todolist);
            context.SaveChanges();
        }
    }
}
