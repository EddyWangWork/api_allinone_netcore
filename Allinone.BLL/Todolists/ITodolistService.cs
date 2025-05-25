using Allinone.Domain.Todolists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allinone.BLL.Todolists
{
    public interface ITodolistService
    {
        IEnumerable<TodolistDto> GetTodolistsUndone();
        Task<Todolist> Get(int id);
        Task<Todolist> Add(TodolistAddReq req);
        Task<Todolist> Update(int id, TodolistAddReq req);
        Task<Todolist> Delete(int id);
    }
}
