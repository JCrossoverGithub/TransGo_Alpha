using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace TransGo_Alpha.Client.Components
{
    public class CourseId
    {
        [Required]
        [Range(100000, 999999, ErrorMessage = "Code must be 6 digits!")]
        public int Id { get; set; }

        public bool ShowDialog { get; set; }

    }

}
