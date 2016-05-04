using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace MS.EntityAggregate.Dtos
{
    public enum ViewEventType { EntityCreated, EntityDeleted }

    [Queue("ViewEvent")]
    public class ViewEvent
    {
        public ViewEventType EventType { get; set; }
        public int EntityId { get; set; }
    }
}
