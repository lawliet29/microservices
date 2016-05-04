using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace MS.EntityView
{
    public enum ViewEventType { EntityCreated, EntityDeleted }

    [Queue("ViewEvent")]
    public class ViewEvent
    {
        public ViewEventType EventType { get; set; }
        public int EntityId { get; set; }
    }

    public class EntityView
    {
        public ISet<int> State { get; } = new HashSet<int>();

        public void HandleEvent(ViewEvent e)
        {
            switch (e.EventType)
            {
                case ViewEventType.EntityCreated:
                    State.Add(e.EntityId);
                    return;
                case ViewEventType.EntityDeleted:
                    State.Remove(e.EntityId);
                    return;
                default:
                    throw new InvalidOperationException("Unknown event type");
            }
        }

        public static EntityView Instance { get; set; } = new EntityView();
    }
}
