using System.Net.Http;
using MS.EntityAggregate.Dtos;

namespace MS.EntityAggregate
{
    public interface IEntityView
    {
        void HandleEvent(ViewEvent e);
    }

    public static class EntityView
    {
        public static IEntityView Instance { get; set; } = new EntityViewRestClient();
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
}
