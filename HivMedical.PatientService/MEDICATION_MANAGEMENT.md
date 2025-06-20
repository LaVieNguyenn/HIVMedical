# Medication Management System for HIV Patients

## 📋 Overview
Comprehensive medication management system specifically designed for HIV patients, focusing on ARV (Antiretroviral) therapy management, adherence tracking, and medication scheduling.

## 🚀 Features Implemented

### ✅ Core Medication Management
- **Medication Database** - Complete ARV and HIV medication catalog
- **Prescription Management** - Doctor prescribing workflow
- **Patient Medication Tracking** - Current and historical medications
- **Medication Scheduling** - Daily medication schedules
- **Adherence Monitoring** - Track medication compliance
- **Refill Management** - Track refills and due dates

### ✅ HIV-Specific Features
- **ARV Medication Categories** - NRTI, NNRTI, PI, INSTI classifications
- **Drug Interaction Checking** - Built-in interaction warnings
- **Side Effect Tracking** - Monitor and report adverse effects
- **Treatment Regimen Management** - Complete HIV treatment protocols

## 🏗️ Architecture

### Database Schema
```
Medications
├── Basic Info (Name, Generic, Brand)
├── HIV Classification (Category, Type)
├── Clinical Data (Strength, Form, Description)
├── Safety Info (Side Effects, Contraindications)
└── Storage Instructions

PatientMedications
├── Prescription Details (Doctor, Date, Dosage)
├── Treatment Period (Start/End dates)
├── Status Tracking (Active, Completed, Discontinued)
├── Adherence Stats (Percentage, Missed doses)
└── Refill Management

MedicationSchedules
├── Timing (Scheduled time, Day of week)
├── Instructions (Special notes)
└── Reminder Settings

MedicationAdherence
├── Scheduled vs Actual times
├── Status (Taken, Missed, Late, Skipped)
├── Reasons for non-adherence
└── Side effects reported
```

## 🔐 Security & Permissions

### Role-based Access Control
- **Admin**: Full medication database management
- **Doctor**: Prescribe, view all patient medications, update status
- **Patient**: View own medications, report adherence, view schedules

### Permission Matrix
| Action | Patient | Doctor | Admin |
|--------|---------|--------|-------|
| View Medications | ✅ (catalog) | ✅ (all) | ✅ (all) |
| Create Medication | ❌ | ✅ | ✅ |
| Prescribe Medication | ❌ | ✅ | ✅ |
| View Own Medications | ✅ | ✅ (patients) | ✅ (all) |
| Update Prescription | ❌ | ✅ | ✅ |
| Discontinue Medication | ❌ | ✅ | ✅ |
| Report Adherence | ✅ | ✅ | ✅ |

## 🌐 API Endpoints

### Medication Catalog
```http
GET /api/medication - All active medications
GET /api/medication/hiv - HIV-specific medications
GET /api/medication/arv - ARV medications only
GET /api/medication/search?searchTerm={term} - Search medications
GET /api/medication/filter - Advanced filtering
POST /api/medication - Create new medication (Doctor/Admin)
PUT /api/medication/{id} - Update medication (Doctor/Admin)
DELETE /api/medication/{id} - Deactivate medication (Admin)
```

### Patient Medications
```http
GET /api/patientmedication/my-medications - Patient's all medications
GET /api/patientmedication/my-current-medications - Current medications
GET /api/patientmedication/my-summary - Medication summary with stats
GET /api/patientmedication/patient/{id} - Patient medications (Doctor/Admin)
GET /api/patientmedication/patient/{id}/current - Current medications
GET /api/patientmedication/patient/{id}/summary - Patient summary
POST /api/patientmedication/prescribe - Prescribe medication (Doctor/Admin)
PUT /api/patientmedication/{id} - Update prescription (Doctor/Admin)
PATCH /api/patientmedication/{id}/discontinue - Discontinue medication
GET /api/patientmedication/filter - Filter with permissions
```

## 📊 Key Features

### 1. **HIV Medication Categories**
- **NRTI** (Nucleoside Reverse Transcriptase Inhibitors)
- **NNRTI** (Non-Nucleoside Reverse Transcriptase Inhibitors)
- **PI** (Protease Inhibitors)
- **INSTI** (Integrase Strand Transfer Inhibitors)
- **Entry/Fusion Inhibitors**

### 2. **Prescription Workflow**
```
Doctor → Select Patient → Choose Medication → Set Dosage/Frequency → Add Instructions → Prescribe
```

### 3. **Adherence Tracking**
- Daily medication schedules
- Taken/Missed/Late/Skipped status
- Adherence percentage calculation
- Missed dose reasons tracking
- Side effects reporting

### 4. **Refill Management**
- Track remaining refills
- Next refill due dates
- Automatic refill reminders
- Refill history

### 5. **Drug Safety**
- Drug interaction warnings
- Contraindication checks
- Side effect monitoring
- Storage instructions

## 📝 Usage Examples

### 1. Prescribe ARV Medication
```json
POST /api/patientmedication/prescribe
{
  "patientId": 1,
  "medicationId": 1,
  "prescribedDate": "2024-12-20T00:00:00Z",
  "startDate": "2024-12-21T00:00:00Z",
  "dosage": "1 tablet",
  "frequency": "Once daily",
  "instructions": "Take at bedtime with food",
  "notes": "Monitor for CNS side effects",
  "refillsRemaining": 5,
  "nextRefillDue": "2025-01-20T00:00:00Z"
}
```

### 2. Get Patient Medication Summary
```json
GET /api/patientmedication/my-summary
Response: {
  "totalMedications": 3,
  "activeMedications": 2,
  "overallAdherencePercentage": 95.5,
  "totalMissedDoses": 2,
  "currentMedications": [...],
  "upcomingRefills": [...]
}
```

### 3. Filter ARV Medications
```http
GET /api/medication/filter?category=ARV&medicationType=NNRTI&form=Tablet
```

## 🧪 Testing

### Test Coverage
- ✅ Medication CRUD operations
- ✅ Prescription workflow
- ✅ Permission validation
- ✅ Duplicate prevention
- ✅ Data validation
- ✅ Error handling

### Test File
`Patient.Api/MedicationManagement.http` - 26 comprehensive test scenarios

## 📈 Business Rules

### Validation Rules
- ❌ Cannot prescribe same medication twice to same patient (active)
- ❌ Cannot prescribe to non-existent patient
- ❌ Cannot prescribe non-existent medication
- ❌ Cannot create duplicate medication names
- ✅ Soft delete for medications (mark inactive)
- ✅ Track all prescription changes with audit trail

### Adherence Calculation
```
Adherence % = (Taken Doses / Total Scheduled Doses) × 100
```

### Status Flow
```
Prescribed → Active → Completed/Discontinued
```

## 🔄 Integration Points

### With Other Services
- **Auth Service**: Doctor/Patient authentication
- **Appointment Service**: Link medications to appointments
- **Medical Records**: Medication history in records

### Future Integrations
- **Notification Service**: Medication reminders
- **Lab Service**: Monitor drug levels
- **Pharmacy Service**: Prescription fulfillment

## 🚀 Getting Started

### Prerequisites
- Patient Service running
- Database updated with medication tables
- JWT authentication configured

### Sample HIV Medications
The system supports common HIV medications:
- **Atripla** (Efavirenz/Tenofovir/Emtricitabine)
- **Tivicay** (Dolutegravir)
- **Truvada** (Tenofovir/Emtricitabine)
- **Prezista** (Darunavir)
- And many more...

### Quick Start
1. Create medications using admin account
2. Doctors can prescribe to patients
3. Patients can view their medications
4. Track adherence and refills

## 📞 Support
For issues or questions about medication management, refer to the main project documentation.

## 🔮 Future Enhancements
- Medication interaction checker
- Automated adherence reminders
- Integration with pharmacy systems
- Mobile app for medication tracking
- AI-powered side effect monitoring
- Telemedicine integration for medication consultations
