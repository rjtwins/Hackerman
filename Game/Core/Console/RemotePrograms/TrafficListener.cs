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
            this.IsMalicious = true;
        }

        public override string RunProgram(Endpoint ranOn)
        {
            this.Running = true;
            this.AttachedToo = ranOn;
            this.AttachedToo.OnLogin += Target_OnLogin;
            Global.IRCWindow.AddMessageFromThread(this.Name, this.Name, "Listener Started:\n"
                + "ON: IP: " + ranOn.IPAddress + "\tHOST: " + ranOn.name + "\n");
            return "TrafficListner Initiated";
        }

        private void Target_OnLogin(object sender, EndpointLoginEventArgs e)
        {
            this.UsernamePasswordList.Add((e.Username, e.Password));
            if (!Global.IRCWindow.ChannelExsits(this.Name))
            {
                Global.IRCWindow.AddChannelFromThread(this.Name);
            }
            String fromIP = "127.0.0.1";
            if(e.From != null)
            {
                fromIP = e.From.IPAddress;
            }
            if(((int)e.EndpointHashing) > ((int)Endpoint.EndpointHashing.NONE))
            {
                string login = e.Username + e.Password;
                login = UTILS.GetHashString(login);

                Global.IRCWindow.AddMessageFromThread(this.Name, this.Name, "Traffic Detected:\n"
                    + "FROM: IP: " + fromIP + "\n"
                    + "TOO: IP: " + this.AttachedToo.IPAddress + " \tHOST: " + this.AttachedToo.name + "\n"
                    + "LOGIN HASH: " + login);

                Global.LocalSystem.TrafficListnerAddEntry(e.From, this.AttachedToo, e.Username, e.Password, e.EndpointHashing, login);
                return;
            }

            Global.LocalSystem.TrafficListnerAddEntry(e.From, this.AttachedToo, e.Username, e.Password);
            Global.IRCWindow.AddMessageFromThread(this.Name, this.Name, "Traffic Detected:\n"
                + "FROM: IP: " + fromIP + "\n"
                + "TOO: IP: " + this.AttachedToo.IPAddress + " \tHOST: " + this.AttachedToo.name + "\n"
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
                + "FROM: IP: " + this.AttachedToo.IPAddress
                + "\tHOST: " + this.AttachedToo.name);

            this.Running = false;

            if(AttachedToo != null)
            {
                AttachedToo.OnLogin -= Target_OnLogin;
                this.AttachedToo = null;
            }
        }



    }
}
