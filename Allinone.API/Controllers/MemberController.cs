using Allinone.BLL.Members;
using Allinone.Domain.Members;
using Allinone.Helper.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allinone.API.Controllers
{
    [ApiController]
    //[ResponseCompressionAttribute]
    [Route("[controller]")]
    public class MemberController(IMemberService memberService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("loginV2")]
        public async Task<IActionResult> LoginV2(MemberLoginReq req)
        {
            var member = await memberService.LoginV2(req.Name, req.Password);

            if (member.IsNullOrEmpty())
            {
                return Unauthorized("Username or password incorrect");
            }

            return Ok(member);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(MemberLoginReq req)
        {
            var member = await memberService.Add(req.Name, req.Password);

            return Ok(member);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Edit(int id, MemberLoginReq req)
        //{
        //    var response = await _memberBLL.Edit(id, req);
        //    return Ok(response);
        //}
    }
}
