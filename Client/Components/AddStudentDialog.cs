using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace TransGo_Alpha.Client.Components
{
    public class Student
    {
        public string email { get; set; }

        public bool ShowDialog { get; set; }

    }

}
