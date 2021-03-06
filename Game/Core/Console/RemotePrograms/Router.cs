using Game.Core.Endpoints;
using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console.RemotePrograms
{
    public class Router : Program
    {
        public Router(SoftwareLevel softwareLevel = SoftwareLevel.LVL0) : base("BOUNCE_RPI.exe", true)
        {
            this.IsMalicious = true;
            this.SoftwareLevel = softwareLevel;
        }

        public override string RunProgram(Endpoint ranOn)
        {
            base.RunProgram(ranOn);

            if (!ranOn.CanBreachFirewall())
            {
                Debug.WriteLine("BOUNCE COULD NOT BREACH FIREWALL");
                return "BOUNCE startup error, blocked by firewall.";
            }
            
            Global.BounceNetwork.AddAndNotify(ranOn);
            return "BOUNCE routing point setup completed.";
        }

        public override void StopProgram(bool ranOnRemote = false)
        {
            base.StopProgram();
            Global.BounceNetwork.RemoveAndNotify(RanOn);
        }
    }
}
