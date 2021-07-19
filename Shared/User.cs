using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransGo_Alpha.Shared
{
    public class User
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public string School { get; set; }
        public string Studentid { get; set; }
        public string Coursecodes { get; set; }
        public string Ownedcourses { get; set; }
        public string Roles { get; set; }
    }
}
