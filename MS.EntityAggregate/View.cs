using System.Net.Http;
using EasyNetQ;
using MS.EntityAggregate.Dtos;

namespace MS.EntityAggregate
{
    public interface IEntityView
    {
        void HandleEvent(ViewEvent e);
    }

    public static class EntityView
    {
        public static IEntityView Instance { get; set; } = new EntityViewRabbitClient();
    }

    public class EntityViewRestClient : IEntityView
    {
        public EntityViewRestClient()
        {
            var a = 5;
        }

        public const string Url = "http://localhost:9001/view";

        public void HandleEvent(ViewEvent e)
        {
            using (var client = new HttpClient())
            {
                client.PostAsJsonAsync(Url, e).Wait();
            }
        }
    }

    public class EntityViewRabbitClient : IEntityView
    {
        private readonly string _queueName;

        public EntityViewRabbitClient(string queueName = "viewEvent")
        {

            _queueName = queueName;
        }

        public static IBus Bus { get; } =
            RabbitHutch.CreateBus("amqp://dmowhoix:n95a17-CgycVl9cHsxXOCbVhX-R5iEDP@hare.rmq.cloudamqp.com/dmowhoix");
        

        public void HandleEvent(ViewEvent e)
        {
            Bus.Send(_queueName, e);
        }
    }
}
