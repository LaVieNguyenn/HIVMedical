using SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Domain.Entities
{
    public class DoctorQualification
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int QualificationId { get; set; }
        public Qualification? Qualification { get; set; }
        public Doctors Doctor { get; set; } = null!;
    }
}
