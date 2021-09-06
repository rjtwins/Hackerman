using Game.Core.Endpoints;
using Game.UI;
using System;

namespace Game.Core.FileSystem
{
    public class Program : ICloneable
    {
        public Folder Folder { set; get; }
        public string Name { protected set; get; }
        public byte[] content { private set; get; }
        public bool Executable = false;
        public Guid Id { protected set; get; }
        public bool IsMalicious = false;
        protected bool Running = false;
        protected int Version = 1;
        public ProgramWindow ProgramWindow { get; protected set; }
        private bool NeedsConnection { get; set; }

        private Endpoint _ranOn = null;

        protected bool RunOnRemote { get; set; } = false;
        public Endpoint RanOn
        {
            get { return _ranOn; }
            protected set { _ranOn = value; }
        }

        public Program(string name, bool executable = false, bool needsConnection = false)
        {
            this.Name = name;
            this.Executable = executable;
            this.Id = Guid.NewGuid();
            this.NeedsConnection = needsConnection;
            Random rand = new Random();
            content = new byte[512];
            rand.NextBytes(content);
        }

        public override string ToString()
        {
            return System.Text.Encoding.ASCII.GetString(this.content);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public Program Copy()
        {
            return (Program)this.Clone();
        }

        public virtual string RunProgram(Endpoint ranOn)
        {
            this.Running = true;
            this.RanOn = ranOn;
            this.RanOn.ActivePrograms.Add(this);
            if(this.RanOn != Global.LocalEndpoint && NeedsConnection)
            {
                this.RunOnRemote = true;
                this.RanOn.OnDisconnected += RanOn_OnDisconnected;
            }
            return string.Empty;
        }

        private void RanOn_OnDisconnected(object sender, EndpointDisconnectedEventArgs e)
        {
            if (!NeedsConnection)
            {
                return;
            }
            this.RanOn.OnDisconnected -= RanOn_OnDisconnected;
            this.StopProgram(true);
        }

        public virtual void StopProgram(bool remoteLost = false)
        {
            if (!this.Running)
            {
                return;
            }
            this.Running = false;
            this.RanOn.ActivePrograms.Remove(this);
            if(this.ProgramWindow == null)
            {
                return;
            }
            this.ProgramWindow.Close();
        }
    }
}