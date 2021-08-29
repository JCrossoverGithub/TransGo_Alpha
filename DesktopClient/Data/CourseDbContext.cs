using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DesktopClient.Models;

namespace DesktopClient.Data
{
    class CourseDbContext : DbContext
    {
        public DbSet<Course> tblCourses { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=MSI;Initial Catalog=tgAlpha;Integrated Security=True;MultipleActiveResultSets=True");
        }
    }
}
