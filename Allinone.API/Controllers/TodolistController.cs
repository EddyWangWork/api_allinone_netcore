using Allinone.BLL.Todolists;
using Allinone.Domain.Todolists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TodolistController(ITodolistService todolistService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTodolistsUndone()
        {
            var response = todolistService.GetTodolistsUndone();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = await todolistService.Get(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TodolistAddReq req)
        {
            var response = await todolistService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TodolistAddReq req)
        {
            var response = await todolistService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await todolistService.Delete(id);
            return Ok(response);
        }
    }
}
