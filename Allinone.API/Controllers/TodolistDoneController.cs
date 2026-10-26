using Allinone.BLL.Todolists;
using Allinone.Domain.Todolists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TodolistDoneController(ITodolistDoneService todolistDoneService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var response = await todolistDoneService.GetAsync();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id) =>
            Ok(await todolistDoneService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Add(TodolistDoneAddReq req) =>
            Ok(await todolistDoneService.Add(req));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, TodolistDoneUpdateReq req) =>
            Ok(await todolistDoneService.Update(id, req));

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await todolistDoneService.Delete(id));
    }
}
