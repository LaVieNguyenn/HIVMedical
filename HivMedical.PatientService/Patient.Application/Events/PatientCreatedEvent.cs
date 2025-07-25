using System.Text.Json.Serialization;

namespace Patient.Application.Events
{
    public class PatientCreatedEvent
    {
        public int PatientId { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte Gender { get; set; }
        public int AuthUserId { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public string? HIVStatus { get; set; }
        public string? TreatmentStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        // Parameterless constructor for JSON deserialization
        public PatientCreatedEvent() { }

        public PatientCreatedEvent(int patientId, string patientCode, string fullName, string? email, 
            string? phone, DateTime dateOfBirth, byte gender, int authUserId, DateTime? diagnosisDate, 
            string? hivStatus, string? treatmentStatus, DateTime createdAt)
        {
            PatientId = patientId;
            PatientCode = patientCode;
            FullName = fullName;
            Email = email;
            Phone = phone;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            AuthUserId = authUserId;
            DiagnosisDate = diagnosisDate;
            HIVStatus = hivStatus;
            TreatmentStatus = treatmentStatus;
            CreatedAt = createdAt;
        }
    }

    public class PatientUpdatedEvent
    {
        public int PatientId { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte Gender { get; set; }
        public string? HIVStatus { get; set; }
        public string? TreatmentStatus { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Parameterless constructor for JSON deserialization
        public PatientUpdatedEvent() { }

        public PatientUpdatedEvent(int patientId, string patientCode, string fullName, string? email,
            string? phone, DateTime dateOfBirth, byte gender, string? hivStatus, string? treatmentStatus, DateTime updatedAt)
        {
            PatientId = patientId;
            PatientCode = patientCode;
            FullName = fullName;
            Email = email;
            Phone = phone;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            HIVStatus = hivStatus;
            TreatmentStatus = treatmentStatus;
            UpdatedAt = updatedAt;
        }
    }
}
