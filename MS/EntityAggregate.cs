using System.Net.Http;

namespace MS
{
    public enum EntityCommandType
    {
        Save,
        Delete
    }

    public class EntityCommand
    {
        public EntityCommandType Type { get; set; }
        public Entity Entity { get; set; }
    }

    public interface IEntityAggregate
    {
        void ProcessCommand(EntityCommand command);
    }

    public class RestApiEntityAggregateClient : IEntityAggregate
    {
        private const string Endpoint = "http://localhost:9000/aggregate";

        public void ProcessCommand(EntityCommand command)
        {
            using (var client = new HttpClient())
            {
                client.PostAsJsonAsync(Endpoint, command).Wait();
            }
        }
    }
}