using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Server.Data;
using TransGo_Alpha.Shared;

namespace TransGo_Alpha.Server.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;

        // Inject a new instance of UserDbContext to the constructor of the controller
        public UserController(UserDbContext context)
        {
            this._context = context;
        }

        [HttpPost("adduser")]
        public async Task<IActionResult> Post(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user.Firstname);
        }
    }
}
