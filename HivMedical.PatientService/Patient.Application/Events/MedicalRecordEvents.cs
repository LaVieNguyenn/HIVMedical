namespace Patient.Application.Events
{
    public class MedicalRecordCreatedEvent
    {
        public int MedicalRecordId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
        public string RecordType { get; set; } = string.Empty; // Lab Result, Clinical Note, Treatment Update
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? LabResults { get; set; }
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public MedicalRecordCreatedEvent(int medicalRecordId, int patientId, string patientName, string patientCode,
            DateTime recordDate, string recordType, string title, string description, string? labResults,
            string? medications, string? notes, int doctorId, DateTime createdAt)
        {
            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            PatientName = patientName;
            PatientCode = patientCode;
            RecordDate = recordDate;
            RecordType = recordType;
            Title = title;
            Description = description;
            LabResults = labResults;
            Medications = medications;
            Notes = notes;
            DoctorId = doctorId;
            CreatedAt = createdAt;
        }
    }

    public class LabResultRecordedEvent
    {
        public int MedicalRecordId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string LabResults { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public LabResultRecordedEvent(int medicalRecordId, int patientId, string patientName, string patientCode,
            DateTime recordDate, string title, string labResults, int doctorId, DateTime createdAt)
        {
            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            PatientName = patientName;
            PatientCode = patientCode;
            RecordDate = recordDate;
            Title = title;
            LabResults = labResults;
            DoctorId = doctorId;
            CreatedAt = createdAt;
        }
    }

    public class TreatmentUpdateRecordedEvent
    {
        public int MedicalRecordId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientCode { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Medications { get; set; }
        public string? Notes { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }

        public TreatmentUpdateRecordedEvent(int medicalRecordId, int patientId, string patientName, string patientCode,
            DateTime recordDate, string title, string description, string? medications, string? notes,
            int doctorId, DateTime createdAt)
        {
            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            PatientName = patientName;
            PatientCode = patientCode;
            RecordDate = recordDate;
            Title = title;
            Description = description;
            Medications = medications;
            Notes = notes;
            DoctorId = doctorId;
            CreatedAt = createdAt;
        }
    }
}
