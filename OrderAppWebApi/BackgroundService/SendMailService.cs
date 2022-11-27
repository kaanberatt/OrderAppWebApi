using MySqlX.XDevAPI.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace OrderAppWebApi.BackgroundService
{
    public class SendMailService : IHostedService
    {
        private Timer timer;
        // Kuyruğu dinleyecek.
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(SendQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;            
        }
        public void SendQueue(object? state)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ddhnjhsy:t0MhrOPWugz9q8PJpjUtYRNSq-KDXF5z@rat.rmq2.cloudamqp.com/ddhnjhsy");
            using IConnection connection = factory.CreateConnection();

            using (IModel channel = connection.CreateModel())
            {
                #region Queue Declaration
                string queueName = "SendTask";
                channel.QueueDeclare(queueName, true, false, false);
                #endregion

                var consumer = new EventingBasicConsumer(channel);
                SendMail("kaanberattokat@gmail.com");

                consumer.Received += (obj, e) =>
                {
                    var result = Encoding.UTF8.GetString(e.Body.ToArray());
                    Console.WriteLine("Kuyruktaki işlemi tamamladım!");
                };
                channel.BasicConsume(queueName, true, consumer);
               
            }
        }



        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }
        public void SendMail(string toEmail)
        {
            var from = "kaanberattokat@gmail.com";
            var to = toEmail;
            var subject = "Test mail";
            var body = "Test body";

            var username = "kaanberattokat@gmail.com "; // get from Mailtrap

            var password = "";
            var host = "";
            var port = 0;

            var client = new SmtpClient();
            try
            {
                client.Host = host;
                client.Port = port;
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Send(from, to, subject, body) ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }
    }
}
