using Game.Core.Console.LocalPrograms;
using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using static Game.Core.Endpoints.Endpoint;

namespace Game.Core.Console
{
    public class LocalSystem
    {
        //TODO make components to fix placeholders for speed exct

        public Bouncer Bouncer = new Bouncer();
        private Dictionary<string, Endpoint[]> SavedBounceLists = new Dictionary<string, Endpoint[]>();

        public EndpointFirewall FirewallBypass = EndpointFirewall.NONE;
        public EndpointMonitor MonitorBypass = EndpointMonitor.NONE;
        public bool FirewallBypassActive { get; private set; }
        public bool MonitorBypassActive { get; private set; }

        //private Dictionary<string, Program> SavedFiles = new Dictionary<string, Program>();

        internal void SaveCurrentBounceListsAs(string commandBody)
        {
            this.SavedBounceLists[commandBody] = Global.Bounce.BounceList.ToArray();
        }

        internal Endpoint[] LoadBounceList(string commandBody)
        {
            if (SavedBounceLists.TryGetValue(commandBody, out Endpoint[] bounceList))
            {
                return bounceList;
            }
            throw new Exception(commandBody + " was not found.");
        }

        internal Dictionary<string, Endpoint[]> GetSavedBouncelists()
        {
            return this.SavedBounceLists;
        }

        internal bool RemoveBounceList(string commandBody)
        {
            if (!this.SavedBounceLists.ContainsKey(commandBody))
            {
                return false;
            }
            SavedBounceLists.Remove(commandBody);
            return true;
        }

        public int GetModumSpeed()
        {
            return 99;
        }

        public int GetProcessorSpeed()
        {
            return 99;
        }

        public int GetDiskMemory()
        {
            return 999999;
        }

        public int GetDiskFreeMemory()
        {
            return 99999;
        }

        public int GetDiskUsedMemory()
        {
            return 0;
        }
    }
}