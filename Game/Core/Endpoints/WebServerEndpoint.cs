using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    public class WebServerEndpoint : Endpoint
    {
        #region Backing fields
        [JsonProperty]
        private List<(string, Guid)> visitorLog = new();
        #endregion
        #region Properties
        public List<(string, Guid)> VisitorLog
        {
            get { return visitorLog; }
            set { visitorLog = value; }
        }
        #endregion

        [JsonConstructor]
        public WebServerEndpoint()
        {

        }

        public WebServerEndpoint(Person person, EndpointType endpointType) : base(person, endpointType)
        {
        }

        public void VisitedBy(Endpoint visitor)
        {
            this.VisitorLog.Add((Global.GameTime.ToString(), visitor.Id));
            if(OnVisit != null)
            {
                OnVisit(this, new WebEndpointVisitEventArgs(visitor));
            }
        }

        internal override void AdminSystemCheck()
        {
            base.AdminSystemCheck();
            //Trim Logs to 20 entries.
            while (this.VisitorLog.Count > 20)
            {
                this.VisitorLog.RemoveAt(this.VisitorLog.Count - 1);
            }
        }

        public delegate void WebEndpointVisitEventHandler(object sender, WebEndpointVisitEventArgs e);
        public event WebEndpointVisitEventHandler OnVisit;

    }
    public class WebEndpointVisitEventArgs : EventArgs
    {
        public Endpoint From;
        public WebEndpointVisitEventArgs(Endpoint from)
        {
            From = from;
        }
    }
}
