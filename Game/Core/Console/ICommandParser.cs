using Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console
{
    public interface ICommandParser
    {
        void AttachConsole(ConsoleContent consoleContent);
        void ParseCommand(string consoleInput, string prefix);
    }
}
