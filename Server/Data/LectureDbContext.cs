using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;


namespace TransGo_Alpha.Server.Data
{
    public class LectureDbContext : DbContext
    {
        public virtual DbSet<Lecture> tblLectures { get; set; }

        public LectureDbContext(DbContextOptions<LectureDbContext> options) : base(options)
        {
        }
    }
}
