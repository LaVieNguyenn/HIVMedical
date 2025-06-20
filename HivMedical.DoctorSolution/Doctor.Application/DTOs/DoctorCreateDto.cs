using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Application.DTOs
{
    public class DoctorCreateDto
    {
        public int UserId { get; set; }
        public List<string>? Qualifications { get; set; }
        public List<string>? Specializations { get; set; }
    }
}
