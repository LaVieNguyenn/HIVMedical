using System.ComponentModel.DataAnnotations;

namespace Authentication.Application.DTOs
{
    public class UpdateUserRequest
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? UserName { get; set; }

        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public byte? Gender { get; set; }

        [Range(1, 5)]
        public int? RoleId { get; set; }

        public bool? IsAnonymous { get; set; }

      
        [MinLength(6)]
        public string? NewPassword { get; set; }
    }
}
