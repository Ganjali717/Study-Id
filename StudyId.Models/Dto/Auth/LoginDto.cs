using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyId.Models.Dto
{
    public class LoginDto
    {
        public string? ReturnUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
