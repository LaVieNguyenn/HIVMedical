using SharedKernel.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public byte Gender { get; set; }
        public string? Phone { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
