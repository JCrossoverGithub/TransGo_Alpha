using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransGo_Alpha.Shared;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace TransGo_Alpha.Client.Components
{
    public class Login
    {
        [Required]
        //old example requirement parameter
        //[Range(100000, 999999, ErrorMessage = "Code must be 6 digits!")]
        // Create good-proper ones in the future
        public string email { get; set; }

        [Required]
        public string password { get; set; }
        public bool ShowDialog { get; set; }

    }
}
