using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace AuthorizationServer.Services
{
    public class BrokerService:IBrokerService
    {
        private object _message = new object();
        private object _body = new object();
        
        public object GetMessage()
        {
            return _message;
        }

        public object SetMessage(object obj)
        {
            return _message = obj;
        }

        public byte[] GetBody()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(GetMessage()));
        }

        public object SetBody(object obj)
        {
            return _body = obj;
        }

        public void PublishMessageNewUserCreated()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            
                var body = GetBody();
    
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare("auth-user-created", durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicPublish("", "auth-user-created", null, body);
        }
    }
}