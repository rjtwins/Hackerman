using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core.FileSystem
{
    public class FileSystem : Folder
    {
        public AccessLevel UserAccessLevel = AccessLevel.USER;

        public Folder CurrentFolder;

        public ConnectionLog SystemLog;
        public EndpointBackend ParrentEndpoint { protected set; get; }
        public List<Program> SystemPrograms { private set; get; } = new List<Program>();
        public List<Folder> AllFolders { protected set; get; } = new List<Folder>();

        public FileSystem(EndpointBackend endpoint) : base("root")
        {
            this.ParrentEndpoint = endpoint;
            this.Parent = this;
            this.CurrentFolder = this;
            this.ParentFileSystem = this;
            this.AllFolders.Add(this);
            GenerateStandardFolderStructure();
        }

        private void GenerateStandardFolderStructure()
        {
            SystemLog = new ConnectionLog();
            AccessLevel = AccessLevel.ROOT;
            MakeFolderFromPath(@"root\system").AccessLevel = AccessLevel.ROOT;
            MakeFolderFromPath(@"root\system\logs").AccessLevel = AccessLevel.ROOT; ;
            GetFolderFromPath(@"root\system\logs").AddProgram(this.SystemLog);

            MakeFolderFromPath(@"root\users").AccessLevel = AccessLevel.ADMIN;
            MakeFolderFromPath(@"root\users\admin").AccessLevel = AccessLevel.ADMIN;
            MakeFolderFromPath(@"root\users\admin\userinfo").AccessLevel = AccessLevel.ADMIN;
            GetFolderFromPath(@"root\users\admin\userinfo").AddProgram(new TextFile("users.info"));
        }

        internal void ResetConnection()
        {
            this.CurrentFolder = this;
            this.UserAccessLevel = AccessLevel.USER;
        }

        internal string CurrentPath()
        {
            return this.CurrentFolder.ToString();
        }

        internal void ConnectTo(AccessLevel accessLevel)
        {
            this.UserAccessLevel = accessLevel;
            switch (accessLevel)
            {
                case AccessLevel.USER:
                    this.CurrentFolder = GetFolderFromPath(@"root\users");
                    break;

                case AccessLevel.ADMIN:
                    this.CurrentFolder = GetFolderFromPath(@"root\users\admin");
                    break;

                case AccessLevel.ROOT:
                    this.CurrentFolder = GetFolderFromPath(@"root");
                    break;

                default:
                    break;
            }
        }

        internal string TryPrintFile(string command)
        {
            if (CurrentFolder.Programs.ContainsKey(command))
            {
                return CurrentFolder.Programs[command].ToString();
            }
            return "File not found.";
        }

        internal string NavigateTo(string f)
        {
            //Debug.WriteLine("Attempting to navigate to: " + f);
            Folder tempFolder = null;
            string accesLevelString = string.Empty;
            if (f.StartsWith(@"root\"))
            {
                tempFolder = GetFolderFromPath(f);
            }
            else
            {
                tempFolder = GetFolderFromPath(f, CurrentFolder);
            }
            if (((int)tempFolder.AccessLevel) <= (int)UserAccessLevel)
            {
                CurrentFolder = tempFolder;
                return CurrentFolder.ToString();
            }
            accesLevelString = UTILS.AccessLevelString(tempFolder.AccessLevel);
            throw new Exception("Access Denied.\nPlease relog as " + accesLevelString + " to open this folder.\n");
        }

        internal string ListFromCurrentFolder()
        {
            List<string> folderFileList = new List<string>();
            foreach (string f in CurrentFolder.Folders.Keys)
            {
                folderFileList.Add(f.GetHashCode().ToString() + "\t" + "<DIR>" + "\t\t" + f);
            }
            foreach (string f in CurrentFolder.Programs.Keys)
            {
                folderFileList.Add(f.GetHashCode().ToString() + "\t" + "<FILE>" + "\t\t" + f);
            }
            return string.Join('\n', folderFileList);
        }

        public void AddFileToFolder(Program p, Folder f, bool o)
        {
            if (!this.AllFolders.Contains(f))
            {
                throw new Exception("The system cannot find the path specified to.");
            }
            if (f.Programs.ContainsKey(p.Name) && !o)
            {
                throw new Exception("This file already exists");
            }
            f.AddProgram(p);
        }

        public Folder MakeFolderFromPath(string path)
        {
            Folder F = this;
            Folder NewFolder = null;
            List<string> folders = path.Split('\\').ToList<string>();
            folders.RemoveAt(0); // rmove root, we are root;
            foreach (string folder in folders)
            {
                if (F.Folders.TryGetValue(folder, out Folder _F))
                {
                    F = _F;
                    continue;
                }
                NewFolder = new Folder(F, folder);
                F.Folders[folder] = NewFolder;
            }
            return NewFolder;
        }

        public void MakeFolderFromPath(string path, Folder start)
        {
            List<string> folders = path.Split('\\').ToList<string>();
            folders.RemoveAt(0); // remove root, we are root;
            Folder F = start;

            if (!this.AllFolders.Contains(start))
            {
                throw new Exception("A folder from a different filesystem was passed this should never happen!");
            }

            foreach (string folder in folders)
            {
                if (F.Folders.TryGetValue(folder, out Folder _F))
                {
                    F = _F;
                    continue;
                }
                F.Folders[folder] = new Folder(F, folder);
            }
        }

        public Folder GetFolderFromPath(string path)
        {
            Folder F = this;
            List<string> folders = path.Split('\\').ToList<string>();
            folders.RemoveAt(0); //Remove "root" because we are root

            foreach (string FString in folders)
            {
                try
                {
                    F = F.Folders[FString];
                }
                catch (KeyNotFoundException ex)
                {
                    throw new Exception("The system cannot find the path specified to.");
                }
            }
            return F;
        }

        public Folder GetFolderFromPath(string path, Folder start)
        {
            if (start.ParentFileSystem != this)
            {
                throw new Exception("A folder from a different filesystem was passed this should never happen!");
            }

            if (path == "..")
            {
                return CurrentFolder.Parent;
            }
            Folder F = start;
            List<string> folders = path.Split('\\').ToList<string>();

            foreach (string FString in folders)
            {
                if (!F.Folders.ContainsKey(FString))
                {
                    throw new Exception("The system cannot find the path specified to.");
                }
                F = F.Folders[FString];
            }
            return F;
        }
    }
}