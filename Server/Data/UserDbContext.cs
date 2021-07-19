using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;


namespace TransGo_Alpha.Server.Data
{
    public class UserDbContext : DbContext
    {
        public virtual DbSet<User> tblUsers { get; set; }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
    }
}
