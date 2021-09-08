using Game.Core.Endpoints;
using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console.RemotePrograms
{
    public class Worm : Program
    {
        bool WebWorm { get; set; }
        bool TwoWayConnection { get; set; }
        private Type Payload { get; set; }
        
        public Worm(string name = "worm.exe", SoftwareLevel Level = SoftwareLevel.LVL0, Type payload = null, bool webWorm = false, bool twoWayInjection = false) : base(name, true)
        {
            this.SoftwareLevel = Level;
            this.Payload = payload;
            this.IsMalicious = true;
            this.WebWorm = webWorm;
            this.TwoWayConnection = twoWayInjection;
        }

        public override string RunProgram(Endpoint ranOn)
        {
            if (AnotherInstanceActive(ranOn))
            {
                return $"{this.Name} close other instances before executing this program.";
            }
            base.RunProgram(ranOn);
            this.RanOn.OnLogin += RanOn_OnLogin;

            string result = InjectPayload(ranOn, ranOn.CurrentUsername);

            //Infect the website so anybody who connects gets the worm.
            if (this.WebWorm && ranOn.GetType() == typeof(WebServerEndpoint))
            {
                ((WebServerEndpoint)ranOn).OnVisit += RanOn_OnVisit;
            }

            if (TwoWayConnection)
            {
                ranOn.OnLoggedIn += RanOn_OnLoggedIn;
            }

            return $"WORMENGINE {this.Name} starting...hooked. \n {result}.";
        }
        private void RanOn_OnLoggedIn(object sender, EndpointLoggedInEventArgs e)
        {
            RanOn_OnLogin(e.Too);
        }
        private void RanOn_OnVisit(object sender, WebEndpointVisitEventArgs e)
        {
            RanOn_OnLogin(e.From);
        }
        private bool AnotherInstanceActive(Endpoint ranOn)
        {
            if (ranOn.ActivePrograms.Contains(this))
            {
                return true;
            }
            if (!ranOn.ActivePrograms.TrueForAll(x => x.Id != this.Id))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="payloadInstance"></param>
        /// <returns>string result</returns>
        private string InjectPayload(Endpoint target, string username = "shared")
        {
            if(this.Payload == null)
            {
                return "Payload NONE injected";
            }

            Program payloadInstance = (Program)Activator.CreateInstance(Payload);
            
            try
            {
                CopyRunPayload(target, payloadInstance, username);
                return $"Payload {payloadInstance.Name} injected";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in worm.InjectPayload {ex.Message} stack trace {ex.StackTrace}.");
                return $"Payload {payloadInstance.Name} could not be injected.";
            }
        }
        private void CopyRunPayload(Endpoint target, Program payloadInstance, string username = "shared")
        {
            if (((int)this.SoftwareLevel) > ((int)SoftwareLevel.LVL2))
            {
                target.UploadFileToo($"root\\system\\autostart", payloadInstance, true, true);
                target.RunProgram($"root\\system\\autostart\\{payloadInstance.Name}");
                return;
            }
            target.UploadFileToo($"root\\users\\{username}", payloadInstance, true, true);
            target.RunProgram($"root\\users\\{username}\\{payloadInstance.Name}");
        }
        private void RanOn_OnLogin(object sender, EndpointLoginEventArgs e = null)
        {
            string username = "shared";
            Endpoint from = null;

            if (e != null)
            {
                username = e.Username;
                from = e.From;
            }
            else
            {
                from = sender as Endpoint;
            }
            
            if(from == null)
            {
                return;
            }

            double roll = Global.Rand.NextDouble();
            switch (this.SoftwareLevel)
            {
                case SoftwareLevel.LVL0 when roll < 0.25:
                    InfectUsersFiles(from, username);
                    InjectPayload(from, username);
                    break;
                case SoftwareLevel.LVL1 when roll < 0.50:
                    InfectUsersFiles(from, username);
                    InjectPayload(from, username);
                    break;
                case SoftwareLevel.LVL2 when roll < 0.75:
                    InfectUsersFiles(from, username);
                    InjectPayload(from, username);
                    break;
                case SoftwareLevel.LVL3:
                    InfectUsersFiles(from, username);
                    InjectPayload(from, username);
                    break;
                case SoftwareLevel.LVL4:
                    InfectAutostart(from);
                    InjectPayload(from, username);
                    break;
                default:
                    break;
            }
        }
        private void InfectUsersFiles(Endpoint target, string username)
        {
            try
            {
                CopyInjectWormToUsersFiles(target, username);
            }
            catch (Exception)
            {

            }
        }
        private void InfectAutostart(Endpoint target)
        {
            try
            {
                CopyInjectWormToAutostart(target);
            }
            catch (Exception)
            {

            }
        }
        private void CopyInjectWormToUsersFiles(Endpoint target, string username = "shared")
        {
            Worm worm = CopyWormAndPayload();
            target.UploadFileToo($"root\\users\\{username}", worm, true, true);
            target.RunProgram($"root\\users\\{username}\\{worm.Name}");
        }
        private void CopyInjectWormToAutostart(Endpoint target)
        {
            Worm worm = CopyWormAndPayload();
            target.UploadFileToo($"root\\system\\autostart", worm, true, true);
            target.RunProgram($"root\\system\\autostart\\{this.Name}");
        }
        private Worm CopyWormAndPayload()
        {
            Worm worm = new Worm(this.Name, this.SoftwareLevel, this.Payload, this.WebWorm, this.TwoWayConnection);
            worm.Id = this.Id;
            return worm;
        }
        public override void StopProgram(bool remoteLost = false)
        {
            if(this.RanOn == null)
            {
                base.StopProgram(remoteLost);
                return;
            }

            //Unsubscribe from events
            if (TwoWayConnection)
            {
                this.RanOn.OnLoggedIn -= this.RanOn_OnLoggedIn;
            }
            if (this.WebWorm && this.RanOn.GetType() == typeof(WebServerEndpoint))
            {
                ((WebServerEndpoint)this.RanOn).OnVisit -= this.RanOn_OnVisit;
            }
            this.RanOn.OnLogin -= this.RanOn_OnLogin;
            base.StopProgram(remoteLost);
        }
    }
}
