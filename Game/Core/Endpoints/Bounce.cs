using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    public class Bounce
    {
        public List<Endpoint> BounceList { private set; get; } = new List<Endpoint>();
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

        internal void ToggleBounde(Endpoint endpoint)
        {
            if (BounceList.Contains(endpoint))
            {
                BounceList.Remove(endpoint);
                return;
            }
            BounceList.Add(endpoint);
        }

        internal bool MakeConnection()
        {
            Endpoint from;
            Endpoint too;
            Endpoint current = null;

            if(BounceList.Count <= 1)
            {
                return false;
            }

            if(BounceList.Count == 2)
            {
                Global.Console.ConnectToFrom(BounceList[0], BounceList[1]);
                Global.EndPointMap.DisplayConnection();
                return true;
            }

            for (int i = 1; i < BounceList.Count-1; i++)
            {
                from = BounceList[i - 1];
                too = BounceList[i + 1];
                current = BounceList[i];
                current.BounceTo(from, too);
            }
            Global.Console.ConnectToFrom(BounceList[BounceList.Count - 1], current);
            Global.EndPointMap.DisplayConnection();
            return true;
        }
    }
}
