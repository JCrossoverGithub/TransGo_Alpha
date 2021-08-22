using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace TransGo_Alpha.Client.Components
{
    public class TeachReg
    {
        [Required]
        //old example requirement parameter
        //[Range(100000, 999999, ErrorMessage = "Code must be 6 digits!")]
        // Create good-proper ones in the future
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        [Required]
        public string Password { get; set; }
        public bool ShowDialog { get; set; }

    }
}
