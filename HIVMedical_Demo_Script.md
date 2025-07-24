# HIVMedical System - Complete Demo Script

## Overview
This demo script provides a comprehensive walkthrough of the HIVMedical microservices system, demonstrating key features through API calls and real-world scenarios.

## System Architecture Quick Reference
- **API Gateway**: `http://localhost:5000` (Routes to all services)
- **Authentication Service**: `http://localhost:5001`
- **Patient Service**: `http://localhost:7030`
- **RabbitMQ**: `localhost:15672` (Management UI)
- **SQL Server**: `localhost:1433` (Auth & Patient DBs)

---

## Pre-Demo Setup

### 1. Start the System
```bash
# Navigate to project root
cd D:\HIV\HIVMedical

# Start all services using Docker Compose
docker-compose up -d

# Verify all containers are running
docker-compose ps
```

### 2. Verify Service Health
```http
GET http://localhost:5000/health
GET http://localhost:5001/health
GET http://localhost:7030/health
```

---

## Demo Scenario: Complete Patient Journey

### Phase 1: User Registration & Authentication

#### 1.1 Register a Patient
```http
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
    "username": "john_patient",
    "email": "john.doe@example.com",
    "password": "SecurePass123!",
    "firstName": "John",
    "lastName": "Doe",
    "role": "Patient"
}
```

**Expected Response**: 201 Created with user details

#### 1.2 Register a Doctor
```http
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
    "username": "dr_smith",
    "email": "dr.smith@hivmedical.com", 
    "password": "DocPass123!",
    "firstName": "Sarah",
    "lastName": "Smith",
    "role": "Doctor"
}
```

#### 1.3 Patient Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
    "username": "john_patient",
    "password": "SecurePass123!"
}
```

**Expected Response**: JWT token (save this as `PATIENT_TOKEN`)

#### 1.4 Doctor Login
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
    "username": "dr_smith",
    "password": "DocPass123!"
}
```

**Expected Response**: JWT token (save this as `DOCTOR_TOKEN`)

---

### Phase 2: Patient Profile Management

#### 2.1 Get Patient Profile
```http
GET http://localhost:5000/api/auth/profile
Authorization: Bearer {{PATIENT_TOKEN}}
```

#### 2.2 Update Patient Profile
```http
PUT http://localhost:5000/api/auth/profile
Authorization: Bearer {{PATIENT_TOKEN}}
Content-Type: application/json

{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1234567890",
    "dateOfBirth": "1985-06-15",
    "address": "123 Main St, City, State 12345"
}
```

---

### Phase 3: Medication Management

#### 3.1 Browse Available Medications (Doctor View)
```http
GET http://localhost:5000/api/medication
Authorization: Bearer {{DOCTOR_TOKEN}}
```

#### 3.2 Search HIV Medications
```http
GET http://localhost:5000/api/medication/hiv
Authorization: Bearer {{DOCTOR_TOKEN}}
```

#### 3.3 Search ARV Medications
```http
GET http://localhost:5000/api/medication/arv
Authorization: Bearer {{DOCTOR_TOKEN}}
```

#### 3.4 Search Medications by Name
```http
GET http://localhost:5000/api/medication/search?searchTerm=efavirenz
Authorization: Bearer {{DOCTOR_TOKEN}}
```

#### 3.5 Doctor Prescribes Medication to Patient
```http
POST http://localhost:5000/api/patientmedication/prescribe
Authorization: Bearer {{DOCTOR_TOKEN}}
Content-Type: application/json

{
    "patientId": 1,
    "medicationId": 1,
    "prescribedDate": "2024-01-15",
    "startDate": "2024-01-16",
    "dosage": "600mg",
    "frequency": "Once daily",
    "instructions": "Take with food in the evening",
    "duration": "90 days",
    "refillsAllowed": 5
}
```

#### 3.6 Patient Views Current Medications
```http
GET http://localhost:5000/api/patientmedication/my-current-medications
Authorization: Bearer {{PATIENT_TOKEN}}
```

#### 3.7 Patient Views All Medications History
```http
GET http://localhost:5000/api/patientmedication/my-medications
Authorization: Bearer {{PATIENT_TOKEN}}
```

#### 3.8 Patient Medication Summary
```http
GET http://localhost:5000/api/patientmedication/my-summary
Authorization: Bearer {{PATIENT_TOKEN}}
```

---

### Phase 4: Appointment Management

#### 4.1 Patient Books Appointment
```http
POST http://localhost:5000/api/appointment
Authorization: Bearer {{PATIENT_TOKEN}}
Content-Type: application/json

{
    "doctorId": 2,
    "appointmentDate": "2024-02-15T10:00:00Z",
    "appointmentType": "Routine Checkup",
    "reason": "Monthly HIV monitoring visit",
    "notes": "Patient experiencing some fatigue"
}
```

#### 4.2 Patient Views Upcoming Appointments
```http
GET http://localhost:5000/api/appointment/upcoming?days=30
Authorization: Bearer {{PATIENT_TOKEN}}
```

#### 4.3 Patient Views All Appointments
```http
GET http://localhost:5000/api/appointment/my-appointments
Authorization: Bearer {{PATIENT_TOKEN}}
```

#### 4.4 Patient Appointment Summary
```http
GET http://localhost:5000/api/appointment/my-summary
Authorization: Bearer {{PATIENT_TOKEN}}
```

#### 4.5 Doctor Views Today's Appointments
```http
GET http://localhost:5000/api/appointment/today
Authorization: Bearer {{DOCTOR_TOKEN}}
```

#### 4.6 Doctor Views Their Schedule
```http
GET http://localhost:5000/api/appointment/doctor/2
Authorization: Bearer {{DOCTOR_TOKEN}}
```

#### 4.7 Doctor Updates Appointment Status
```http
PATCH http://localhost:5000/api/appointment/1/status
Authorization: Bearer {{DOCTOR_TOKEN}}
Content-Type: application/json

{
    "status": "Completed",
    "notes": "Patient vitals stable. CD4 count improved. Continue current medication regimen."
}
```

---

### Phase 5: Medical Records (if implemented)

#### 5.1 Doctor Creates Medical Record
```http
POST http://localhost:5000/api/medicalrecord
Authorization: Bearer {{DOCTOR_TOKEN}}
Content-Type: application/json

{
    "patientId": 1,
    "visitDate": "2024-02-15T10:00:00Z",
    "diagnosis": "HIV-1 infection, stable on HAART",
    "symptoms": "Mild fatigue, otherwise asymptomatic",
    "vitals": {
        "bloodPressure": "120/80",
        "temperature": "98.6°F",
        "heartRate": "72 bpm",
        "weight": "75 kg"
    },
    "labResults": {
        "cd4Count": 650,
        "viralLoad": "Undetectable (<20 copies/mL)",
        "hemoglobin": "14.2 g/dL"
    },
    "treatmentNotes": "Patient responding well to current ART regimen"
}
```

#### 5.2 Patient Views Medical Records
```http
GET http://localhost:5000/api/medicalrecord/my-records
Authorization: Bearer {{PATIENT_TOKEN}}
```

---

### Phase 6: Administrative Functions

#### 6.1 Admin Views All Patients (Admin Token Required)
```http
GET http://localhost:5000/api/admin/patients
Authorization: Bearer {{ADMIN_TOKEN}}
```

#### 6.2 Admin Views System Statistics
```http
GET http://localhost:5000/api/admin/statistics
Authorization: Bearer {{ADMIN_TOKEN}}
```

---

## Demo Presentation Flow

### 1. System Overview (5 minutes)
- Show architecture diagram
- Explain microservices approach
- Demonstrate service independence

### 2. Authentication Demo (5 minutes)
- Register users (Patient & Doctor)
- Login and obtain tokens
- Show JWT token structure

### 3. Patient Journey Demo (10 minutes)
- Patient profile management
- View available medications
- Book appointment
- View appointment history

### 4. Doctor Workflow Demo (10 minutes)
- Doctor login
- View patient list
- Prescribe medications
- Manage appointments
- Update appointment status
- Add medical records

### 5. System Integration Demo (5 minutes)
- Show real-time updates
- Demonstrate service communication
- Show message broker activity (RabbitMQ)

---

## Advanced Demo Scenarios

### Scenario A: Emergency Appointment
```http
POST http://localhost:5000/api/appointment
Authorization: Bearer {{PATIENT_TOKEN}}
Content-Type: application/json

{
    "doctorId": 2,
    "appointmentDate": "2024-01-20T14:00:00Z",
    "appointmentType": "Emergency",
    "reason": "Severe side effects from medication",
    "priority": "High",
    "notes": "Patient experiencing nausea and dizziness"
}
```

### Scenario B: Medication Adjustment
```http
PUT http://localhost:5000/api/patientmedication/1
Authorization: Bearer {{DOCTOR_TOKEN}}
Content-Type: application/json

{
    "dosage": "400mg",
    "frequency": "Twice daily",
    "instructions": "Take with food, morning and evening",
    "adjustmentReason": "Reducing dosage due to side effects"
}
```

### Scenario C: Discontinue Medication
```http
PATCH http://localhost:5000/api/patientmedication/1/discontinue
Authorization: Bearer {{DOCTOR_TOKEN}}
Content-Type: application/json

{
    "discontinueDate": "2024-01-25",
    "reason": "Severe adverse reaction",
    "notes": "Patient developed rash and liver enzyme elevation"
}
```

---

## Testing & Validation

### 1. Positive Test Cases
- All endpoints return expected HTTP status codes
- JWT authentication works correctly
- Data persistence across service restarts
- Cross-service communication functions

### 2. Negative Test Cases
- Invalid authentication tokens
- Unauthorized access attempts
- Invalid data formats
- Non-existent resource requests

### 3. Performance Tests
- Response time under load
- Concurrent user handling
- Database query optimization
- Message broker throughput

---

## Demo Environment Requirements

### Hardware
- Minimum 8GB RAM
- 4 CPU cores
- 20GB available disk space

### Software
- Docker Desktop
- .NET 8 SDK
- SQL Server Management Studio (optional)
- API testing tool (Postman/Insomnia/Thunder Client)

### Network Ports
- 5000: API Gateway
- 5001: Authentication Service
- 7030: Patient Service
- 1433: SQL Server
- 5672: RabbitMQ
- 15672: RabbitMQ Management UI

---

## Troubleshooting

### Common Issues
1. **Port conflicts**: Ensure ports are available
2. **Database connection**: Verify SQL Server is running
3. **JWT expiration**: Refresh tokens as needed
4. **Service discovery**: Check docker-compose networking

### Debug Commands
```bash
# Check service logs
docker-compose logs auth-service
docker-compose logs patient-service
docker-compose logs api-gateway

# Restart specific service
docker-compose restart auth-service

# View database connections
docker exec -it sqlserver-auth sqlcmd -S localhost -U sa -P "YourPassword123!"
```

---

## Demo Script Summary

This demo showcases a complete HIV medical management system with:
- ✅ Secure authentication and authorization
- ✅ Patient and doctor role management
- ✅ Comprehensive medication management
- ✅ Appointment scheduling and tracking
- ✅ Medical record management
- ✅ Microservices architecture
- ✅ Real-time communication via message broker
- ✅ RESTful API design
- ✅ Scalable and maintainable codebase

The system demonstrates modern healthcare software architecture principles while addressing the specific needs of HIV patient care and treatment monitoring.
