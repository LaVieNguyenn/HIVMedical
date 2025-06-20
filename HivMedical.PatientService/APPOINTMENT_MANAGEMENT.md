# Appointment Management System

## 📋 Overview
Hệ thống quản lý lịch hẹn cho bệnh nhân HIV với đầy đủ chức năng đặt lịch, xem lịch, hủy/đổi lịch, và xác nhận lịch hẹn.

## 🚀 Features Implemented

### ✅ Core Features
- **Đặt lịch hẹn với bác sĩ** - Bệnh nhân có thể đặt lịch hẹn
- **Xem lịch hẹn của bệnh nhân** - Xem tất cả lịch hẹn theo bệnh nhân
- **Hủy/đổi lịch hẹn** - Cập nhật thông tin lịch hẹn
- **Xác nhận lịch hẹn** - Bác sĩ có thể cập nhật trạng thái lịch hẹn
- **Lọc lịch hẹn theo trạng thái, ngày** - Tìm kiếm và lọc nâng cao

### ✅ Additional Features
- **Conflict Detection** - Kiểm tra trùng lịch bác sĩ
- **Permission Control** - Phân quyền truy cập theo role
- **Appointment Summary** - Thống kê tổng quan lịch hẹn
- **Upcoming Appointments** - Lịch hẹn sắp tới
- **Today's Schedule** - Lịch hẹn hôm nay cho bác sĩ

## 🏗️ Architecture

### Entities
- **Appointment** - Entity chính cho lịch hẹn
- **Patient** - Liên kết với bệnh nhân
- **Navigation Properties** - Quan hệ 1-nhiều

### Services & Controllers
- **AppointmentService** - Business logic
- **AppointmentController** - API endpoints
- **AppointmentRepository** - Data access

## 📊 Database Schema

```sql
Appointments Table:
- Id (int, PK)
- PatientId (int, FK)
- DoctorId (int, FK to Auth service)
- AppointmentDate (datetime)
- AppointmentTime (time)
- AppointmentType (nvarchar) - Consultation, Lab Test, Follow-up
- Status (nvarchar) - Scheduled, Completed, Cancelled, No-Show
- Notes (nvarchar, nullable)
- Reason (nvarchar, nullable)
- CreatedAt, UpdatedAt (datetime)
```

## 🔐 Security & Permissions

### Role-based Access Control
- **Admin**: Full access to all appointments
- **Doctor**: Can view/manage appointments where they are the doctor
- **Patient**: Can only view/manage their own appointments

### Permission Matrix
| Action | Patient | Doctor | Admin |
|--------|---------|--------|-------|
| Create Appointment | ✅ (own) | ✅ (any) | ✅ (any) |
| View Appointment | ✅ (own) | ✅ (own patients) | ✅ (all) |
| Update Appointment | ✅ (own) | ✅ (own patients) | ✅ (all) |
| Update Status | ❌ | ✅ | ✅ |
| Delete Appointment | ✅ (own, scheduled only) | ✅ (own patients) | ✅ (all) |

## 🌐 API Endpoints

### Patient Endpoints
```http
GET /api/appointment/my-appointments - Xem lịch hẹn của tôi
GET /api/appointment/my-summary - Tóm tắt lịch hẹn
GET /api/appointment/upcoming?days=7 - Lịch hẹn sắp tới
POST /api/appointment - Đặt lịch hẹn mới
PUT /api/appointment/{id} - Cập nhật lịch hẹn
DELETE /api/appointment/{id} - Hủy lịch hẹn
```

### Doctor Endpoints
```http
GET /api/appointment/doctor/{doctorId} - Lịch hẹn của bác sĩ
GET /api/appointment/today - Lịch hẹn hôm nay
PATCH /api/appointment/{id}/status - Cập nhật trạng thái
```

### Admin Endpoints
```http
GET /api/appointment/filter - Lọc lịch hẹn nâng cao
GET /api/appointment/patient/{patientId} - Lịch hẹn của bệnh nhân
```

## 📝 Usage Examples

### 1. Đặt lịch hẹn mới
```json
POST /api/appointment
{
  "patientId": 1,
  "doctorId": 2,
  "appointmentDate": "2024-12-25T00:00:00Z",
  "appointmentTime": "09:00:00",
  "appointmentType": "Consultation",
  "reason": "Regular HIV checkup",
  "notes": "Patient requested morning appointment"
}
```

### 2. Lọc lịch hẹn
```http
GET /api/appointment/filter?status=Scheduled&appointmentType=Lab Test&fromDate=2024-12-01&toDate=2024-12-31
```

### 3. Cập nhật trạng thái lịch hẹn
```json
PATCH /api/appointment/1/status
{
  "status": "Completed",
  "notes": "Patient attended appointment. All vitals normal."
}
```

## 🔧 Business Rules

### Validation Rules
- ❌ Không thể đặt lịch trong quá khứ
- ❌ Bác sĩ không thể có 2 lịch hẹn cùng thời điểm
- ❌ Chỉ có thể xóa lịch hẹn đang "Scheduled"
- ❌ Không thể cập nhật lịch hẹn đã "Completed" hoặc "Cancelled"

### Status Flow
```
Scheduled → Completed
Scheduled → Cancelled  
Scheduled → No-Show
```

## 🧪 Testing

### Test File
- `Patient.Api/Appointments.http` - Comprehensive API tests
- Includes all CRUD operations
- Tests permission scenarios
- Validates business rules

### Test Scenarios
1. ✅ Create appointments (various types)
2. ✅ Conflict detection
3. ✅ Past date validation
4. ✅ Permission checks
5. ✅ Status updates
6. ✅ Filtering and search

## 🚀 Getting Started

### Prerequisites
- .NET 8.0
- SQL Server
- JWT Authentication setup

### Run the Service
```bash
cd HIVMedical/HivMedical.PatientService
dotnet run --project Patient.Api
```

Service will be available at: `http://localhost:5072`

### Swagger Documentation
Visit: `http://localhost:5072/swagger`

## 📈 Next Steps

### Potential Enhancements
1. **Notification System** - Email/SMS reminders
2. **Recurring Appointments** - Weekly/monthly schedules
3. **Appointment Templates** - Pre-defined appointment types
4. **Calendar Integration** - Export to Google Calendar
5. **Waiting List** - Queue management for cancelled slots
6. **Appointment History** - Detailed audit trail

## 🐛 Known Issues
- None currently identified

## 📞 Support
For issues or questions, please refer to the main project documentation.
