using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        [HttpGet("{useremail}/{userpassword}")]
        public async Task<IActionResult> Get(string useremail, string userpassword)
        {
            var returneduser = await _context.tblUsers.FirstOrDefaultAsync(a => a.Email == useremail);
            
            if(returneduser.Password != userpassword)
            {
                return NotFound();
            }
            
            return Ok(returneduser);
        }

        [HttpGet("profile/{userid}")]
        public async Task<IActionResult> GetProfile(string userid)
        {
            var returneduser = await _context.tblUsers.FirstOrDefaultAsync(a => a.Id == userid);

            if (returneduser == null)
            {
                return NotFound();
            }

            return Ok(returneduser);
        }

        // Get student info from email
        [HttpGet("studentemail/{email}")]
        public async Task<IActionResult> GetFromEmail(string email)
        {
            var returneduser = await _context.tblUsers.FirstOrDefaultAsync(a => a.Email == email);

            if (returneduser == null)
            {
                return NotFound();
            }

            return Ok(returneduser);
        }

        // Updates a users list of enrolled courses
        [HttpPut("addcourse/{user}")]
        public async Task<IActionResult> UpdateUserCourses(User _user)
        {
            var profile = await _context.tblUsers.FirstOrDefaultAsync(a => a.Id == _user.Id);
            profile.Coursecodes = _user.Coursecodes;
            _context.Entry(profile).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var profile = await _context.tblUsers.FirstOrDefaultAsync(a => a.Id == id);
            if (profile == null)
                return NotFound();

            _context.tblUsers.Remove(profile);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
