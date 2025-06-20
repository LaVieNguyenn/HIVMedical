namespace Patient.Application.DTOs
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LabResults { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMedicalRecordRequest
    {
        public int PatientId { get; set; }
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LabResults { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public int DoctorId { get; set; }
    }

    public class UpdateMedicalRecordRequest
    {
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LabResults { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
    }
}
