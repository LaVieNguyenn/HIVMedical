-- ============================
-- DATABASE: HivMedicalDB
-- ============================
CREATE DATABASE HivMedicalDB
GO
USE HivMedicalDB
GO

-- ============================
-- 1. Roles - Phân quyền người dùng (Guest, Customer, Doctor, Admin,...)
-- ============================
CREATE TABLE roles (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(50) NOT NULL,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT
);

-- ============================
-- 2. Users - Thông tin người dùng (có thể là bệnh nhân, bác sĩ, quản trị viên...)
-- ============================
CREATE TABLE users (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(100) NULL,
    password_hash VARCHAR(255) NULL,
    full_name VARCHAR(255),
    gender TINYINT,
    date_of_birth DATE,
    phone VARCHAR(20),
    email VARCHAR(100),
    is_anonymous BIT,
    role_id INT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);

-- ============================
-- 3. Specializations & Qualifications - Chuyên môn và bằng cấp của bác sĩ
-- ============================
CREATE TABLE specializations (
    specialization_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT
);

CREATE TABLE qualifications (
    qualification_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT
);

-- ============================
-- 4. Doctors - Thông tin chi tiết về bác sĩ
-- ============================
CREATE TABLE doctors (
    doctor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);

CREATE TABLE doctor_specializations (
    id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT,
    specialization_id INT,
    created_at DATETIME, 
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (doctor_id) REFERENCES doctors(doctor_id),
    FOREIGN KEY (specialization_id) REFERENCES specializations(specialization_id)
);

CREATE TABLE doctor_qualifications (
    id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT,
    qualification_id INT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (doctor_id) REFERENCES doctors(doctor_id),
    FOREIGN KEY (qualification_id) REFERENCES qualifications(qualification_id)
);

CREATE TABLE doctor_schedules (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    doctor_id INT NOT NULL,
    weekday TINYINT NOT NULL, -- 1: Monday, 2: Tuesday, ..., 7: Sunday
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    location VARCHAR(255),
    note TEXT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (doctor_id) REFERENCES doctors(doctor_id)
);

-- ============================
-- 5. ARV Protocols - Phác đồ điều trị HIV
-- ============================
CREATE TABLE arv_protocols (
    protocol_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100),
    description TEXT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT
);

CREATE TABLE arv_protocol_components (
    component_id INT PRIMARY KEY IDENTITY(1,1),
    protocol_id INT,
    drug_name VARCHAR(100),
    target_group VARCHAR(100),
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (protocol_id) REFERENCES arv_protocols(protocol_id)
);

-- ============================
-- 6. Appointments - Quản lý lịch hẹn giữa bệnh nhân và bác sĩ
-- ============================
CREATE TABLE appointments (
    appointment_id INT PRIMARY KEY IDENTITY(1,1),
    patient_id INT,
    doctor_id INT,
    appointment_date DATETIME,
    notes TEXT,
    is_follow_up BIT,
    is_anonymous BIT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (patient_id) REFERENCES users(user_id),
    FOREIGN KEY (doctor_id) REFERENCES doctors(doctor_id)
);

-- ============================
-- 7. Treatments - Ghi nhận điều trị thực tế của bệnh nhân
-- ============================
CREATE TABLE treatments (
    treatment_id INT PRIMARY KEY IDENTITY(1,1),
    patient_id INT,
    doctor_id INT,
    start_date DATE,
    end_date DATE,
    protocol_id INT,
    notes TEXT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (patient_id) REFERENCES users(user_id),
    FOREIGN KEY (doctor_id) REFERENCES doctors(doctor_id),
    FOREIGN KEY (protocol_id) REFERENCES arv_protocols(protocol_id)
);

-- ============================
-- 8. Exam Test Types - Danh sách loại xét nghiệm (CD4, Viral Load,...)
-- ============================
CREATE TABLE test_types (
    test_type_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL
);

-- ============================
-- 9. Exam Results - Kết quả xét nghiệm của bệnh nhân
-- ============================
CREATE TABLE exam_results (
    result_id INT PRIMARY KEY IDENTITY(1,1),
    patient_id INT,
    test_type_id INT,
    result_value VARCHAR(50),
    test_date DATE,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (patient_id) REFERENCES users(user_id),
    FOREIGN KEY (test_type_id) REFERENCES test_types(test_type_id)
);

-- ============================
-- 10. Reminder Types - Danh mục loại nhắc nhở (uống thuốc, tái khám...)
-- ============================
CREATE TABLE reminder_types (
    reminder_type_id INT PRIMARY KEY IDENTITY(1,1),
    name VARCHAR(100) NOT NULL
);

-- ============================
-- 11. Reminders - Nhắc nhở uống thuốc, tái khám cho bệnh nhân
-- ============================
CREATE TABLE reminders (
    reminder_id INT PRIMARY KEY IDENTITY(1,1),
    patient_id INT,
    reminder_type_id INT,
    reminder_time DATETIME,
    is_sent BIT,
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (patient_id) REFERENCES users(user_id),
    FOREIGN KEY (reminder_type_id) REFERENCES reminder_types(reminder_type_id)
);

-- ============================
-- 12. Posts - Bài viết chia sẻ, giáo dục, truyền thông
-- ============================
CREATE TABLE posts (
    post_id INT PRIMARY KEY IDENTITY(1,1),
    title VARCHAR(255),
    content TEXT,
    category VARCHAR(100),
    created_at DATETIME,
    created_by INT,
    updated_at DATETIME,
    updated_by INT,
    FOREIGN KEY (created_by) REFERENCES users(user_id)
);


-- Roles
INSERT INTO roles (name, created_at, created_by, updated_at, updated_by) VALUES
('Guest', GETDATE(), 1, GETDATE(), 1),
('Customer', GETDATE(), 1, GETDATE(), 1),
('Staff', GETDATE(), 1, GETDATE(), 1),
('Doctor', GETDATE(), 1, GETDATE(), 1),
('Manager', GETDATE(), 1, GETDATE(), 1),
('Admin', GETDATE(), 1, GETDATE(), 1);

-- Users
INSERT INTO users (username, password_hash, full_name, gender, date_of_birth, phone, email, is_anonymous, role_id, created_at, created_by, updated_at, updated_by) VALUES
('john_doe', 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 'John Doe', 1, '1990-05-12', '1234567890', 'john@example.com', 0, 2, GETDATE(), 1, GETDATE(), 1),
('dr.smith', 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', 'Dr. Anna Smith', 2, '1985-03-20', '9876543210', 'smith@example.com', 0, 4, GETDATE(), 1, GETDATE(), 1),
('anon_user_1', '', NULL, NULL, NULL, NULL, NULL, 1, 1, GETDATE(), 1, GETDATE(), 1);

-- Specializations
INSERT INTO specializations (name, created_at, created_by, updated_at, updated_by) VALUES
('Infectious Diseases', GETDATE(), 1, GETDATE(), 1),
('Pediatrics', GETDATE(), 1, GETDATE(), 1);

-- Qualifications
INSERT INTO qualifications (name, created_at, created_by, updated_at, updated_by) VALUES
('MD', GETDATE(), 1, GETDATE(), 1),
('PhD', GETDATE(), 1, GETDATE(), 1);

-- Doctors
INSERT INTO doctors (user_id, created_at, created_by, updated_at, updated_by) VALUES
(2, GETDATE(), 1, GETDATE(), 1);

-- Doctor Specializations
INSERT INTO doctor_specializations (doctor_id, specialization_id, created_at, created_by, updated_at, updated_by) VALUES
(1, 1, GETDATE(), 1, GETDATE(), 1);

-- Doctor Qualifications
INSERT INTO doctor_qualifications (doctor_id, qualification_id, created_at, created_by, updated_at, updated_by) VALUES
(1, 1, GETDATE(), 1, GETDATE(), 1);

-- Doctor Schedules
INSERT INTO doctor_schedules (doctor_id, weekday, start_time, end_time, location, note, created_at, created_by, updated_at, updated_by) VALUES
(1, 1, '08:00', '12:00', 'Main Clinic Room 1', 'Morning shift', GETDATE(), 1, GETDATE(), 1),
(1, 3, '13:00', '17:00', 'Main Clinic Room 2', 'Afternoon consultations', GETDATE(), 1, GETDATE(), 1);

-- ARV Protocols
INSERT INTO arv_protocols (name, description, created_at, created_by, updated_at, updated_by) VALUES
('TDF + 3TC + DTG', 'Standard first-line treatment for adults', GETDATE(), 1, GETDATE(), 1);

-- ARV Protocol Components
INSERT INTO arv_protocol_components (protocol_id, drug_name, target_group, created_at, created_by, updated_at, updated_by) VALUES
(1, 'Tenofovir (TDF)', 'Adults', GETDATE(), 1, GETDATE(), 1),
(1, 'Lamivudine (3TC)', 'Adults', GETDATE(), 1, GETDATE(), 1),
(1, 'Dolutegravir (DTG)', 'Adults', GETDATE(), 1, GETDATE(), 1);

-- Appointments
INSERT INTO appointments (patient_id, doctor_id, appointment_date, notes, is_follow_up, is_anonymous, created_at, created_by, updated_at, updated_by) VALUES
(1, 1, '2025-05-20 10:00:00', 'Initial consultation', 0, 0, GETDATE(), 1, GETDATE(), 1),
(3, 1, '2025-05-21 09:00:00', 'Anonymous consultation', 0, 1, GETDATE(), 1, GETDATE(), 1);

-- Treatments
INSERT INTO treatments (patient_id, doctor_id, start_date, end_date, protocol_id, notes, created_at, created_by, updated_at, updated_by) VALUES
(1, 1, '2025-05-20', NULL, 1, 'Starting ARV therapy', GETDATE(), 1, GETDATE(), 1);

-- Test Types
INSERT INTO test_types (name) VALUES
('CD4 Count'),
('Viral Load'),
('ARV Resistance');

-- Exam Results
INSERT INTO exam_results (patient_id, test_type_id, result_value, test_date, created_at, created_by, updated_at, updated_by) VALUES
(1, 1, '450', '2025-05-15', GETDATE(), 1, GETDATE(), 1),
(1, 2, 'Undetectable', '2025-05-16', GETDATE(), 1, GETDATE(), 1);

-- Reminder Types
INSERT INTO reminder_types (name) VALUES
('Medication Reminder'),
('Follow-up Appointment');

-- Reminders
INSERT INTO reminders (patient_id, reminder_type_id, reminder_time, is_sent, created_at, created_by, updated_at, updated_by) VALUES
(1, 1, '2025-05-21 08:00:00', 0, GETDATE(), 1, GETDATE(), 1),
(1, 2, '2025-06-20 10:00:00', 0, GETDATE(), 1, GETDATE(), 1);

-- Posts
INSERT INTO posts (title, content, category, created_at, created_by, updated_at, updated_by) VALUES
('Understanding ARV Therapy', 'This article explains the basics of ARV treatment for HIV.', 'Education', GETDATE(), 1, GETDATE(), 1),
('How to manage side effects?', 'Tips for dealing with common side effects of HIV medication.', 'Support', GETDATE(), 1, GETDATE(), 1);
