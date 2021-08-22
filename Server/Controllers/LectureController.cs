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
    [Route("api/lectures")]
    [ApiController]
    public class LectureController : ControllerBase
    {
        private readonly LectureDbContext _context;

        // Inject a new instance of UserDbContext to the constructor of the controller
        public LectureController(LectureDbContext context)
        {
            this._context = context;
        }

        // Call info about a single lecture using its ID.
        [HttpGet("test/{lectureid}")]
        public async Task<IActionResult> GetLecture(string lectureid)
        {
            var lecture = await _context.tblLectures.FirstOrDefaultAsync(a => a.Id == lectureid);

            if (lecture == null)
            {
                return NotFound();
            }

            return Ok(lecture);
        }

    }
}
