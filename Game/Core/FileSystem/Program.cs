using System;

namespace Game.Core.FileSystem
{
    public class Program
    {
        public Folder Folder { set; get; }
        public string Name { private set; get; }
        public byte[] content { private set; get; }
        public bool Executable = false;

        public Program(string name, bool executable)
        {
            this.Name = name;
            this.Executable = executable;
            Random rand = new Random();
            content = new byte[512];
            rand.NextBytes(content);
        }

        public override string ToString()
        {
            return System.Text.Encoding.ASCII.GetString(this.content);
        }
    }
}