using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public static IEntityView Instance { get; set; } = new EntityViewEventStoreClient();
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

	public class HttpApiEntityViewEventStoreClient : IEntityView
	{
		private readonly string _host;
		private readonly string _queueName;
		private readonly HttpClient _client = new HttpClient();

		public HttpApiEntityViewEventStoreClient(string host = "http://stasshiray.com:21113", string queueName = "viewEvent")
		{
			_host = host;
			_queueName = queueName;
		}

		public void HandleEvent(ViewEvent e)
		{
			
			var content = new StringContent(JsonConvert.SerializeObject(e), Encoding.UTF8, "application/json");
			content.Headers.Add("ES-EventId", Guid.NewGuid().ToString());
			content.Headers.Add("ES-EventType", e.GetType().ToString());

			_client.PostAsync($"{_host}/streams/{_queueName}", content).Wait();
		}
	}


	public class EntityViewEventStoreClient : IEntityView
    {
        private readonly string _queueName;
	    private readonly IEventStoreConnection _eventStoreConnection; 

		private IEventStoreConnection Connection
		{
			get
			{
				_eventStoreConnection.ConnectAsync().Wait();
				return _eventStoreConnection;
			}
		}

	    internal EntityViewEventStoreClient(string queueName, IEventStoreConnection connection)
	    {
		    _queueName = queueName;
		    _eventStoreConnection = connection;
	    }

	    public EntityViewEventStoreClient(string queueName, string uri)
		    : this(queueName, EventStoreConnection.Create(new Uri(uri)))
	    {
	    }

        public EntityViewEventStoreClient(string queueName = "viewEvent") : 
			this(queueName, EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113)))
        {
        }

	    public void HandleEvent(ViewEvent e)
	    {
		    var eventData = new EventData(Guid.NewGuid(), e.GetType().ToString(), true,
			    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)), null);
		    Connection.AppendToStreamAsync(_queueName, ExpectedVersion.Any, eventData).Wait();
	    }
    }
}
