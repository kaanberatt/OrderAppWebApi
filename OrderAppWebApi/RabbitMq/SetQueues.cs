using RabbitMQ.Client;

namespace OrderAppWebApi.RabbitMq
{
    public class SetQueues
    {
        // Kuyruk oluşturulur ve kuyruğa ekleme yapılır.
        public static void SetQueue(Byte[] datas)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ddhnjhsy:t0MhrOPWugz9q8PJpjUtYRNSq-KDXF5z@rat.rmq2.cloudamqp.com/ddhnjhsy");
            //RabbitMQ ile Connection Açılır.
            using IConnection connection = factory.CreateConnection();

            using (IModel channel = connection.CreateModel())
            {
                #region Queue Declaration
                string queueName = "SendTask";
                channel.QueueDeclare(queueName, true, false, false);
                // Kuyruk yapısı oluşturuluyor.
                #endregion

                // Parametre olarak gelen değer kuyruğa gönderiliyor. Queue’ye gönderilecek mesaj byte[] tipinde gönderilir.
                channel.BasicPublish(String.Empty, queueName, null, datas);
            }


        }
    }
}
