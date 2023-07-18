using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RabbitMQPublisher
{
    public enum LogNames
    {
        Critical=1,
        Error=2,
        Warning=3,
        Info=4
    }


    public class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://localhost:5672");

            using (var connection = factory.CreateConnection())
            {
                var channel = connection.CreateModel();

                channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

                Dictionary<string,object> headers = new Dictionary<string, object>();

                headers.Add("format","pdf");
                headers.Add("shape2","a4");

                var properties = channel.CreateBasicProperties();
                properties.Headers= headers;
                properties.Persistent = true; //Messages are will be persistent now.

                var product = new Product { Id=1,Name="Kalem",Price=100,Stock=10};

                var productJsonString = JsonSerializer.Serialize(product);

                channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

                Console.WriteLine("Mesaj Gönderilmiştir.");

                //Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
                //{
                //   // var queqeName = $"direct-queqe-{x}";
                //    //channel.QueueDeclare(queqeName, true, false, false);

                //    //channel.QueueBind(queqeName, "logs-direct", routeKey,null);
                //});

                //Random rnd = new Random();
                //Enumerable.Range(1, 50).ToList().ForEach(x =>
                //{
                //    LogNames log1 = (LogNames)rnd.Next(1, 5);
                //    LogNames log2 = (LogNames)rnd.Next(1, 5);
                //    LogNames log3 = (LogNames)rnd.Next(1, 5);

                //    var routeKey = $"{log1}.{log2}.{log3}";
                //    //var routeKey = $"route-{log}";
                //    string message = $"log-type {log1}-{log2}-{log3}";
                //    var messageBody = Encoding.UTF8.GetBytes(message);

                //    channel.BasicPublish("logs-topic",routeKey, null, messageBody);

                //    Console.WriteLine($"Log Gönderilmiştir : {message}");
                //});

                Console.ReadLine();
            }
        }
    }
}
