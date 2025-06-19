namespace SharedLibrary.Messaging
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : class;
        void Subscribe<T, TH>() where T : class where TH : IEventHandler<T>;
    }
} 