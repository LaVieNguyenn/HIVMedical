using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doctor.Domain.Entities
{
    public class Qualification
    {
        public int QualificationId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
