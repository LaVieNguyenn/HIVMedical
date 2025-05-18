# HivMedical System

HivMedical là một hệ thống Microservice được xây dựng nhằm hỗ trợ quản lý và tiếp cận dịch vụ y tế cho bệnh nhân HIV, giúp cải thiện quá trình điều trị, giảm kỳ thị, và cung cấp nền tảng kết nối giữa bệnh nhân, bác sĩ và nhân viên y tế.

## 🎯 Mục tiêu chính

* Cho phép người dùng đăng ký, đăng nhập và sử dụng hệ thống một cách bảo mật
* Quản lý lịch hẹn, lịch tái khám và phác đồ điều trị
* Cung cấp nội dung giáo dục và hỗ trợ trực tuyến giữa bệnh nhân và bác sĩ
* Hỗ trợ truy xuất lịch sử khám, nhắc uống thuốc và phân quyền theo vai trò (Guest, Customer, Doctor, Staff, Manager, Admin)

## 🏗️ Kiến trúc tổng thể

Dự án áp dụng đồng thời **Clean Architecture** và **Microservices**. Mỗi service đều tách biệt, dễ triển khai độc lập và mở rộng.

### Các service chính:

* `AuthenticationService`: xử lý login, đăng ký, sinh JWT
* `DoctorService`: quản lý thông tin bác sĩ, lịch làm việc, bằng cấp
* `AppointmentService`: xử lý lịch hẹn, nhắc nhở tái khám
* `Updating...`

## 🧰 Công nghệ sử dụng

| Thành phần | Công nghệ                                         |
| ---------- | ------------------------------------------------- |
| Backend    | .NET 8 Web API, Clean Architecture, Microservice  |
| Frontend   | Angular                                           |
| Database   | SQL Server (DB First)                             |
| Auth       | JWT Token                                         |
| ORM        | Entity Framework Core                             |
| Docs       | Swagger UI                                        |
| Di         | Microsoft.Extensions.DependencyInjection          |

## 📁 Các thư mục chính (ví dụ với AuthService)

```
AuthenticationService/
├── Auth.Api/              <-- API endpoint + Swagger + JWT middleware
├── Auth.Application/      <-- DTOs, Services, UseCases
├── Auth.Domain/           <-- Entities, Interface, Business logic
├── Auth.Infrastructure/   <-- EF DbContext, Repositories, UoW
├── SharedKernel/          <-- BaseEntity, ValueObjects...
├── SharedLibrary/         <-- JwtService, ApiResponse, IGenericRepo
```

## 🧪 Test API

Dùng file `ApiApi.Presentation.http` trong thư mục `.Api` để test các endpoint `Get`, `Post`, ...

## 📌 Lưu ý

* Mỗi Microservice có DB riêng và file Database được lưu trong folder db
* SharedLibrary và SharedKernel không nên chứa logic đặc thù 1 service
---
