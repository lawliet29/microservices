using System;
using System.Net;
using System.Net.Http;
using System.Text;
using EasyNetQ;
using EventStore.ClientAPI;
using MS.EntityAggregate.Dtos;
using Newtonsoft.Json;

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
	
		private readonly Lazy<IEventStoreConnection> _connection = new Lazy<IEventStoreConnection>(
			() =>
			{
				var connection = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
				connection.ConnectAsync().Wait();
				return connection;
			}); 

        public EntityViewRabbitClient(string queueName = "viewEvent")
        {

            _queueName = queueName;
        }


        public static IBus Bus { get; } =
            RabbitHutch.CreateBus("amqp://pcyuoarf:mm_DAba1hDupi1KnsR5l9kVsTupsBo3V@chicken.rmq.cloudamqp.com/pcyuoarf");

	    public IEventStoreConnection Connection => _connection.Value;

		public void HandleEvent(ViewEvent e)
        {
            Bus.Send(_queueName, e);
			
			var eventData = new EventData(Guid.NewGuid(), e.GetType().ToString(), true, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)), null);
			Connection.AppendToStreamAsync("viewEvent", ExpectedVersion.Any, eventData).Wait();
        }
    }
}
