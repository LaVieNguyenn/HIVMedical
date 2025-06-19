using System.Text.Json.Serialization;

namespace SharedLibrary.Messaging
{
    public abstract class IntegrationEvent
    {
        [JsonInclude]
        public Guid Id { get; private set; }

        [JsonInclude]
        public DateTime CreationDate { get; private set; }

        public IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        [JsonConstructor]
        public IntegrationEvent(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }
    }
} 