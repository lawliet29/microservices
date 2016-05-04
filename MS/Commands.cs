using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS
{
    public class SaveItem
    {
        private readonly IEntityAggregate _aggregate;
        public SaveItem(IEntityAggregate aggregate)
        {
            _aggregate = aggregate;
        }

        public void Execute(Entity entity)
        {
            _aggregate.ProcessCommand(new EntityCommand
            {
                Entity = entity,
                Type = EntityCommandType.Save
            });
        }
    }

    public class DeleteItem
    {
        private readonly IEntityAggregate _aggregate;

        public DeleteItem(IEntityAggregate aggregate)
        {
            _aggregate = aggregate;
        }

        public void Execute(Entity entity)
        {
            _aggregate.ProcessCommand(new EntityCommand
            {
                Entity = entity,
                Type = EntityCommandType.Delete
            });
        }
    }
}
