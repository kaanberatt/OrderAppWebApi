using RabbitMQ.Client;

namespace OrderAppWebApi.RabbitMq
{
    public class SetQueues
    {
        // Kuyruk oluşturulur ve kuyruğa ekleme yapılır.
        public static void SendQueue(Byte[] datas)
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
                #endregion

                channel.BasicPublish(String.Empty, queueName, null, datas);
            }


        }
    }
}
