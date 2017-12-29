using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sender.Fanout
{
    public class FanoutSender : IDisposable
    {
        IConnection connection;
        IModel channel;

        public FanoutSender()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "fanoutLog", type: "fanout");
        }


        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "fanoutLog",
                                 routingKey: "",
                                 basicProperties: properties,
                                 body: body);
            Console.WriteLine(" [x] Sent \" {0} \" to all.", message);
        }


        public void Dispose()
        {
            if (connection != null)
                connection.Close();
            if (channel != null)
                channel.Close();
        }
    }
}
