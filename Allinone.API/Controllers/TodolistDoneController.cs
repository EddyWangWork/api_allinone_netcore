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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var response = await todolistDoneService.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TodolistDoneAddReq req)
        {
            var response = await todolistDoneService.Add(req);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TodolistDoneUpdateReq req)
        {
            var response = await todolistDoneService.Update(id, req);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await todolistDoneService.Delete(id);
            return Ok(response);
        }
    }
}
