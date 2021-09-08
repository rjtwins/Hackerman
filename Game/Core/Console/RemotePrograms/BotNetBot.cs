using Game.Core.Endpoints;
using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console.RemotePrograms
{
    class BotNetBot : Program
    {
        public BotNetBot(string name) : base(name, true, false)
        {

        }

        public override string RunProgram(Endpoint ranOn)
        {
            base.RunProgram(ranOn);

            return string.Empty;
        }

        public override void StopProgram(bool remoteLost = false)
        {
            base.StopProgram(remoteLost);
        }
    }
}
