namespace Patient.Application.Events
{
    public class MedicationPrescribedEvent
    {
        public int PatientMedicationId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string MedicationType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PrescribedByDoctorId { get; set; }
        public DateTime PrescribedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public MedicationPrescribedEvent(int patientMedicationId, int patientId, string patientName, string patientCode,
            int medicationId, string medicationName, string medicationType, string category, int prescribedByDoctorId,
            DateTime prescribedDate, DateTime startDate, DateTime? endDate, string dosage, string frequency,
            string? instructions, string? notes, DateTime createdAt)
        {
            PatientMedicationId = patientMedicationId;
            PatientId = patientId;
            PatientName = patientName;
            PatientCode = patientCode;
            MedicationId = medicationId;
            MedicationName = medicationName;
            MedicationType = medicationType;
            Category = category;
            PrescribedByDoctorId = prescribedByDoctorId;
            PrescribedDate = prescribedDate;
            StartDate = startDate;
            EndDate = endDate;
            Dosage = dosage;
            Frequency = frequency;
            Instructions = instructions;
            Notes = notes;
            CreatedAt = createdAt;
        }
    }

    public class MedicationDiscontinuedEvent
    {
        public int PatientMedicationId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string MedicationType { get; set; } = string.Empty;
        public int PrescribedByDoctorId { get; set; }
        public string DiscontinuationReason { get; set; } = string.Empty;
        public DateTime DiscontinuationDate { get; set; }

        public MedicationDiscontinuedEvent(int patientMedicationId, int patientId, string patientName,
            int medicationId, string medicationName, string medicationType, int prescribedByDoctorId,
            string discontinuationReason, DateTime discontinuationDate)
        {
            PatientMedicationId = patientMedicationId;
            PatientId = patientId;
            PatientName = patientName;
            MedicationId = medicationId;
            MedicationName = medicationName;
            MedicationType = medicationType;
            PrescribedByDoctorId = prescribedByDoctorId;
            DiscontinuationReason = discontinuationReason;
            DiscontinuationDate = discontinuationDate;
        }
    }

    public class MedicationAdherenceRecordedEvent
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public DateTime ScheduledDateTime { get; set; }
        public DateTime? ActualTakenDateTime { get; set; }
        public string Status { get; set; } = string.Empty; // Taken, Missed, Late, Skipped
        public string? Reason { get; set; }
        public string? SideEffectsReported { get; set; }
        public int? DelayMinutes { get; set; }
        public DateTime RecordedAt { get; set; }

        public MedicationAdherenceRecordedEvent(int patientId, string patientName, int medicationId, string medicationName,
            DateTime scheduledDateTime, DateTime? actualTakenDateTime, string status, string? reason,
            string? sideEffectsReported, int? delayMinutes, DateTime recordedAt)
        {
            PatientId = patientId;
            PatientName = patientName;
            MedicationId = medicationId;
            MedicationName = medicationName;
            ScheduledDateTime = scheduledDateTime;
            ActualTakenDateTime = actualTakenDateTime;
            Status = status;
            Reason = reason;
            SideEffectsReported = sideEffectsReported;
            DelayMinutes = delayMinutes;
            RecordedAt = recordedAt;
        }
    }

    public class MedicationRefillDueEvent
    {
        public int PatientMedicationId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public DateTime NextRefillDue { get; set; }
        public int? RefillsRemaining { get; set; }
        public DateTime CheckedAt { get; set; }

        public MedicationRefillDueEvent(int patientMedicationId, int patientId, string patientName, string patientCode,
            int medicationId, string medicationName, DateTime nextRefillDue, int? refillsRemaining, DateTime checkedAt)
        {
            PatientMedicationId = patientMedicationId;
            PatientId = patientId;
            PatientName = patientName;
            PatientCode = patientCode;
            MedicationId = medicationId;
            MedicationName = medicationName;
            NextRefillDue = nextRefillDue;
            RefillsRemaining = refillsRemaining;
            CheckedAt = checkedAt;
        }
    }
}
