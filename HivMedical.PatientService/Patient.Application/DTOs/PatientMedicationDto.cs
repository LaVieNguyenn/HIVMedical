namespace Patient.Application.DTOs
{
    public class PatientMedicationDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int MedicationId { get; set; }
        public int PrescribedByDoctorId { get; set; }
        public DateTime PrescribedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string? Instructions { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DiscontinuationReason { get; set; }
        public DateTime? DiscontinuationDate { get; set; }
        public string? Notes { get; set; }
        public bool IsCurrentlyTaking { get; set; }
        public int? RefillsRemaining { get; set; }
        public DateTime? LastRefillDate { get; set; }
        public DateTime? NextRefillDue { get; set; }
        public decimal? AdherencePercentage { get; set; }
        public int? MissedDoses { get; set; }
        public DateTime? LastAdherenceUpdate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Related data
        public MedicationDto? Medication { get; set; }
        public string? PatientName { get; set; }
        public string? PatientCode { get; set; }
    }

    public class CreatePatientMedicationRequest
    {
        public int PatientId { get; set; }
        public int MedicationId { get; set; }
        public int PrescribedByDoctorId { get; set; }
        public DateTime PrescribedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public int? RefillsRemaining { get; set; }
        public DateTime? NextRefillDue { get; set; }
    }

    public class UpdatePatientMedicationRequest
    {
        public DateTime? EndDate { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string? Instructions { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? DiscontinuationReason { get; set; }
        public DateTime? DiscontinuationDate { get; set; }
        public string? Notes { get; set; }
        public bool IsCurrentlyTaking { get; set; }
        public int? RefillsRemaining { get; set; }
        public DateTime? LastRefillDate { get; set; }
        public DateTime? NextRefillDue { get; set; }
    }

    public class PatientMedicationFilterRequest
    {
        public int? PatientId { get; set; }
        public int? MedicationId { get; set; }
        public int? PrescribedByDoctorId { get; set; }
        public string? Status { get; set; }
        public bool? IsCurrentlyTaking { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public string? Category { get; set; }
        public string? MedicationType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PatientMedicationSummaryDto
    {
        public int TotalMedications { get; set; }
        public int ActiveMedications { get; set; }
        public int CompletedMedications { get; set; }
        public int DiscontinuedMedications { get; set; }
        public decimal? OverallAdherencePercentage { get; set; }
        public int TotalMissedDoses { get; set; }
        public DateTime? LastAdherenceUpdate { get; set; }
        public List<PatientMedicationDto> CurrentMedications { get; set; } = new List<PatientMedicationDto>();
        public List<PatientMedicationDto> UpcomingRefills { get; set; } = new List<PatientMedicationDto>();
    }
}
