using Game.Core.Console;
using Game.Core.FileSystem;
using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    /// <summary>
    /// The endpoint represents a machine that can be connected to.
    /// It has a filesystem to interact with.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public partial class Endpoint
    {
        #region Backing fields
        [JsonProperty]
        private ReferenceList<Program> activePrograms = new(Global.AllProgramsDict, "AllProgramsDict");
        [JsonProperty]
        private List<LogItem> systemLog = new();
        private bool isLocalEndpoint = false;
        [JsonProperty]
        private bool connectedToo = false;
        [JsonProperty]
        private EndpointEvents endpointEvents;
        [JsonProperty]
        private DateTime nextAdminCheckDate;
        [JsonProperty]
        private DateTime nextRestartDate;
        [JsonProperty]
        private ReferenceList<Endpoint> allowedConnections = new(Global.AllEndpointsDict, "AllEndpointsDict");
        [JsonProperty]
        private List<(string, string)> loginHistory = new();
        [JsonProperty]
        private bool firewallActive = false;
        [JsonProperty]
        private ReferenceList<Endpoint> watchedEndpoints = new(Global.AllEndpointsDict, "AllEndpointsDict");
        [JsonProperty]
        private Dictionary<Guid, string> usernamePasswordDict = new();
        [JsonProperty]
        private Dictionary<string, AccessLevel> usernamePasswordAccesDict = new();
        [JsonProperty]
        private AccessLevel accesLevel = AccessLevel.USER;
        [JsonProperty]
        private string currentUsername = string.Empty;
        [JsonProperty]
        private string currentPassword = string.Empty;
        [JsonProperty]
        private string iPAddress;
        [JsonProperty]
        private Guid id;
        [JsonProperty]
        private bool softConnection;
        [JsonProperty]
        private FileSystem.FileSystem fileSystem;
        [JsonProperty]
        private Endpoint connectedFrom;
        [JsonProperty]
        private HashSet<Endpoint> loggedInEndpoints = new();
        [JsonProperty]
        private HashSet<string> loggedInUsernames = new();
        [JsonProperty]
        #endregion

        #region Properties
        public bool IsLocalEndpoint
        {
            get { return isLocalEndpoint; }
            set { isLocalEndpoint = value; }
        }
        public bool ConnectedToo
        {
            get { return connectedToo; }
            set { connectedToo = value; }
        }
        public EndpointEvents EndpointEvents
        {
            get { return endpointEvents; }
            set { endpointEvents = value; }
        }
        public DateTime NextAdminCheckDate
        {
            get { return nextAdminCheckDate; }
            set { nextAdminCheckDate = value; }
        }
        public DateTime NextRestartDate
        {
            get { return nextRestartDate; }
            set { nextRestartDate = value; }
        }
        public ReferenceList<Endpoint> AllowedConnections
        {
            get { return allowedConnections; }
            set { allowedConnections = value; }
        }
        public List<(string,string)> LoginHistory
        {
            get { return loginHistory; }
            set { loginHistory = value; }
        }
        public bool MonitorActive
        {
            get { return this.EndpointMonitor.MonitorActive; }
            private set { this.EndpointMonitor.MonitorActive = value; }
        }
        private EndpointMonitor EndpointMonitor { get; set; }
        public bool FirewallActive
        {
            get { return firewallActive; }
            set { firewallActive = value; }
        }
        public ReferenceList<Program> ActivePrograms
        {
            get { return activePrograms; }
            set { activePrograms = value; }
        }
        public List<LogItem> SystemLog
        {
            get { return systemLog; }
            set { systemLog = value; }
        }
        public ReferenceList<Endpoint> WatchedEndpoints
        {
            get { return watchedEndpoints; }
            set { watchedEndpoints = value; }
        }
        public Dictionary<Guid, string> UsernamePasswordDict
        {
            get { return usernamePasswordDict; }
            set { usernamePasswordDict = value; }
        }
        public Dictionary<string, AccessLevel> UsernamePasswordAccessDict
        {
            get { return usernamePasswordAccesDict; }
            set { usernamePasswordAccesDict = value; }
        }
        public AccessLevel AccessLevel
        {
            get { return accesLevel; }
            set { accesLevel = value; }
        }
        public string CurrentUsername
        {
            get { return currentUsername; }
            set { currentUsername = value; }
        }
        public string CurrentPassword
        {
            get { return currentPassword; }
            set { currentPassword = value; }
        }
        public string IPAddress
        {
            get
            {
                return iPAddress.ToString();
            }
            protected set
            {
                this.iPAddress = value;
            }
        }
        public Guid Id
        {
            get { return id; }
            private set { id = value; }
        }
        public bool SoftConnection
        {
            get { return softConnection; }
            set { softConnection = value; }
        }
        public FileSystem.FileSystem FileSystem
        {
            get { return fileSystem; }
            set { fileSystem = value; }
        }
        public Endpoint ConnectedFrom
        {
            get { return connectedFrom; }
            set { connectedFrom = value; }
        }
        public HashSet<Endpoint> LoggedInEndpoints
        {
            get { return loggedInEndpoints; }
            set { loggedInEndpoints = value; }
        }
        public HashSet<string> LoggedInUsernames
        {
            get { return loggedInUsernames; }
            set { loggedInUsernames = value; }
        }
        #endregion
        //TODO: make it so if you spoof the network it will not block you.


        [JsonConstructor]
        public Endpoint()
        {

        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            AllowedConnections.SetReferenceDict(Global.AllEndpointsDict);
            ActivePrograms.SetReferenceDict(Global.AllProgramsDict);
            WatchedEndpoints.SetReferenceDict(Global.AllEndpointsDict);
        }

        public SoftwareLevel GetMonitorLevel()
        {
            return this.EndpointMonitor.Level;
        }

        public void SetMonitorLevel(SoftwareLevel value)
        {
            this.EndpointMonitor.Level = value;
        }

        public bool CanBreachFirewall()
        {
            if (!HasFirewall())
                return true;
            if (!FirewallActive)
                return true;
            if (((int)LocalSystem.Intance.FirewallBypass) > ((int)this.Firewall)
                && LocalSystem.Intance.FirewallBypassActive)
            {
                return true;
            }
            return false;
        }

        public bool HasFirewall()
        {
            if (((int)this.Firewall) > ((int)SoftwareLevel.LVL0))
            {
                return true;
            }
            return false;
        }

        private void SetupEndpoint()
        {
            if (this.IsLocalEndpoint)
            {
                return;
            }

            //Generate IPAdress with Guid as seed
            var data = new byte[4];
            new Random(Id.GetHashCode()).NextBytes(data);
            data[0] |= 1;
            iPAddress = new IPAddress(data).ToString();
            this.FileSystem = new FileSystem.FileSystem(this);

            switch (this.EndpointType)
            {
                case EndpointType.PERSONAL:
                    this.Name = this.Owner.Name + "'s Desktop";
                    break;

                case EndpointType.EXTERNALACCES:
                    this.Name = this.Owner.Name + " External Access Server";
                    break;

                case EndpointType.INTERNAL:
                    this.Name = this.Owner.Name + " Internal Services";
                    break;

                case EndpointType.WEB:
                    this.Name = this.Owner.Name + " Web Server";
                    break;

                case EndpointType.BANK:
                    this.Name = this.Owner.Name;
                    break;

                case EndpointType.MEDIA:
                    break;

                case EndpointType.DATABASE:
                    this.Name = this.Owner.Name + " Storage Server";
                    this.IsHidden = true;
                    break;

                case EndpointType.GOVERMENT:
                    this.Name = this.Owner.Name;
                    break;

                default:
                    break;
            }
            this.EndpointEvents = new EndpointEvents(this);
            this.EndpointMonitor = new(this);
        }

        internal string PrintSchedule()
        {
            string result = "SCHEDULE:\n"
                + this.NextRestartDate.ToString() + "Scheduled automatic restart.\n"
                + this.NextAdminCheckDate.ToString() + "Scheduled administrative maintenance.\n";
            return result;
        }

        internal bool HasConnection()
        {
            if (this.SoftConnection)
            {
                return true;
            }
            return false;
        }

        internal string GetPassword(string user)
        {
            foreach (var person in UsernamePasswordDict.Keys)
            {
                if (Global.AllPersonsDict[person].Name != user)
                {
                    continue;
                }
                return UsernamePasswordDict[person];
            }
            return string.Empty;
        }

        internal string GetPassword(Person user)
        {
            if (UsernamePasswordDict.TryGetValue(user.Id, out string password))
            {
                return password;
            }
            return string.Empty;
        }

        internal Person GetRandomUser(bool noSystemUsers = true)
        {
            int nrUsers = this.UsernamePasswordDict.Keys.Count;

            Person randomUser = Global.AllPersonsDict[UsernamePasswordDict.Keys.ToList()[Global.Rand.Next(nrUsers)]];

            if (nrUsers == 1)
            {
                return null;
            }

            if (!noSystemUsers)
            {
                return randomUser;
            }

            switch (randomUser.Name)
            {
                case "root":
                    return GetRandomUser();

                default:
                    break;
            }

            return randomUser;
        }

        internal string PrintUsers()
        {
            string result = "USER:\t\tTYPE:\n";
            foreach (Guid personId in this.UsernamePasswordDict.Keys)
            {
                Person person = Global.AllPersonsDict[personId];
                result += person.Name + "\t\t" + this.UsernamePasswordAccessDict[person.Name + person.WorkPassword].ToString() + "\n";
            }
            return result;
        }

        internal void AddEmployes(Endpoint[] employes)
        {
            foreach (Endpoint e in employes)
            {
                this.AddUser(e.Owner, e.Owner.WorkPassword, AccessLevel.USER);
            }
        }

        public void AddUser(Person person, string password, AccessLevel accessLevel)
        {
            this.UsernamePasswordDict[person.Id] = password;
            this.UsernamePasswordAccessDict[person.Name + password] = accessLevel;
            Folder newFolder = FileSystem.MakeFolderFromPath(@"root\users\" + person.Name);
            if (newFolder != null)
            {
                newFolder.AccessLevel = accessLevel;
                newFolder.Owner = person.Name;
            }
        }

        internal void BounceTo(Endpoint from, Endpoint too)
        {
            if (from == null || too == null)
            {
                return;
            }
            LogConnectionRouted(from, too);
        }

        internal void Discconect()
        {
            this.SoftConnection = false;
            Global.RemoteSystem = null;
            if (OnDisconnected == null)
            {
                return;
            }
            OnDisconnected(this, new(this.ConnectedFrom));
            LogDisconnected();
            FileSystem.ResetConnection();
            CurrentUsername = string.Empty;
            CurrentPassword = string.Empty;
            this.ConnectedFrom = null;
        }

        internal string CurrentPath()
        {
            string result = FileSystem.CurrentFolder.ToString();
            result = result.Remove(0, 4);
            if (result == ":")
            {
                result = @":\";
            }

            return CurrentUsername + @"@" + this.IPAddress + result;
        }

        internal string ListFromCurrentFolder()
        {
            string listString = "";
            listString = this.FileSystem.ListFromCurrentFolder();
            return listString;
        }

        internal Program GetFile(string folderPath, string fileName)
        {
            Program p = this.FileSystem.GetFileFromPath(folderPath, fileName, CurrentUsername);
            if (this.OnFileGet != null)
            {
                this.OnFileGet(this, new(this.ConnectedFrom, p));
            }
            return p;
        }

        internal string TryPrintFile(string command)
        {
            return FileSystem.TryPrintFile(command);
        }

        internal string RunProgram(string path)
        {
            this.LogFileRan(path);
            if(this.OnFileRun != null)
            {
                OnFileRun(this, new(this.ConnectedFrom, FileSystem.GetFileFromPath(path)));
            }
            return FileSystem.TryRunFile(path);
        }

        internal string PrintActivePrograms()
        {
            string result = string.Empty;
            this.ActivePrograms.ForEach(x => result += Global.AllEndpointsDict[x].Name);
            return result;
        }

        internal string NavigateTo(string command)
        {
            FileSystem.NavigateTo(command, CurrentUsername);
            return CurrentPath();
        }

        internal string UploadFileToo(string path, Program p, bool log = true, bool o = false)
        {
            string result = this.FileSystem.CopyFileToFonder(path, p, CurrentUsername, o);
            if (result != "Done") 
            {
                return result;
            }
            if(log && !this.IsLocalEndpoint)
            {
                this.SystemLog.Add(LogItemBuilder
                    .Builder()
                    .FILE_COPIED()
                    .From(this.ConnectedFrom)
                    .User(this.CurrentUsername)
                    .AccesLevel(this.AccessLevel)
                    .TimeStamp(Global.GameTime));
            }
            if(this.OnFileAdd != null)
            {
                this.OnFileAdd(this, new(ConnectedFrom, p));
            }
            return result;
        }

        internal string RemoveFileFrom(string path, Program p, bool log = true)
        {
            string result = this.FileSystem.RemoveFileFromFolder(path, p, CurrentUsername);
            if (result != "Done") 
            {
                return result;
            }
            if (log)
            {
                this.SystemLog.Add(LogItemBuilder
                    .Builder()
                    .FILE_DELETED()
                    .From(this.ConnectedFrom)
                    .User(this.CurrentUsername)
                    .AccesLevel(this.AccessLevel)
                    .TimeStamp(Global.GameTime));
            }
            if(this.OnFileRemove != null)
            {
                this.OnFileRemove(this, new(ConnectedFrom, p));
            }
            throw new NotImplementedException();
        }

        internal bool AllowsConnection(Endpoint from)
        {
            foreach (Endpoint e in AllowedConnections)
            {
                Debug.WriteLine(e.Name);
            }

            if (AllowedConnections.Count == 0)
            {
                return true;
            }
            if (AllowedConnections.Contains(from))
            {
                return true;
            }
            return false;
        }

        internal void ConnectToo(Endpoint from)
        {
            this.SoftConnection = true;

            if (OnConnected != null)
            {
                EndpointConnectedEventArgs args = new EndpointConnectedEventArgs(null);
                OnConnected(this, args);
            }
        }

        internal string LogInToo(string username, string password, Endpoint from, bool fromProgram = false)
        {
            if ((int)State > (int)EndpointState.STARTING)
            {
                if (fromProgram)
                {
                    return string.Empty;
                }
                throw new Exception("Remote machine unreachable.");
            }

            if (from == null)
            {
                //??? not possible
                throw new Exception("Attempted to connect to endpoint from a null address this should not be possible.");
            }

            LogConnectionAttempt(username, from);

            if(this.LoggedInUsernames.Contains(username) ||
                this.LoggedInEndpoints.Contains(from))
            {
                throw new Exception($"User {username} is already logged in.");
            }

            if (UsernamePasswordAccessDict.TryGetValue(username + password, out AccessLevel temp))
            {
                this.AccessLevel = temp;
                FileSystem.ConnectTo(this.AccessLevel, username);
                LogConnectionSucces(username, from, this.AccessLevel);
                CurrentUsername = username;
                CurrentPassword = password;
                this.ConnectedFrom = from;
                this.LoginHistory.Add((username, password));

                if (OnLogin != null)
                {
                    EndpointLoginEventArgs args = new EndpointLoginEventArgs(from, username, password, this.MemoryHashing);
                    OnLogin(this, args);
                }
                return "Logged in as: " + username;
            }
            else
            {
                LogConnectionFailed(username, from);
                if(this.OnFailedLogin != null)
                {
                    OnFailedLogin(this, new EndpointLoginEventArgs(from, username, password, this.MemoryHashing));
                }
                if (!fromProgram)
                    throw new Exception("Username password combination not found.");
                return string.Empty;
            }
        }

        internal void LoggedInTo(string username, string password, Endpoint too)
        {
            if(OnLoggedIn != null)
            {
                EndpointLoggedInEventArgs e = new EndpointLoggedInEventArgs(too, username, password, too.MemoryHashing);
                OnLoggedIn(this, e);
            }
        }

        #region Mock Login

        /// <summary>
        /// Faking a user remotely logging in on this machine.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="from"></param>
        internal void MockRemoteLogInToo(string username, string password, Endpoint from)
        {
            if(this.LoggedInEndpoints.Contains(from) || this.LoggedInUsernames.Contains(username))
            {
                return;
            }
            AccessLevel accessLevel = UsernamePasswordAccessDict[username + password];
            LogConnectionAttempt(username, from);
            LogConnectionSucces(username, from, accessLevel);
            this.LoginHistory.Add((username, password));
            this.LoggedInEndpoints.Add(from);
            this.LoggedInUsernames.Add(username);

            if (OnLogin != null)
            {
                EndpointLoginEventArgs args = new EndpointLoginEventArgs(from, username, password, this.MemoryHashing);
                OnLogin(this, args);
            }
            Task.Factory.StartNew(() =>
            {
                string taskUsername = username;
                Endpoint taskEndpoint = from;
                AccessLevel taskAccesLevel = accessLevel;
                //Sleep for upto 4 ingame hours before logout
                Global.EventTicker.SleepSeconds(Global.Rand.Next(14400));
                this.LoggedInEndpoints.Remove(taskEndpoint);
                this.LoggedInUsernames.Remove(taskUsername);
                this.SystemLog.Add(LogItemBuilder.Builder()
                .CONNECTION_DISCONNECTED()
                .From(taskEndpoint)
                .User(taskUsername)
                .AccesLevel(accessLevel)
                .TimeStamp(Global.GameTime)
                );
            });
        }

        /// <summary>
        /// Faking a user logging in physically on this machine.
        /// <param name="username"></param>
        /// <param name="password"></param>
        internal void MockLocalLogInToo(string username, string password)
        {
            if (this.LoggedInUsernames.Contains(username))
            {
                return;
            }
            AccessLevel accessLevel = UsernamePasswordAccessDict[username + password];
            LogConnectionSucces(username, null, accessLevel);
            this.LoginHistory.Add((username, password));
            this.LoggedInUsernames.Add(username);

            if (OnLogin != null)
            {
                EndpointLoginEventArgs args = new EndpointLoginEventArgs(null, username, password, this.MemoryHashing);
                OnLogin(this, args);
            }
            Task.Factory.StartNew(() =>
            {
                string taskUsername = username;
                AccessLevel taskAccesLevel = accessLevel;
                //Sleep for upto 4 ingame hours before logout
                Global.EventTicker.SleepSeconds(Global.Rand.Next(14400));
                this.LoggedInUsernames.Remove(username);
                this.SystemLog.Add(LogItemBuilder.Builder()
                .CONNECTION_DISCONNECTED()
                .From(null)
                .User(taskUsername)
                .AccesLevel(accessLevel)
                .TimeStamp(Global.GameTime)
                );
            });
        }

        #endregion

        #region System Logs

        private void LogConnectionRouted(Endpoint from, Endpoint too)
        {
            this.SystemLog.Add(LogItemBuilder.Builder()
                .CONNECTION_ROUTED()
                .Between(from, too)
                .User(this.CurrentUsername)
                .AccesLevel(this.AccessLevel)
                .TimeStamp(Global.GameTime));
        }

        private void LogConnectionFailed(string username, Endpoint from)
        {
            this.SystemLog.Add(LogItemBuilder
                .Builder()
                .CONNECTION_FAILED()
                .From(from)
                .User(username)
                .AccesLevel(AccessLevel.NONE)
                .TimeStamp(Global.GameTime)
                );
        }

        private void LogConnectionSucces(string username, Endpoint from, AccessLevel accessLevel)
        {
            this.SystemLog.Add(LogItemBuilder
                .Builder()
                .CONNECTION_SUCCES()
                .From(from)
                .User(username)
                .AccesLevel(accessLevel)
                .TimeStamp(Global.GameTime)
                );
        }

        private void LogConnectionAttempt(string username, Endpoint from)
        {
            this.SystemLog.Add(LogItemBuilder
                .Builder()
                .CONNECTION_ATTEMPT()
                .From(from)
                .User(username)
                .AccesLevel(AccessLevel.NONE)
                .TimeStamp(Global.GameTime)
                );
        }

        private void LogDisconnected()
        {
            this.SystemLog.Add(LogItemBuilder.Builder()
                .CONNECTION_DISCONNECTED()
                .From(this.ConnectedFrom)
                .User(this.CurrentUsername)
                .AccesLevel(this.AccessLevel)
                .TimeStamp(Global.GameTime)
                );
        }

        private void LogFileRan(string path)
        {
            this.SystemLog.Add(LogItemBuilder
                .Builder()
                .FILE_RUN(path)
                .From(this.ConnectedFrom)
                .User(this.CurrentUsername)
                .AccesLevel(this.AccessLevel)
                .TimeStamp(Global.GameTime)
                );
        }

        #endregion Connection logs

        #region Shutdown/Startup
        internal virtual void AutoRestart()
        {
            if (this.IsLocalEndpoint)
            {
                return;
            }
            this.Restart();
            this.EndpointEvents.ScheduleNextRestart();
        }

        protected virtual void Restart()
        {
            shutdown();
            Task.Factory.StartNew(() =>
            {
                Global.EventTicker.SleepSeconds(120);
                startup();
            });
        }

        protected virtual void startup()
        {
            this.FileSystem.CurrentFolder = this.FileSystem;
            State = EndpointState.STARTING;
            RunStartupPrograms();
            if ((int)State > (int)EndpointState.STARTING)
            {
                return;
            }
            State = EndpointState.ONLINE;
        }

        protected virtual void RunStartupPrograms()
        {
            this.FileSystem.RunStartupPrograms();
        }

        protected virtual void shutdown()
        {
            State = EndpointState.SHUTTINGDOWN;
            this.SoftConnection = false;
            FileSystem.ResetConnection();
            CurrentUsername = string.Empty;
            CurrentPassword = string.Empty;
            this.FileSystem.AllFolders.ToList().ForEach(folder => folder.Programs.Values.ToList().ForEach(program => program.StopProgram()));

            if (Global.RemoteSystem == null)
            {
                return;
            }
            if (Global.RemoteSystem.Id == this.Id)
            {
                Global.RemoteConsole.CommandParser.ExitDisconnectFromThread();
                Discconect();
            }
        }
        #endregion

        internal virtual void AdminSystemCheck()
        {
            this.UsernamePasswordDict.ToList().ForEach(x => LoginIfAdmin(x));

            if (this.IsLocalEndpoint)
            {
                return;
            }

            #region logs
            //Trim Logs to 20 entries.
            while (this.SystemLog.Count > 20)
            {
                this.SystemLog.RemoveAt(this.SystemLog.Count - 1);
            }
            #endregion logs

            #region virus scan

            foreach (Folder f in this.FileSystem.AllFolders)
            {
                foreach (Program p in f.Programs.Values)
                {
                    if (p.IsMalicious)
                    {
                        p.StopProgram();
                        f.RemoveProgram(p);
                    }
                }
            }

            #endregion virus scan

            this.EndpointEvents.ScheduleNextAdminCheck();

            void LoginIfAdmin(KeyValuePair<Guid, string> x)
            {
                if (UsernamePasswordAccessDict[Global.AllPersonsDict[x.Key].Name + x.Value] == AccessLevel.ADMIN)
                {
                    this.MockLocalLogInToo(Global.AllPersonsDict[x.Key].Name, x.Value);
                }
            }
        }

        public string PrintLogs()
        {
            string result = "SYSTEM LOG:\n";
            foreach (LogItem item in this.SystemLog)
            {
                string acceslevel = item.AccessLevel.ToString();
                string ipAddress = "127.0.0.1";

                if (item.From != null)
                {
                    ipAddress = item.From.IPAddress;
                }
                result += item.TimeStamp.ToString() + "\t";

                switch (item.LogType)
                {
                    case LogType.CONNECTION_ATTEMPT:
                        result += "LOGIN ATTEMPT:\t\t" + ipAddress;
                        break;
                    case LogType.CONNECTION_FAILED:
                        result += "FAILED AUTHENTICAION:\t" + ipAddress;
                        break;
                    case LogType.CONNECTION_SUCCES:
                        result += "CONNECTION SUCCESFULL:\t" + ipAddress + "\t ACCESS: " + acceslevel;
                        break;
                    case LogType.CONNECTION_ROUTED:
                        result += "ROUTED:\t\t" + ipAddress + " : " + item.Too.IPAddress;
                        break;
                    case LogType.FILE_EDITED:
                        result += "FILE EDIT:\t\t" + ipAddress + " PATH: " + item.FilePath;
                        break;
                    case LogType.FILE_COPIED:
                        result += "FILE COPIED:\t\t" + ipAddress + " PATH: " + item.FilePath;
                        break;
                    case LogType.FILE_DELETED:
                        result += "FILE DELETED:\t\t" + ipAddress + " PATH: " + item.FilePath;
                        break;
                    case LogType.FILE_RUN:
                        result += "FILE RUN:\t\t" + ipAddress + " PATH: " + item.FilePath;
                        break;
                    case LogType.CREDITTRANSFER:
                        result += "M-BANKING USER:" + item.UserName + "@" + ipAddress + "\t FROM: " + item.TransferFrom.Name + "@" + item.FromBank + "\t " + item.TransferToo + "@" + item.TooBank;
                        break;
                    default:
                        break;
                }
                result += Environment.NewLine;
            }
            return result;
        }

        public void RemoveLogItem(LogItem item)
        {
            if (!this.SystemLog.Contains(item))
            {
                return;
            }
            this.SystemLog.Remove(item);
        }

        public List<LogItem> GetSystemLogs()
        {
            return this.SystemLog;
        }
    }
}