using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace TransGo_Alpha.Shared
{
    public class Course
    {
        public string Id { get; set; }
        public string CourseNum { get; set; }
        public string CourseName { get; set; }
        public string Semester { get; set; }
        public string Instructors { get; set; }
        public string Students { get; set; }
        public string Transcripts { get; set; }

    }
}
