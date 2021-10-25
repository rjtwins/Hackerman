using Game.Core.World;
using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EndpointMonitor
    {
        #region backing fields
        [JsonProperty]
        private Guid parentEndpoint;
        [JsonProperty]
        private ReferenceList<Endpoint> suspiciousEndpoints = new(Global.AllEndpointsDict, "AllEndpointsDict");
        [JsonProperty]
        private SoftwareLevel level = SoftwareLevel.LVL0;
        [JsonProperty]
        private bool monitorActive = false;
        [JsonProperty]
        private int FailedLogins = 0;
        [JsonProperty]
        private bool StartPassiveTraceOnDisconnect = false;
        #endregion

        #region properties
        public Endpoint ParentEndpoint
        {
            get
            {
                return Global.AllEndpointsDict[parentEndpoint];
            }
            set
            {
                this.parentEndpoint = value.Id;
            }
        }
        public ReferenceList<Endpoint> SuspiciousEndpoints { private get => suspiciousEndpoints; set => suspiciousEndpoints = value; }
        public bool MonitorActive
        {
            get { return monitorActive; }
            set { monitorActive = value; }
        }
        public SoftwareLevel Level { get => level; set => level = value; }
        #endregion

        public EndpointMonitor(Endpoint parentEndpoint)
        {
            this.ParentEndpoint = parentEndpoint;
            this.ParentEndpoint.OnFailedLogin += ParentEndpoint_OnFailedLogin;
            this.ParentEndpoint.OnLogin += ParentEndpoint_OnLogin; ;
            this.ParentEndpoint.OnDisconnected += ParentEndpoint_OnDisconnected;
            this.ParentEndpoint.OnConnected += ParentEndpoint_OnConnected;
            this.ParentEndpoint.OnShutdown += ParentEndpoint_OnShutdown;
            this.ParentEndpoint.OnFileGet += ParentEndpoint_OnFileGet;
            this.ParentEndpoint.OnFileAdd += ParentEndpoint_OnFileAdd;
            this.ParentEndpoint.OnFileRemove += ParentEndpoint_OnFileRemove;
            this.ParentEndpoint.OnFileRun += ParentEndpoint_OnFileRun;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            this.ParentEndpoint.OnFailedLogin += ParentEndpoint_OnFailedLogin;
            this.ParentEndpoint.OnLogin += ParentEndpoint_OnLogin; ;
            this.ParentEndpoint.OnDisconnected += ParentEndpoint_OnDisconnected;
            this.ParentEndpoint.OnConnected += ParentEndpoint_OnConnected;
            this.ParentEndpoint.OnShutdown += ParentEndpoint_OnShutdown;
            this.ParentEndpoint.OnFileGet += ParentEndpoint_OnFileGet;
            this.ParentEndpoint.OnFileAdd += ParentEndpoint_OnFileAdd;
            this.ParentEndpoint.OnFileRemove += ParentEndpoint_OnFileRemove;
            this.ParentEndpoint.OnFileRun += ParentEndpoint_OnFileRun;
        }

        private void ParentEndpoint_OnFileRun(object sender, EndpointFileOperationEventArgs e)
        {
            if (!this.MonitorActive)
            {
                return;
            }
            if (IsUserConnectingFromKnownEndpoint(this.ParentEndpoint.CurrentUsername, this.ParentEndpoint.ConnectedFrom))
            {
                return;
            }
            if (e.File.IsWatched)
            {
                StartActiveTrace();
                return;
            }
            if (e.File.IsMalicious)
            {
                StartActiveTrace();
            }
        }

        private void ParentEndpoint_OnFileRemove(object sender, EndpointFileOperationEventArgs e)
        {
            if (!this.MonitorActive)
            {
                return;
            }
            if (IsUserConnectingFromKnownEndpoint(this.ParentEndpoint.CurrentUsername, this.ParentEndpoint.ConnectedFrom))
            {
                return;
            }
            if (e.File.IsWatched)
            {
                StartActiveTrace();
                return;
            }
            if (e.File.IsMalicious)
            {
                StartActiveTrace();
            }
        }

        private void ParentEndpoint_OnFileAdd(object sender, EndpointFileOperationEventArgs e)
        {
            //Adding files should be fine right? right?
            //if (!this.MonitorActive)
            //{
            //    return;
            //}
            //if (IsUserConnectingFromKnownEndpoint(this.ParentEndpoint.CurrentUsername, this.ParentEndpoint.ConnectedFrom))
            //{
            //    return;
            //}
            //if (e.File.IsMalicious)
            //{
            //    StartActiveTrace();
            //}
        }

        private void ParentEndpoint_OnFileGet(object sender, EndpointFileOperationEventArgs e)
        {
            if (!MonitorActive)
            {
                return;
            }
            if (e.File.IsWatched)
            {
                StartActiveTrace();
            }
        }

        private void ParentEndpoint_OnShutdown(object sender)
        {
            this.FailedLogins = 0;
            this.SuspiciousEndpoints.Clear();
        }

        private void ParentEndpoint_OnLogin(object sender, EndpointLoginEventArgs e)
        {
            ResetLoginCount();
            if (!MonitorActive)
            {
                return;
            }
            if (!CheckIfAdminOrigin(e))
            {
                this.SuspiciousEndpoints.Add(e.From);
                StartActiveTrace();
            }
        }

        private void ParentEndpoint_OnConnected(object sender, EndpointConnectedEventArgs e)
        {
            if (!this.MonitorActive)
            {
                return;
            }
            if (this.SuspiciousEndpoints.Contains(e.From))
            {
                StartActiveTrace();
            }
        }

        private void ParentEndpoint_OnDisconnected(object sender, EndpointDisconnectedEventArgs e)
        {
            ResetLoginCount();
            if (this.StartPassiveTraceOnDisconnect)
            {
                bool fromDisconnect = false;
                LogItem log = this.ParentEndpoint.FindConnectedLog(e.From);
                if (log == null)
                {
                    fromDisconnect = true;
                    log = this.ParentEndpoint.FindDisconnectedLog(e.From);
                }
                if (log == null)
                {
                    //if we get here that means there is neither a connection nor disconnection log for the connection we wanna trace :(
                    return;
                }
                new PassiveTraceTracker().StartTrace(this.ParentEndpoint, e.From, log.TimeStamp, fromDisconnect);
            }
        }

        private bool CheckIfAdminOrigin(EndpointLoginEventArgs e)
        {
            if (e.From == null)
            {
                return true;
            }
            if (ParentEndpoint.UsernamePasswordAccessDict[e.Username + e.Password] != AccessLevel.ADMIN)
            {
                return true;
            }
            foreach (Guid pId in ParentEndpoint.UsernamePasswordDict.Keys)
            {
                Person p = Global.AllPersonsDict[pId];
                if (p.Name != e.Username)
                {
                    continue;
                }
                if (p.PersonalComputer != e.From)
                {
                    //Admin connected from a unknown point start trace
                    return false;
                }
                return true;
            }
            throw new Exception("CheckIfAdminOrigin: The username we are checking is not link to a person registered at the endpoint!");
        }

        private bool IsUserConnectingFromKnownEndpoint(string username, Endpoint from)
        {
            foreach (Guid pId in this.ParentEndpoint.UsernamePasswordDict.Keys)
            {
                Person p = Global.AllPersonsDict[pId];
                if (p.Name != username)
                {
                    continue;
                }
                if (p.PersonalComputer != from)
                {
                    return false;
                }
                return true;
            }
            throw new Exception("IsUserConnectingFromKnownEndpoint: The username we are checking is not link to a person registered at the endpoint!");
        }

        private void ParentEndpoint_OnFailedLogin(object sender, EndpointLoginEventArgs e)
        {
            this.FailedLogins += 1;
            if (this.FailedLogins >= 5)
            {
                this.SuspiciousEndpoints.Add(e.From);
                StartActiveTrace();
            }
        }

        private void StartActiveTrace()
        {
            SuspiciousEndpoints.Add(ParentEndpoint.ConnectedFrom);
            ActiveTraceTracker.Instance.StartTrace(ParentEndpoint.TraceSpeed);
        }

        private void ResetLoginCount()
        {
            this.FailedLogins = 0;
        }
    }
}