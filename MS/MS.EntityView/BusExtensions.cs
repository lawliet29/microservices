using System;
using System.Text;
using EasyNetQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MS.EntityView
{
    public static class BusExtensions
    {
        public static IDisposable Consume<TMessage>(this IBus bus, string queueName, Action<TMessage> handler)
        {
            var queue = bus.Advanced.QueueDeclare(queueName);
            return bus.Advanced.Consume(queue, (body, properties, info) =>
            {
                var message = Encoding.UTF8.GetString(body);
                handler.Invoke(JsonConvert.DeserializeObject<TMessage>(message));
            });
        }

        public static IDisposable ConsumeJson(this IBus bus, string queueName, Action<JObject> handler)
        {
            var queue = bus.Advanced.QueueDeclare(queueName);
            return bus.Advanced.Consume(queue, (body, properties, info) =>
            {
                var message = Encoding.UTF8.GetString(body);
                handler.Invoke(JObject.Parse(message));
            });
        }
    }
}
