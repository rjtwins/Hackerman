using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Game.Core.FileSystem
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Folder
    {
        [JsonProperty]
        public Guid Id { private set; get; }

        public AccessLevel AccessLevel = AccessLevel.USER;

        [JsonProperty]
        public string Name { protected set; get; }
        [JsonProperty]
        private Guid parentFileSystem;
        [JsonProperty]
        private Guid parent;
        [JsonProperty]
        public string Owner { set; get; } = string.Empty;
        [JsonProperty]
        public ReferenceDictionary<string, Folder> Folders { protected set; get; } = new(Global.AllFoldersDict, "AllFoldersDict");
        [JsonProperty]
        public ReferenceDictionary<string, Program> Programs { private set; get; } = new(Global.AllProgramsDict, "AllProgramsDict");

        public Folder Parent
        {
            get
            {
                return Global.AllFoldersDict[parent];
            }
            set
            {
                parent = value.Id;
            }
        }
        public FileSystem ParentFileSystem 
        { 
            get 
            {
                Folder toReturn = null;
                Global.AllFoldersDict.TryGetValue(parentFileSystem, out toReturn);
                return (FileSystem)toReturn;
            }
            set 
            {
                if(value == null)
                {
                    return;
                }
                parentFileSystem = value.Id;
            }
        }

        [JsonConstructor]
        public Folder()
        {

        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            Programs.SetReferenceDict(Global.AllProgramsDict);
            Folders.SetReferenceDict(Global.AllFoldersDict);
        }

        public Folder(Folder parent, string name)
        {
            this.Id = Guid.NewGuid();
            Global.AllFoldersDict[Id] = this;
            this.Name = name;
            this.Parent = parent;

            if(this.ParentFileSystem != null)
            {
                this.ParentFileSystem = parent.ParentFileSystem;
                this.ParentFileSystem.AllFolders.Add(this);
            }
        }

        public bool IsPerson(string Person)
        {
            return this.Owner == Person;
        }

        public Program AddProgram(Program p)
        {
            p.Folder = this;
            this.Programs[p.Name] = p;
            return p;
        }

        public Folder(string name)
        {
            this.Id = Guid.NewGuid();
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