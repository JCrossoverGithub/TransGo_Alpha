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
    [Route("api/courses")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly CourseDbContext _context;

        // Inject a new instance of CourseContext to the constructor of the controller
        public CourseController(CourseDbContext context)
        {
            this._context = context;
        }

        // Fetches the details of ONE course that matches the passed courseid parameter
        // SIMPLIFY THIS BY TAKING IN THE WHOLE COURSE ARRAY AS 1 PARAMETER (MAKES IT 1 GET REQUEST INSTEAD OF MULTIPLE)
        [HttpGet("{courseid}")]
        public async Task<IActionResult> GetCourse(string courseid)
        {
            var returnedcourse = await _context.tblCourses.FirstOrDefaultAsync(a => a.Id == courseid);

            if (returnedcourse == null)
            {
                return NotFound();
            }

            return Ok(returnedcourse);
        }

        // Checks if a course exists using its ID
        [HttpGet("check/{courseid}")]
        public async Task<Boolean> CheckCourse(string courseid)
        {
            // Maybe can simplify this to check in the Student controller Put (update).
            // The only thing to think about for that, is what to do when it returns false for the class existing
            // exist
            var checker = _context.tblCourses.Find(courseid);
            if (checker == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // Create new course in DB
        [HttpPost("createcourse")]
        public async Task<IActionResult> Post(Course course)
        {
            _context.Add(course);
            await _context.SaveChangesAsync();
            return Ok(course.CourseName);
        }

        // Adds a student ID to a course's student list upon registering
        [HttpPut("adduserid/{course}")]
        public async Task<IActionResult> UpdateStudentList(Course _course)
        {
            var course = await _context.tblCourses.FirstOrDefaultAsync(a => a.Id == _course.Id);
            // add if course.students == null check
            if (String.IsNullOrEmpty(course.Students))
            {
                course.Students = _course.Students;
                _context.Entry(course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else 
            {
                course.Students = _course.Students + " " + course.Students;
                _context.Entry(course).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }

        // Remove a student ID from a course's students list upon registering
        [HttpPut("removeuser/{_course}")]
        public async Task<IActionResult> RemoveStudent(Course _course)
        {
            var course = await _context.tblCourses.FirstOrDefaultAsync(a => a.Id == _course.Id);
            // add if course.students == null check
            course.Students = _course.Students;
            _context.Entry(course).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
