using System;

namespace MS
{
    public abstract class Command
    {
        public int EntityId { get; set; }
    }

    public class CreateCommand : Command { }

    public class DeleteCommand : Command { }


    public interface IExecuteCommand
    {
        void Execute(Command command);
    }

    public class ExecuteCommand : IExecuteCommand
    {
        private readonly SaveItem _saveItem;
        private readonly DeleteItem _deleteItem;

        public ExecuteCommand(SaveItem saveItem, DeleteItem deleteItem)
        {
            _saveItem = saveItem;
            _deleteItem = deleteItem;
        }

        public void Execute(Command command)
        {
            var entity = new Entity { EntityId = command.EntityId };

            var saveCommand = command as CreateCommand;
            if (saveCommand != null)
            {
                _saveItem.Execute(entity);
                return;
            }

            var deleteCommand = command as DeleteCommand;
            if (deleteCommand != null)
            {
                _deleteItem.Execute(entity);
                return;
            }

            throw new InvalidOperationException("Unknown command");
        }
    }
}
