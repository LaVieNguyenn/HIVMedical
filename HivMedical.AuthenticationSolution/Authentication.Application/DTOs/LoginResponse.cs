using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.DTOs
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
