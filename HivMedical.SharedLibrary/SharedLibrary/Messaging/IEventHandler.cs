namespace SharedLibrary.Messaging
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task Handle(TEvent @event);
    }
} 