using System.Collections.Generic;

namespace MS
{
    public interface ISplitItem
    {
        void Split(IEnumerable<Command> commands);
    }

    public class SplitItem : ISplitItem
    {
        private readonly IExecuteCommand _executeCommand;

        public SplitItem(IExecuteCommand executeCommand)
        {
            _executeCommand = executeCommand;
        }

        public void Split(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                _executeCommand.Execute(command);
            }
        }
    }

}