using Game.Core.Endpoints;
using Game.Core.UIPrograms;
using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Game.Core.FileSystem
{
    [JsonObject(MemberSerialization.OptIn)]
    public class FileSystem : Folder
    {
        public AccessLevel UserAccessLevel = AccessLevel.USER;

        [JsonProperty]
        private Guid currentFolder;

        public Folder CurrentFolder
        {
            get
            {
                return Global.AllFoldersDict[currentFolder];
            }
            set
            {
                currentFolder = value.Id;
            }
        }

        [JsonProperty]
        private Guid parentEndpoint;

        public Endpoint ParentEndpoint
        {
            get
            {
                return Global.AllEndpointsDict[parentEndpoint];
            }
            set
            {
                parentEndpoint = value.Id;
            }
        }

        [JsonProperty]
        public ReferenceList<Folder> AllFolders { get; set; } = new ReferenceList<Folder>(Global.AllFoldersDict, "AllFoldersDict");

        [JsonConstructor]
        public FileSystem() : base("root")
        {

        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            AllFolders.SetReferenceDict(Global.AllFoldersDict);
        }

        public FileSystem(Endpoint endpoint) : base("root")
        {
            
            this.ParentEndpoint = endpoint;
            this.Parent = this;
            this.CurrentFolder = this;
            this.ParentFileSystem = null;
            this.AllFolders.Add(this);
            GenerateStandardFolderStructure();
        }

        private void GenerateStandardFolderStructure()
        {
            AccessLevel = AccessLevel.ROOT;
            MakeFolderFromPath(@"root\system").AccessLevel = AccessLevel.ROOT;
            MakeFolderFromPath(@"root\system\autostart").AccessLevel = AccessLevel.ROOT;

            MakeFolderFromPath(@"root\users").AccessLevel = AccessLevel.ADMIN;
            MakeFolderFromPath(@"root\users\shared").AccessLevel = AccessLevel.USER;
            MakeFolderFromPath(@"root\users\admin").AccessLevel = AccessLevel.ADMIN;
            MakeFolderFromPath(@"root\users\admin\userinfo").AccessLevel = AccessLevel.ADMIN;

            GetFolderFromPath(@"root\system").AddProgram(new Program("Apature32.dll"));
            GetFolderFromPath(@"root\system").AddProgram(new Program("Services.dll"));
            GetFolderFromPath(@"root\system").AddProgram(new Program("System32Lib.dll"));
            GetFolderFromPath(@"root\system").AddProgram(new Program("Helper32Lib.dll"));
            
            GetFolderFromPath(@"root\users\admin\userinfo").AddProgram(new TextFile("users.info"));
            GetFolderFromPath(@"root\users\shared").AddProgram(new MBanking());
        }

        internal string CopyFileToFonder(string folderPath, Program p, string user, bool o = false)
        {
            Folder f = null;
            if (folderPath == null)
            {
                f = CurrentFolder;
            }
            else if (folderPath.Contains("root"))
            {
                f = GetFolderFromPath(folderPath);
            }
            else
            {
                f = GetFolderFromPath(folderPath, CurrentFolder);
            }
            if (CheckUserAcces(user, f))
            {
                return AddFileToFolder(p.Copy(), f, o);
            }
            AccesLevelException(f);
            return "ERROR";
        }

        private static void AccesLevelException(Folder f)
        {
            string accesLevelString = f.AccessLevel.ToString();
            throw new Exception("Access Denied.\nPlease relog as " + accesLevelString + " to open this folder.\n");
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

        internal void ConnectTo(AccessLevel accessLevel, string username)
        {
            this.UserAccessLevel = accessLevel;
            switch (accessLevel)
            {
                case AccessLevel.USER:
                    this.CurrentFolder = GetFolderFromPath(@"root\users\" + username);
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

        internal Program GetFileFromPath(string path, string user = null)
        {
            string folderPath = string.Empty;
            string fileName = path;
            //Ugly else if chain but gwatever
            string[] splitPath = path.Split("\\");
            if(splitPath.Length > 1)
            {
                folderPath = string.Join("\\", splitPath[0..(splitPath.Length - 2)]);
                fileName = splitPath[splitPath.Length - 1];
            }

            Folder f = null;
            if (folderPath == string.Empty)
            {
                f = this.CurrentFolder;
            }
            else if (this.Folders.Count == 0)
            {
                f = this.CurrentFolder;
            }
            else if (folderPath.Contains("root"))
            {
                f = GetFolderFromPath(folderPath);
            }
            else
            {
                f = GetFolderFromPath(folderPath, this.CurrentFolder);
            }
            if (!CheckUserAcces(user, f))
            {
                AccesLevelException(f);
            }

            if (!f.Programs.TryGetValue(fileName, out Program p))
            {
                return null;
            }
            return p;
        }

        internal Program GetFileFromPath(string folderPath, string fileName, string user = null)
        {
            //Ugly else if chain but gwatever
            Folder f = null;
            if (folderPath == null)
            {
                f = this.CurrentFolder;
            }
            else if (this.Folders.Count == 0)
            {
                f = this.CurrentFolder;
            }
            else if (folderPath.Contains("root"))
            {
                f = GetFolderFromPath(folderPath);
            }
            else
            {
                f = GetFolderFromPath(folderPath, this.CurrentFolder);
            }
            if (!CheckUserAcces(user, f))
            {
                AccesLevelException(f);
            }

            if (!f.Programs.TryGetValue(fileName, out Program p))
            {
                return null;
            }
            return p;
        }

        internal string RemoveFileFromFolder(string path, Program p, string user = null)
        {
            Folder f = null;
            if (path == null)
            {
                f = CurrentFolder;
            }
            else if (path.Contains("root"))
            {
                f = GetFolderFromPath(path);
            }
            else
            {
                f = GetFolderFromPath(path, CurrentFolder);
            }
            if (!CheckUserAcces(user, f))
            {
                AccesLevelException(f);
            }
            if (this.ParentEndpoint.ActivePrograms.Contains(p))
            {
                return "File is currently being used by another process.";
            }
            f.RemoveProgram(p);
            return "Done";
        }

        internal string NavigateTo(string f, string user)
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
            if (CheckUserAcces(user, tempFolder))
            {
                CurrentFolder = tempFolder;
                return CurrentFolder.ToString();
            }
            accesLevelString = tempFolder.AccessLevel.ToString();
            throw new Exception("Access Denied.\nPlease relog as " + accesLevelString + " to open this folder.\n");
        }

        private bool CheckUserAcces(string user, Folder folder)
        {
            if (user == null || user == string.Empty || string.IsNullOrWhiteSpace(user))
            {
                return true;
            }
            return ((int)folder.AccessLevel) <= (int)UserAccessLevel &&
                            (folder.Owner == user || UserAccessLevel == AccessLevel.ADMIN || UserAccessLevel == AccessLevel.ROOT);
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

        /// <summary>
        /// Do not call this externally!
        /// </summary>
        /// <param name="p"></param>
        /// <param name="f"></param>
        /// <param name="o"></param>
        /// <param name="user"></param>
        private string AddFileToFolder(Program p, Folder f, bool o)
        {
            if (!this.AllFolders.Contains(f) && this.AllFolders.Count != 0)
            {
                throw new Exception("The system cannot find the path specified to.");
            }
            if (f.Programs.ContainsKey(p.Name) && !o)
            {
                throw new Exception("This file already exists.");
            }
            if (f.Programs.ContainsKey(p.Name) && o && this.ParentEndpoint.ActivePrograms.Contains(p))
            {
                throw new Exception("Cannot override active program.");
            }
            f.AddProgram(p);
            return "Done";
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
                AllFolders.Add(NewFolder);
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
                AllFolders.Add(F.Folders[folder]);
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
            //if(!CheckUserAcces(user, F))
            //{
            //    AccesLevelException(F);
            //}
            return F;
        }

        internal string TryRunFile(string path)
        {
            string[] splitPath = path.Split('\\');
            Folder tempFolder;
            if (splitPath.Length > 0)
            {
                tempFolder = GetFolderFromPath(string.Join("\\", splitPath[0..(splitPath.Length - 1)]));
                if (tempFolder.Programs.TryGetValue(splitPath[splitPath.Length - 1], out Program p))
                {
                    return p.RunProgram(this.ParentEndpoint);
                }
            }
            if (CurrentFolder.Programs.TryGetValue(path, out Program q))
            {
                return q.RunProgram(this.ParentEndpoint);
            }
            return "Program \"" + path + "\" not found.";
        }

        internal void RunStartupPrograms()
        {
            foreach (Program p in GetFolderFromPath(@"root\system\autostart").Programs.Values)
            {
                p.RunProgram(this.ParentEndpoint);
            }
        }

        public Folder GetFolderFromPath(string path, Folder start)
        {
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