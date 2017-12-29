﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receiver.Fanout
{
    public class FanoutReceiver1 : IDisposable
    {
        public IConnection connection;
        public IModel channel;

        public FanoutReceiver1()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();


            channel.ExchangeDeclare(exchange: "fanoutLog", type: "fanout");

            // Get a server-assigned Queue becasue we only want 
            // upcoming messages, not existing.
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                              exchange: "fanoutLog",
                              routingKey: "");


            Console.WriteLine(" [Listening for Logs]");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, e) =>
            {
                var body = e.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Log Received {0}", message);


                // Manual ACK
                //channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
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
