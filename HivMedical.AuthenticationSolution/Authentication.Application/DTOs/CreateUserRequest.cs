using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.DTOs
{
    public class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? UserName { get; set; }

        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public byte Gender { get; set; } = 0;

        [Required]
        [Range(1, 5)]
        public int RoleId { get; set; }

        public bool IsAnonymous { get; set; } = false;
    }
}
