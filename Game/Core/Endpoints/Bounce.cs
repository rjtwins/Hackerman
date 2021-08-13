using System.Collections.Generic;

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
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Endpoint from, Endpoint too, bool succes</returns>
        internal (Endpoint, Endpoint, bool) MakeConnection()
        {
            Endpoint from = null;
            Endpoint too = null;
            Endpoint current = null;

            if (BounceList.Count <= 1)
            {
                return (null, null, false);
            }

            if (BounceList.Count == 2)
            {
                Global.EndPointMap.DisplayConnection();
                return (BounceList[0], BounceList[1], true);
            }

            for (int i = 1; i < BounceList.Count - 1; i++)
            {
                from = BounceList[i - 1];
                too = BounceList[i + 1];
                current = BounceList[i];
                current.BounceTo(from, too);
            }
            Global.EndPointMap.DisplayConnection();
            return (from, too, true);
        }

    }
}