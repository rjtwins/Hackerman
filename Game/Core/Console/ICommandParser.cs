using Game.UI;

namespace Game.Core.Console
{
    public interface ICommandParser
    {
        void AttachConsole(ConsoleContent consoleContent);

        void ParseCommand(string consoleInput, string prefix);
    }
}