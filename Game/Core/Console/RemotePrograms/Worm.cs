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
        SoftwareLevel WormLevel = SoftwareLevel.LVL0;
        private Type Payload { get; set; }
        private bool PayloadDelivered { get; set; } = false;
        

        public Worm(string name, SoftwareLevel Level = SoftwareLevel.LVL0, Type payload = null) : base(name, true)
        {
            this.WormLevel = Level;
            this.Payload = payload;
            this.IsMalicious = true;
        }
        public override string RunProgram(Endpoint ranOn)
        {
            if (ranOn.ActivePrograms.Contains(this))
            {
                return $"{this.Name} close other instances before executing this program.";
            }
            if(!ranOn.ActivePrograms.TrueForAll(x => x.Id != this.Id))
            {
                return $"{this.Name} close other instances before executing this program.";
            }
            base.RunProgram(ranOn);
            this.RanOn.OnLogin += RanOn_OnLogin;
            string payloadName = "NONE";
            if(this.Payload != null)
            {
                Program payloadInstance = (Program)Activator.CreateInstance(Payload);
                payloadName = payloadInstance.Name;
                InjectPayload(ranOn, payloadInstance);
            }
            return $"WORMENGINE {this.Name} starting...hooked. \n Payload {payloadName} injected.";
        }

        private void InjectPayload(Endpoint target, Program payloadInstance)
        {
            if (this.PayloadDelivered)
            {
                return;
            }
            this.PayloadDelivered = true;

            try
            {
                CopyRunPayload(target, payloadInstance);
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Exception in worm.InjectPayload {ex.Message} stack trace {ex.StackTrace}.");
            }
        }

        private void CopyRunPayload(Endpoint target, Program payloadInstance)
        {
            if (((int)this.WormLevel) > ((int)SoftwareLevel.LVL2))
            {
                target.UploadFileToo($"root\\system\\autostart", payloadInstance, true);
                target.RunProgram($"root\\system\\autostart\\{payloadInstance.Name}");
                return;
            }
            target.UploadFileToo($"root\\users\\shared", payloadInstance, true);
            target.RunProgram($"root\\users\\shared\\{payloadInstance.Name}");
        }

        private void RanOn_OnLogin(object sender, EndpointLoginEventArgs e)
        {
            Endpoint from = e.From;
            if(from == null)
            {
                return;
            }

            double roll = Global.Rand.NextDouble();
            Program payloadInstance = (Program)Activator.CreateInstance(this.Payload);
            switch (this.WormLevel)
            {
                case SoftwareLevel.LVL0 when roll < 0.25:
                    InfectUsersFiles(from, e.Username);
                    InjectPayload(from, payloadInstance);
                    break;
                case SoftwareLevel.LVL1 when roll < 0.50:
                    InfectUsersFiles(from, e.Username);
                    InjectPayload(from, payloadInstance);
                    break;
                case SoftwareLevel.LVL2 when roll < 0.75:
                    InfectUsersFiles(from, e.Username);
                    InjectPayload(from, payloadInstance);
                    break;
                case SoftwareLevel.LVL3:
                    InfectUsersFiles(from, e.Username);
                    InjectPayload(from, payloadInstance);
                    break;
                case SoftwareLevel.LVL4:
                    InfectAutostart(from);
                    InjectPayload(from, payloadInstance);
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
        private void CopyInjectWormToUsersFiles(Endpoint target, string username)
        {
            Worm worm = CopyWormAndPayload();
            target.UploadFileToo($"root\\users\\{username}", worm, true);
            target.RunProgram($"root\\users\\{username}\\{worm.Name}");
        }

        private void CopyInjectWormToAutostart(Endpoint target)
        {
            Worm worm = CopyWormAndPayload();
            target.UploadFileToo($"root\\system\\autostart", worm, true);
            target.RunProgram($"root\\system\\autostart\\{this.Name}");
        }

        private Worm CopyWormAndPayload()
        {
            Worm worm = new Worm(this.Name, this.WormLevel, this.Payload);
            worm.Id = this.Id;
            return worm;
        }

        public override void StopProgram(bool remoteLost = false)
        {
            base.StopProgram(remoteLost);
        }
    }
}
