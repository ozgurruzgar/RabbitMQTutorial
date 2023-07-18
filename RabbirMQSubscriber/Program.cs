using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace RabbirMQSubscriber
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://localhost:5672");

            using var connection = factory.CreateConnection();
            
                var channel = connection.CreateModel();
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
            //channel.QueueDeclare("hello-queqe", true, false, false);

            //channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout); We can use this code in here.

            //var randomQueqeName =channel.QueueDeclare().QueueName; //"log-database-save";

            //channel.QueueDeclare(randomQueqeName, durable: true, false, false);

            //channel.QueueBind(randomQueqeName, "logs-fanout", "", null); //When app down,about queqe will be remove. 

            channel.BasicQos(0, 1, false); 

                var consumer = new EventingBasicConsumer(channel);

                var queqeName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            headers.Add("x-match","any");


            channel.QueueBind(queqeName,"header-exchange",string.Empty,headers);

            Console.WriteLine("Loglar dinleniyor...");

                consumer.Received += (object sender, BasicDeliverEventArgs arg) =>
                {
                    var message = Encoding.UTF8.GetString(arg.Body.ToArray());

                    Product product = JsonSerializer.Deserialize<Product>(message);

                    Thread.Sleep(1500);

                    Console.WriteLine($"Gelen Mesaj: { product.Id} - {product.Name} - {product.Price} - {product.Stock}");

                    //File.AppendAllText("log-critical.txt", message + "\n"); // \n escape character semantically new line.

                    channel.BasicAck(arg.DeliveryTag, false);
                };
            channel.BasicConsume(queqeName, false, consumer);

            Console.ReadLine();
            
        }
    }
}
