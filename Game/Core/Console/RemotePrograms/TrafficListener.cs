using Game.Core.Endpoints;
using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console.RemotePrograms
{
    class TrafficListener : Program
    {
        private Endpoint AttachedToo;
        private List<(string, string)> UsernamePasswordList = new();

        public TrafficListener() : base("TrafficListner", true)
        {

        }

        public override string RunProgram(Endpoint ranOn)
        {
            this.AttachedToo = ranOn;
            this.AttachedToo.OnLogin += Target_OnLogin;
            return "TrafficListner Initiated";
        }

        private void Target_OnLogin(object sender, EndpointLoginEventArgs e)
        {
            this.Running = true;
            this.UsernamePasswordList.Add((e.Username, e.Password));
            if (!Global.IRCWindow.ChannelExsits(this.Name))
            {
                Global.IRCWindow.AddChannelFromThread(this.Name);
            }
            Global.IRCWindow.AddMessageFromThread(this.Name, this.Name, "Traffic Detected:\n"
                + "FROM: IP: " + e.From.IPAddress + "\n"
                + "TOO: IP: " + this.AttachedToo.IPAddress + " HOST: " + this.AttachedToo.name + "\n"
                + "USER: " + e.Username + "\nPWRD: " + e.Password);
        }

        public override void StopProgram()
        {
            if (!this.Running)
            {
                return;
            }

            if (!Global.IRCWindow.ChannelExsits(this.Name))
            {
                Global.IRCWindow.AddChannelFromThread(this.Name);
            }
            Global.IRCWindow.AddMessageFromThread(this.Name, this.Name, "SHUTDOWN:\n"
                + "FROM: IP: " + this.AttachedToo.IPAddress);

            this.Running = false;

            if(AttachedToo != null)
            {
                AttachedToo.OnLogin -= Target_OnLogin;
                this.AttachedToo = null;
            }
        }
    }
}
