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

        public TrafficListener() : base("TrafficListner", true)
        {
            this.IsMalicious = true;
            this.Name += "v" + this.Version.ToString();
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
            if (!Global.IRCWindow.ChannelExsits(this.Name))
            {
                Global.IRCWindow.AddChannelFromThread(this.Name);
            }
            String fromIP = "127.0.0.1";
            if(e.From != null)
            {
                fromIP = e.From.IPAddress;
            }
            string result = string.Empty;
            string login = UTILS.GetHashString(e.Username + e.Password);
            result += "Traffic Detected:\n";
            result += "ON: IP: " + this.AttachedToo.IPAddress + " \tHOST: " + this.AttachedToo.name + "\n";
            if (((int)e.EndpointHashing) > ((int)Endpoint.EndpointHashing.NONE))
            {
                result += "LOGIN HASH: " + login + "\n";
            }
            else
            {
                result += "USER: " + e.Username + "\nPWRD: " + e.Password;
            }
            if(this.Version > 1)
            {
                result += "FROM: IP: " + fromIP + "\n";
            }

            Global.LocalSystem.TrafficListnerAddEntry(e.From, this.AttachedToo, e.Username, e.Password, this.Version, e.EndpointHashing, login);
            Global.IRCWindow.AddMessageFromThread(this.Name, this.Name, result);
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
                + "ON: IP: " + this.AttachedToo.IPAddress
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
