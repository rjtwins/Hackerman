using Game.Core.Endpoints;
using Game.Core.FileSystem;
using System;

namespace Game.Core.Console.RemotePrograms
{
    internal class TrafficListener : Program
    {
        public TrafficListener() : base("TrafficListner", true)
        {
            this.IsMalicious = true;
            this.Name += "v" + this.Version.ToString();
        }

        public override string RunProgram(Endpoint ranOn)
        {
            base.RunProgram(ranOn);
            this.RanOn.OnLogin += Target_OnLogin;
            Global.IRCWindow.AddMessageFromThread(this.Name, "TFL", "Listener Started:\n"
                + "ON: IP: " + ranOn.IPAddress + "\t HOST: " + ranOn.Name + "\n");
            return "TrafficListner Initiated";
        }

        private void Target_OnLogin(object sender, EndpointLoginEventArgs e)
        {
            if (!Global.IRCWindow.ChannelExsits(this.Name))
            {
                Global.IRCWindow.AddChannelFromThread(this.Name);
            }
            String fromIP = "127.0.0.1";
            if (e.From != null)
            {
                fromIP = e.From.IPAddress;
            }
            string result = string.Empty;
            string login = UTILS.GetHashString(e.Username + e.Password);
            result += "Traffic Detected:\n";
            result += "ON: IP: " + this.RanOn.IPAddress + " \tHOST: " + this.RanOn.Name + "\n";
            if (((int)e.EndpointHashing) > ((int)EndpointHashing.NONE))
            {
                result += "LOGIN HASH: " + login + "\n";
            }
            else
            {
                result += "USER: " + e.Username + "\nPWRD: " + e.Password;
            }
            if (this.Version > 1)
            {
                result += "FROM: IP: " + fromIP + "\n";
            }

            Global.LocalSystem.TrafficListnerAddEntry(e.From, this.RanOn, e.Username, e.Password, this.Version, e.EndpointHashing, login);
            Global.IRCWindow.AddMessageFromThread(this.Name, "TFL", result);
        }

        public override void StopProgram(bool ranOnRemote = false)
        {
            if (!this.Running)
            {
                return;
            }
            base.StopProgram();

            if (!Global.IRCWindow.ChannelExsits(this.Name))
            {
                Global.IRCWindow.AddChannelFromThread(this.Name);
            }
            Global.IRCWindow.AddMessageFromThread(this.Name, "TFL", "SHUTDOWN:\n"
                + "ON: IP: " + this.RanOn.IPAddress
                + "\tHOST: " + this.RanOn.Name);

            if (this.RanOn != null)
            {
                this.RanOn.OnLogin -= Target_OnLogin;
                this.RanOn = null;
            }
        }
    }
}