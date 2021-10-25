using Game.Core.Endpoints;
using Game.UI;
using Newtonsoft.Json;
using System;

namespace Game.Core.FileSystem
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Program : ICloneable
    {
        private Guid folder;

        [JsonIgnore]
        public Folder Folder
        {
            get
            {
                return Global.AllFoldersDict[folder];
            }
            set
            {
                folder = value.Id;
            }
        }
        [JsonIgnore]
        public Endpoint RanOn
        {
            get { return Global.AllEndpointsDict[ranOn]; }
            protected set { ranOn = value.Id; }
        }

        public string Name { get => name; set => name = value; }
        public byte[] Content { get => content; set => content = value; }
        public bool Executable { get => executable; set => executable = value; }
        public bool IsWatched { get => isWatched; set => isWatched = value; }
        public Guid Id { get => id; set => id = value; }
        public bool IsMalicious { get => isMalicious; set => isMalicious = value; }
        public bool Running { get => running; set => running = value; }
        public SoftwareLevel SoftwareLevel { get => softwareLevel; set => softwareLevel = value; }
        public ProgramWindow ProgramWindow { get => programWindow; set => programWindow = value; }
        public bool NeedsConnection { get => needsConnection; set => needsConnection = value; }
        public double AdminDetectionProp { get => adminDetectionProp; set => adminDetectionProp = value; }
        public bool RunOnRemote { get => runOnRemote; set => runOnRemote = value; }

        [JsonProperty]
        private string name;
        [JsonProperty]
        private byte[] content = null;
        [JsonProperty]
        private bool executable = false;
        [JsonProperty]
        private bool isWatched = false;
        [JsonProperty]
        private Guid id;
        [JsonProperty]
        private bool isMalicious = false;
        [JsonProperty]
        private bool running = false;
        [JsonProperty]
        private SoftwareLevel softwareLevel = SoftwareLevel.LVL0;
        [JsonProperty]
        private ProgramWindow programWindow;
        [JsonProperty]
        private bool needsConnection;
        [JsonProperty]
        private Guid ranOn;
        [JsonProperty]
        private double adminDetectionProp = 0;
        [JsonProperty]
        private bool runOnRemote = false;

        [JsonConstructor]
        public Program()
        {

        }

        public Program(string name, bool executable = false, bool needsConnection = false, bool isWatched = false)
        {
            this.Name = name;
            this.Executable = executable;
            this.Id = Guid.NewGuid();
            this.NeedsConnection = needsConnection;
            this.IsWatched = isWatched;
            Global.AllProgramsDict[Id] = this;
        }

        public override string ToString()
        {
            if(this.Content == null)
            {
                Random rand = new Random();
                Content = new byte[512];
                rand.NextBytes(Content);
            }
            return System.Text.Encoding.ASCII.GetString(this.Content);
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