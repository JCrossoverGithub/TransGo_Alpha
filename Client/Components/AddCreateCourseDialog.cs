using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace TransGo_Alpha.Client.Components
{
    public class CreateCourse
    {
        public string CourseNumber { get; set; }
        public string CourseName { get; set; }
        public bool ShowDialog { get; set; }

    }
}
