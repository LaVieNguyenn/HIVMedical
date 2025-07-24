using SharedKernel.Entities;

namespace Patient.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public int AuthUserId { get; set; } 
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        
    
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
