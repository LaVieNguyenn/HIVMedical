using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Domain.Entities
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public ICollection<DoctorSchedule>? Schedules { get; set; }
        public ICollection<DoctorQualification>? Qualifications { get; set; }
        public ICollection<DoctorSpecialization>? Specializations { get; set; }
    }
}
