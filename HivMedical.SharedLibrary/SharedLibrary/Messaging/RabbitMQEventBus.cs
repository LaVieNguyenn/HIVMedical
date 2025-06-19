using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SharedLibrary.Messaging
{
    public class RabbitMQEventBus : IEventBus, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMQEventBus> _logger;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public RabbitMQEventBus(
            IConnectionFactory connectionFactory, 
            IServiceScopeFactory scopeFactory,
            ILogger<RabbitMQEventBus> logger)
        {
            _connectionFactory = connectionFactory;
            _scopeFactory = scopeFactory;
            _logger = logger;
            _handlers = new Dictionary<string, List<Type>>();
            
            try
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                
                _logger.LogInformation("RabbitMQ connection established");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not connect to RabbitMQ");
                throw;
            }
        }

        public void Publish<T>(T @event) where T : class
        {
            var eventName = @event.GetType().Name;
            _logger.LogInformation("Publishing event {EventName}", eventName);

            try
            {
                _channel.QueueDeclare(queue: eventName, 
                                     durable: true, 
                                     exclusive: false, 
                                     autoDelete: false);

                var message = JsonSerializer.Serialize(@event);
                var body = Encoding.UTF8.GetBytes(message);

                _channel.BasicPublish(exchange: "",
                                     routingKey: eventName,
                                     basicProperties: null,
                                     body: body);
                
                _logger.LogInformation("Event {EventName} published successfully", eventName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing event {EventName}", eventName);
                throw;
            }
        }

        public void Subscribe<T, TH>() where T : class where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
                
                _channel.QueueDeclare(queue: eventName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false);
                
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    await ProcessEvent(eventName, message);
                };
                
                _channel.BasicConsume(queue: eventName,
                                     autoAck: true,
                                     consumer: consumer);
            }

            if (_handlers[eventName].Any(h => h == handlerType))
            {
                _logger.LogWarning("Handler Type {HandlerType} already registered for {EventName}", handlerType.Name, eventName);
                return;
            }

            _handlers[eventName].Add(handlerType);
            _logger.LogInformation("Subscribed to event {EventName} with {HandlerType}", eventName, handlerType.Name);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (!_handlers.ContainsKey(eventName))
            {
                _logger.LogWarning("No handler registered for {EventName}", eventName);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var handlers = _handlers[eventName];

            foreach (var handlerType in handlers)
            {
                try
                {
                    var handler = scope.ServiceProvider.GetService(handlerType);
                    if (handler == null)
                    {
                        _logger.LogWarning("Handler {HandlerType} not registered in DI", handlerType.Name);
                        continue;
                    }

                    var eventType = _handlers.FirstOrDefault(x => x.Key == eventName).Value
                                            .FirstOrDefault(h => h == handlerType);

                    var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    var eventData = JsonSerializer.Deserialize(message, eventType);

                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { eventData });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event {EventName}", eventName);
                }
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
} 