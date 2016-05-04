using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent e);
        void Subscribe<TEvent>(Action<TEvent> handler);
    }
}
