using SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Domain.Entities
{
    public class Doctors 
    {
            public int DoctorId { get; set; }
            public int UserId { get; set; }
            public User? User { get; set; }
            public ICollection<DoctorSchedule>? Schedules { get; set; }
            public ICollection<DoctorQualification>? Qualifications { get; set; }
            public ICollection<DoctorSpecialization>? Specializations { get; set; }
    }
}
