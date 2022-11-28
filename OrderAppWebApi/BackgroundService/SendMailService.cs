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
            // Her 60 saniyede 1 SendQueue methodu tetiklenecek.
            timer = new Timer(SendQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
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

                consumer.Received += (obj, e) =>
                {
                    var result = Encoding.UTF8.GetString(e.Body.ToArray());
                    SendMail(result);
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
            #region Mail Content Info
            var from = "kaan.softwareengineer@gmail.com";
            var to = toEmail;
            var subject = "Order App Test Mail";
            var body = "Bu mail Order App uygulamasını test etmek amacıyla gönderilmiştir.";
            #endregion

            #region Password
            var username = "kaan.softwareengineer@gmail.com"; // get from Mailtrap
            var password = "21085454Kt";
            var host = "smtp.office365.com";
            var port = 587;
            #endregion

            var client = new SmtpClient();
            try
            {
                #region Mail Configuration
                client.Host = host;
                client.Port = port;
                client.Credentials = new NetworkCredential(username, password);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;

                #endregion

                client.Send(from, to, subject, body); // send mail
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }
    }
}
