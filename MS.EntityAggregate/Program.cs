using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json.Serialization;
using Owin;

namespace MS.EntityAggregate
{
    class Program
    {
        public static void Main()
        {
            using (var server = new EntityAggregateServer().Connect())
            {
                Console.ReadKey();
            }
        }
    }
}
