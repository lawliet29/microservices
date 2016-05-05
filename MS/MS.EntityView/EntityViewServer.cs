using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using EasyNetQ;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Serialization;
using Owin;

namespace MS.EntityView
{
    public class EntityViewController : ApiController
    {
        private readonly EntityView _view;

        public EntityViewController()
        {
            _view = EntityView.Instance;
        }

        [Route("view")]
        public IHttpActionResult HandleEvent(ViewEvent e)
        {
            _view.HandleEvent(e);
            return Ok();
        }
    }

    public class EntityViewQueueListener
    {
        private readonly EntityView _view;

        public static IBus Bus { get; } =
            RabbitHutch.CreateBus("amqp://pcyuoarf:mm_DAba1hDupi1KnsR5l9kVsTupsBo3V@chicken.rmq.cloudamqp.com/pcyuoarf");

        public EntityViewQueueListener()
        {
            _view = EntityView.Instance;
        }

        public IDisposable Subscribe()
        {
            return Bus.Consume<ViewEvent>("viewEvent", e =>
            {
                _view.HandleEvent(e);
            });
        }
    }

    public class EntityViewServer
    {
        public IDisposable Connect() => WebApp.Start("http://localhost:9001", app =>
        {
            var configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);
            configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            app.UseWebApi(configuration);
        });
    }
}
