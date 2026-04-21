using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Andon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult GetPublicText()
        {
            return Ok("This is a public text.");
        }

        [HttpGet("user")]
        [Authorize]
        public IActionResult GetUserText()
        {
            return Ok("This is a user text.");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "3")]
        public IActionResult GetAdminText()
        {
            return Ok("This is an admin text.");
        }
    }
}
