namespace Patient.Application.DTOs
{
    public class MedicationScheduleDto
    {
        public int Id { get; set; }
        public int PatientMedicationId { get; set; }
        public int MedicationId { get; set; }
        public int PatientId { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string? SpecialInstructions { get; set; }
        public bool IsActive { get; set; }
        public bool ReminderEnabled { get; set; }
        public int? ReminderMinutesBefore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Related data
        public MedicationDto? Medication { get; set; }
        public string? PatientName { get; set; }
        public string? MedicationName { get; set; }
        public string? Dosage { get; set; }
    }

    public class CreateMedicationScheduleRequest
    {
        public int PatientMedicationId { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string? SpecialInstructions { get; set; }
        public bool ReminderEnabled { get; set; } = true;
        public int? ReminderMinutesBefore { get; set; } = 30;
    }

    public class UpdateMedicationScheduleRequest
    {
        public TimeSpan ScheduledTime { get; set; }
        public string DayOfWeek { get; set; } = string.Empty;
        public string? SpecialInstructions { get; set; }
        public bool IsActive { get; set; }
        public bool ReminderEnabled { get; set; }
        public int? ReminderMinutesBefore { get; set; }
    }

    public class MedicationAdherenceDto
    {
        public int Id { get; set; }
        public int PatientMedicationId { get; set; }
        public int PatientId { get; set; }
        public int MedicationId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? ActualTakenDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public bool WasReminded { get; set; }
        public DateTime? ReminderSentAt { get; set; }
        public string? SideEffectsReported { get; set; }
        public int? DelayMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Related data
        public MedicationDto? Medication { get; set; }
        public string? PatientName { get; set; }
        public string? MedicationName { get; set; }
    }

    public class RecordMedicationAdherenceRequest
    {
        public int PatientMedicationId { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? ActualTakenDateTime { get; set; }
        public string Status { get; set; } = string.Empty; // Taken, Missed, Late, Skipped
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public string? SideEffectsReported { get; set; }
    }

    public class UpdateMedicationAdherenceRequest
    {
        public DateTime? ActualTakenDateTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public string? SideEffectsReported { get; set; }
    }

    public class MedicationAdherenceFilterRequest
    {
        public int? PatientId { get; set; }
        public int? MedicationId { get; set; }
        public int? PatientMedicationId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class MedicationAdherenceReportDto
    {
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public DateTime ReportPeriodStart { get; set; }
        public DateTime ReportPeriodEnd { get; set; }
        public int TotalScheduledDoses { get; set; }
        public int TakenDoses { get; set; }
        public int MissedDoses { get; set; }
        public int LateDoses { get; set; }
        public int SkippedDoses { get; set; }
        public decimal AdherencePercentage { get; set; }
        public List<MedicationAdherenceDto> AdherenceRecords { get; set; } = new List<MedicationAdherenceDto>();
        public List<string> CommonMissedReasons { get; set; } = new List<string>();
        public List<string> ReportedSideEffects { get; set; } = new List<string>();
    }

    public class DailyMedicationScheduleDto
    {
        public DateTime Date { get; set; }
        public List<MedicationScheduleDto> Schedules { get; set; } = new List<MedicationScheduleDto>();
        public List<MedicationAdherenceDto> AdherenceRecords { get; set; } = new List<MedicationAdherenceDto>();
    }
}
