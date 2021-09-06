using System;
using System.Collections.Generic;

namespace Game.Core.Endpoints
{
    public class BouncePathManager
    {
        public List<Endpoint> BounceList { private set; get; } = new List<Endpoint>();

        public bool AddBounce(string endpointIP)
        {
            foreach (var e in Global.AllEndpoints)
            {
                if (endpointIP != e.IPAddress)
                {
                    continue;
                }
                if (!UTILS.CanAddBounce(e))
                {
                    return false;
                }
                AddBounce(e);
                return true;
            }
            return false;
        }

        public void AddBounce(Endpoint endpoint)
        {
            if (BounceList.Contains(endpoint))
            {
                return;
            }
            this.BounceList.Add(endpoint);
        }

        public void RemoveBounce(Endpoint endpoint)
        {
            if (!this.BounceList.Contains(endpoint))
            {
                return;
            }
            this.BounceList.Remove(endpoint);
        }

        internal void ToggleBounce(Endpoint endpoint)
        {
            if (BounceList.Contains(endpoint))
            {
                BounceList.Remove(endpoint);
                return;
            }
            BounceList.Add(endpoint);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>Endpoint from, Endpoint too, bool succes</returns>
        internal (Endpoint, Endpoint, bool) SetupDoBouncePath()
        {
            Endpoint from = null;
            Endpoint too = null;
            Endpoint current = null;

            if (BounceList.Count < 1)
            {
                return (null, null, false);
            }

            if (BounceList.Count == 1)
            {
                return (Global.LocalEndpoint, BounceList[0], true);
            }
            BounceList[0].BounceTo(Global.LocalEndpoint, BounceList[1]);

            if (BounceList.Count == 2)
            {
                from = BounceList[0];
                too = BounceList[1];
                return (from, too, true);
            }

            BounceList[0].BounceTo(Global.LocalEndpoint, BounceList[1]);
            for (int i = 1; i < BounceList.Count - 1; i++)
            {
                from = BounceList[i - 1];
                current = BounceList[i];
                too = BounceList[i + 1];
                current.BounceTo(from, too);
            }
            return (current, too, true);
        }
    }
}