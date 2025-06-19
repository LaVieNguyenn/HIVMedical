using System;

namespace Authentication.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Role { get; set; }
        
        // Thông tin profile
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? BloodType { get; set; }
        public string? Allergies { get; set; }
        public string? ChronicConditions { get; set; }
    }
} 