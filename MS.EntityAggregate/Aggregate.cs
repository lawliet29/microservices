using System;
using MS.EntityAggregate.Dtos;

namespace MS.EntityAggregate
{
    public interface IEntityAggregate
    {
        void ProcessCommand(EntityCommand command);
    }

    public class EntityAggregate : IEntityAggregate
    {
        private readonly IEntityView _view;

        public EntityAggregate(IEntityView view)
        {
            _view = view;
        }

        public void ProcessCommand(EntityCommand command)
        {
            switch (command.Type)
            {
                case EntityCommandType.Delete:
                    _view.HandleEvent(new ViewEvent { EntityId = command.Entity.EntityId, EventType = ViewEventType.EntityDeleted });
                    return;
                case EntityCommandType.Save:
                    _view.HandleEvent(new ViewEvent { EntityId = command.Entity.EntityId, EventType = ViewEventType.EntityCreated });
                    return;
                default:
                    throw new InvalidOperationException("Unknown command type");
            }
        }
    }
}
