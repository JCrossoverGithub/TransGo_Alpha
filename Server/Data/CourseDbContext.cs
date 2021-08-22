using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;


namespace TransGo_Alpha.Server.Data
{
    public class CourseDbContext : DbContext
    {
        public virtual DbSet<Course> tblCourses { get; set; }

        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options)
        {
        }
    }
}
