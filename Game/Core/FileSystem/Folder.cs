using System.Collections.Generic;

namespace Game.Core.FileSystem
{
    public class Folder
    {
        public AccessLevel AccessLevel = AccessLevel.USER;
        public string Name { protected set; get; }
        public FileSystem ParentFileSystem { protected set; get; }
        public Folder Parent { protected set; get; }

        public string Owner { set; get; } = string.Empty;
        public Dictionary<string, Folder> Folders { protected set; get; } = new Dictionary<string, Folder>();
        public Dictionary<string, Program> Programs { private set; get; } = new Dictionary<string, Program>();

        public Folder(Folder parent, string name)
        {
            this.Name = name;
            this.Parent = parent;
            this.ParentFileSystem = parent.ParentFileSystem;
            this.ParentFileSystem.AllFolders.Add(this);
        }

        public bool IsPerson(string Person)
        {
            return this.Owner == Person;
        }

        public void AddProgram(Program p)
        {
            p.Folder = this;
            this.Programs[p.Name] = p;
        }

        public Folder(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            if (Parent == this)
            {
                return @"root:";
            }
            return Parent.ToString() + @"\" + this.Name;
        }

        internal void RemoveProgram(Program p)
        {
            p.Folder = null;
            this.Programs.Remove(p.Name);
        }
    }
}