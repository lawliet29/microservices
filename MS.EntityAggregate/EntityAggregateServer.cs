using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Serialization;
using Owin;

namespace MS.EntityAggregate
{
    public class EntityAggregateServer
    {
        public IDisposable Connect() => WebApp.Start("http://localhost:9002", app =>
        {
            var configuration = new HttpConfiguration();
            configuration.MapHttpAttributeRoutes();
            configuration.Formatters.Remove(configuration.Formatters.XmlFormatter);
            configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            app.UseWebApi(configuration);
        });
    }
}
