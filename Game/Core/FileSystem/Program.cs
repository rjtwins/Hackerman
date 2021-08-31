using Game.Core.Endpoints;
using System;

namespace Game.Core.FileSystem
{
    public class Program : ICloneable
    {
        public Folder Folder { set; get; }
        public string Name { protected set; get; }
        public byte[] content { private set; get; }
        public bool Executable = false;
        public Guid Id { private set; get; }
        public bool IsMalicious = false;
        protected bool Running = false;
        protected int Version = 1;

        public Program(string name, bool executable = false)
        {
            this.Name = name;
            this.Executable = executable;
            this.Id = Guid.NewGuid();
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
            return "";
        }

        public virtual void StopProgram()
        {
            if (!this.Running)
            {
                return;
            }
            this.Running = false;

        }
    }
}