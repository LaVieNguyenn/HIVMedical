using System.Text.Json.Serialization;

namespace SharedLibrary.Messaging.Events
{
    public class UserRegisteredEvent : IntegrationEvent
    {
        public int UserId { get; }
        public string Email { get; }
        public string FullName { get; }
        public string Role { get; }

        [JsonConstructor]
        public UserRegisteredEvent(int userId, string email, string fullName, string role)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            Role = role;
        }
    }
} 