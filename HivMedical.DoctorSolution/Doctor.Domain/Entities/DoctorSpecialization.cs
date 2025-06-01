using SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Domain.Entities
{
    public class DoctorSpecialization
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int SpecializationId { get; set; }
        public Specialization? Specialization { get; set; }
        public Doctors Doctor { get; set; } = null!;
    }
}
