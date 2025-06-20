# RabbitMQ Events Integration - Patient Service

## 📋 Overview
Patient Service đã được tích hợp với RabbitMQ để publish các domain events quan trọng. Các events này có thể được consume bởi các services khác trong hệ thống.

## 🚀 Events đã được implement

### ✅ **Patient Events**
1. **PatientCreatedEvent** - Khi tạo bệnh nhân mới
2. **PatientUpdatedEvent** - Khi cập nhật thông tin bệnh nhân

### ✅ **Appointment Events**
1. **AppointmentCreatedEvent** - Khi tạo lịch hẹn mới
2. **AppointmentStatusChangedEvent** - Khi thay đổi trạng thái lịch hẹn
3. **AppointmentCancelledEvent** - Khi hủy lịch hẹn

### ✅ **Medication Events**
1. **MedicationPrescribedEvent** - Khi kê đơn thuốc mới
2. **MedicationDiscontinuedEvent** - Khi ngừng thuốc
3. **MedicationAdherenceRecordedEvent** - Khi ghi nhận tuân thủ thuốc
4. **MedicationRefillDueEvent** - Khi đến hạn tái cấp thuốc

### ✅ **Medical Record Events**
1. **MedicalRecordCreatedEvent** - Khi tạo hồ sơ y tế mới
2. **LabResultRecordedEvent** - Khi ghi nhận kết quả xét nghiệm
3. **TreatmentUpdateRecordedEvent** - Khi cập nhật điều trị

## 🔧 Configuration

### RabbitMQ Settings (appsettings.json)
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest",
    "Port": "5672"
  }
}
```

### Docker RabbitMQ
```yaml
rabbitmq:
  image: rabbitmq:3-management
  container_name: hiv-rabbitmq
  ports:
    - "5672:5672"    # RabbitMQ service
    - "15672:15672"  # Management interface
  environment:
    - RABBITMQ_DEFAULT_USER=guest
    - RABBITMQ_DEFAULT_PASS=guest
```

## 📊 Queue Names
Các queue được tạo tự động với tên giống với tên Event class:

### Patient Queues:
- `PatientCreatedEvent`
- `PatientUpdatedEvent`

### Appointment Queues:
- `AppointmentCreatedEvent`
- `AppointmentStatusChangedEvent`
- `AppointmentCancelledEvent`

### Medication Queues:
- `MedicationPrescribedEvent`
- `MedicationDiscontinuedEvent`
- `MedicationAdherenceRecordedEvent`
- `MedicationRefillDueEvent`

### Medical Record Queues:
- `MedicalRecordCreatedEvent`
- `LabResultRecordedEvent`
- `TreatmentUpdateRecordedEvent`

## 🔍 Monitoring RabbitMQ

### RabbitMQ Management Interface
- **URL**: http://localhost:15672
- **Username**: guest
- **Password**: guest

### Kiểm tra Queues
1. Truy cập RabbitMQ Management UI
2. Vào tab "Queues"
3. Xem các queue đã được tạo và số lượng messages

### Kiểm tra Messages
1. Click vào queue name
2. Xem "Get messages" để đọc message content
3. Kiểm tra message payload và properties

## 📝 Event Payload Examples

### PatientCreatedEvent
```json
{
  "PatientId": 1,
  "PatientCode": "HIV001",
  "FullName": "Nguyen Van A",
  "Email": "nguyenvana@email.com",
  "Phone": "0123456789",
  "DateOfBirth": "1990-01-01T00:00:00Z",
  "Gender": 1,
  "AuthUserId": 123,
  "DiagnosisDate": "2024-01-01T00:00:00Z",
  "HIVStatus": "Positive",
  "TreatmentStatus": "On Treatment",
  "CreatedAt": "2024-12-20T10:30:00Z"
}
```

### AppointmentCreatedEvent
```json
{
  "AppointmentId": 1,
  "PatientId": 1,
  "PatientName": "Nguyen Van A",
  "PatientCode": "HIV001",
  "DoctorId": 2,
  "AppointmentDate": "2024-12-25T00:00:00Z",
  "AppointmentTime": "09:00:00",
  "AppointmentType": "Consultation",
  "Reason": "Regular HIV checkup",
  "Notes": "Patient requested morning appointment",
  "CreatedAt": "2024-12-20T10:30:00Z"
}
```

### MedicationPrescribedEvent
```json
{
  "PatientMedicationId": 1,
  "PatientId": 1,
  "PatientName": "Nguyen Van A",
  "PatientCode": "HIV001",
  "MedicationId": 1,
  "MedicationName": "Efavirenz/Tenofovir/Emtricitabine",
  "MedicationType": "NNRTI + NRTI",
  "Category": "ARV",
  "PrescribedByDoctorId": 2,
  "PrescribedDate": "2024-12-20T00:00:00Z",
  "StartDate": "2024-12-21T00:00:00Z",
  "EndDate": null,
  "Dosage": "1 tablet",
  "Frequency": "Once daily",
  "Instructions": "Take at bedtime with food",
  "Notes": "Monitor for CNS side effects",
  "CreatedAt": "2024-12-20T10:30:00Z"
}
```

## 🔄 Integration Scenarios

### Notification Service Integration
```csharp
// Notification Service có thể subscribe các events:
- AppointmentCreatedEvent → Send appointment confirmation
- MedicationPrescribedEvent → Send medication instructions
- MedicationRefillDueEvent → Send refill reminders
- LabResultRecordedEvent → Send result notifications
```

### Analytics Service Integration
```csharp
// Analytics Service có thể subscribe:
- PatientCreatedEvent → Update patient statistics
- MedicationAdherenceRecordedEvent → Calculate adherence rates
- AppointmentStatusChangedEvent → Track appointment patterns
```

### Audit Service Integration
```csharp
// Audit Service có thể subscribe tất cả events để:
- Track all patient activities
- Maintain audit trail
- Compliance reporting
```

## 🧪 Testing Events

### Manual Testing
1. Tạo patient mới → Check `PatientCreatedEvent` queue
2. Đặt lịch hẹn → Check `AppointmentCreatedEvent` queue
3. Kê đơn thuốc → Check `MedicationPrescribedEvent` queue

### Automated Testing
```csharp
// Test event publishing
[Test]
public async Task CreatePatient_ShouldPublishPatientCreatedEvent()
{
    // Arrange
    var request = new CreatePatientRequest { ... };
    
    // Act
    await patientService.CreatePatientAsync(request);
    
    // Assert
    // Verify event was published to RabbitMQ
}
```

## 🚀 Next Steps

### Event Handlers Implementation
1. **Notification Service** - Subscribe và gửi notifications
2. **Analytics Service** - Subscribe và tính toán metrics
3. **Audit Service** - Subscribe và lưu audit logs

### Event Versioning
1. Implement event versioning strategy
2. Handle backward compatibility
3. Event schema evolution

### Error Handling
1. Dead letter queues
2. Retry mechanisms
3. Event replay capabilities

## 🐛 Troubleshooting

### Common Issues
1. **RabbitMQ Connection Failed**
   - Check Docker container status
   - Verify connection settings
   - Check network connectivity

2. **Events Not Published**
   - Check service logs
   - Verify EventBus registration
   - Check RabbitMQ permissions

3. **Queue Not Created**
   - Check RabbitMQ Management UI
   - Verify queue declaration
   - Check exchange bindings

### Debug Commands
```bash
# Check RabbitMQ container
docker ps | grep rabbitmq

# Check RabbitMQ logs
docker logs hiv-rabbitmq

# Check queue status
curl -u guest:guest http://localhost:15672/api/queues
```
