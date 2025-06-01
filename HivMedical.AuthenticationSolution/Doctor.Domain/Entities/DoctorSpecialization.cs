using SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Domain.Entities
{
    public class DoctorSpecialization : BaseEntity
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
