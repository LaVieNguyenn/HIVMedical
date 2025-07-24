namespace Patient.Application.DTOs
{
    public class DoctorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class CreateDoctorRequest
    {
        public int AuthUserId { get; set; } // Reference to User in Auth service
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class UpdateDoctorRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}

