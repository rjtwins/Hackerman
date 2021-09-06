using Game.Core.FileSystem;
using Game.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Game.Core.Endpoints
{
    /// <summary>
    /// The endpoint represents a machine that can be connected to.
    /// It has a filesystem to interact with.
    /// </summary>
    public partial class Endpoint
    {
        #region Backing fields
        private List<Program> _activePrograms = new();
        private List<LogItem> _systemLog = new();
        private bool _isLocalEndpoint = false;
        private bool _connectedToo = false;
        private EndpointEvents _endpointEvents;
        private DateTime _nextAdminCheckDate;
        private DateTime _nextRestartDate;
        private List<Endpoint> _allowedConnections = new();
        private List<(string, string)> _LoginHistory = new();
        private bool _monitorActive = false;
        private bool _firewallActive = false;
        private List<Endpoint> _watchedEndpoints = new();
        private Dictionary<Person, string> _usernamePasswordDict = new();
        private Dictionary<string, AccessLevel> _usernamePasswordAccesDict = new();
        private AccessLevel _accesLevel = AccessLevel.USER;
        private string _currentUsername = string.Empty;
        private string _currentPassword = string.Empty;
        private IPAddress _iPAddress;
        private Guid _id;
        private bool _softConnection;
        private FileSystem.FileSystem _fileSystem;
        private Endpoint _connectedFrom;
        #endregion

        #region Properties
        public bool IsLocalEndpoint
        {
            get { return _isLocalEndpoint; }
            set { _isLocalEndpoint = value; }
        }
        public bool ConnectedToo
        {
            get { return _connectedToo; }
            set { _connectedToo = value; }
        }
        public EndpointEvents EndpointEvents
        {
            get { return _endpointEvents; }
            set { _endpointEvents = value; }
        }
        public DateTime NextAdminCheckDate
        {
            get { return _nextAdminCheckDate; }
            set { _nextAdminCheckDate = value; }
        }
        public DateTime NextRestartDate
        {
            get { return _nextRestartDate; }
            set { _nextRestartDate = value; }
        }
        public List<Endpoint> AllowedConnections
        {
            get { return _allowedConnections; }
            set { _allowedConnections = value; }
        }
        public List<(string,string)> LoginHistory
        {
            get { return _LoginHistory; }
            set { _LoginHistory = value; }
        }
        public bool MonitorActive
        {
            get { return _monitorActive; }
            set { _monitorActive = value; }
        }
        public bool FirewallActive
        {
            get { return _firewallActive; }
            set { _firewallActive = value; }
        }
        public List<Program> ActivePrograms
        {
            get { return _activePrograms; }
            set { _activePrograms = value; }
        }
        public List<LogItem> SystemLog
        {
            get { return _systemLog; }
            set { _systemLog = value; }
        }
        public List<Endpoint> WatchedEndpoints
        {
            get { return _watchedEndpoints; }
            set { _watchedEndpoints = value; }
        }
        public Dictionary<Person, string> UsernamePasswordDict
        {
            get { return _usernamePasswordDict; }
            set { _usernamePasswordDict = value; }
        }
        public Dictionary<string, AccessLevel> UsernamePasswordAccessDict
        {
            get { return _usernamePasswordAccesDict; }
            set { _usernamePasswordAccesDict = value; }
        }
        public AccessLevel AccessLevel
        {
            get { return _accesLevel; }
            set { _accesLevel = value; }
        }
        public string CurrentUsername
        {
            get { return _currentUsername; }
            set { _currentUsername = value; }
        }
        public string CurrentPassword
        {
            get { return _currentPassword; }
            set { _currentPassword = value; }
        }
        public string IPAddress
        {
            get
            {
                return _iPAddress.ToString();
            }
            protected set
            {
                this._iPAddress = System.Net.IPAddress.Parse(value);
            }
        }
        public Guid Id
        {
            get { return _id; }
            private set { _id = value; }
        }
        public bool SoftConnection
        {
            get { return _softConnection; }
            set { _softConnection = value; }
        }
        public FileSystem.FileSystem FileSystem
        {
            get { return _fileSystem; }
            set { _fileSystem = value; }
        }
        public Endpoint ConnectedFrom
        {
            get { return _connectedFrom; }
            set { _connectedFrom = value; }
        }

        #endregion

        #region Event handlers
        public delegate void EndpointDisconnectedEventHandler(object sender, EndpointDisconnectedEventArgs e);

        public event EndpointDisconnectedEventHandler OnDisconnected;

        public delegate void EndpointConnectedEventHandler(object sender, EndpointConnectedEventArgs e);

        public event EndpointConnectedEventHandler OnConnected;

        public delegate void EndpointLoginEventHandler(object sender, EndpointLoginEventArgs e);

        public event EndpointLoginEventHandler OnLogin;
        #endregion

        /// <summary>
        /// Under dictionary attack, will always tricker a trace track
        /// </summary>
        public void UnderDictHack()
        {
            if (((int)this.Monitor) <= ((int)EndpointMonitor.NONE))
            {
                return;
            }
            Global.ActiveTraceTracker.StartTrace(this.TraceSpeed);
        }

        //TODO: make it so if you spoof the network it will not block you.
        public bool CanBreachFirewall()
        {
            if (!HasFirewall())
                return true;
            if (!FirewallActive)
                return true;
            if (((int)Global.LocalSystem.FirewallBypass) > ((int)this.Firewall)
                && Global.LocalSystem.FirewallBypassActive)
            {
                return true;
            }
            return false;
        }

        public bool HasFirewall()
        {
            if (((int)this.Firewall) > ((int)EndpointFirewall.NONE))
            {
                return true;
            }
            return false;
        }

        private void SetupEndpoint()
        {
            this.Id = Guid.NewGuid();

            if (this.IsLocalEndpoint)
            {
                return;
            }

            //Generate IPAdress with Guid as seed
            var data = new byte[4];
            new Random(Id.GetHashCode()).NextBytes(data);
            data[0] |= 1;
            _iPAddress = new IPAddress(data);
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

                case EndpointType.BANK:
                    this.Name = this.Owner.Name;
                    break;

                case EndpointType.MEDIA:
                    break;

                case EndpointType.DATABASE:
                    this.Name = this.Owner.Name + " Storage Server";
                    this.isHidden = true;
                    break;

                case EndpointType.GOVERMENT:
                    this.Name = this.Owner.Name;
                    break;

                default:
                    break;
            }
            this.EndpointEvents = new EndpointEvents(this);
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
                if (person.Name != user)
                {
                    continue;
                }
                return UsernamePasswordDict[person];
            }
            return string.Empty;
        }

        internal string GetPassword(Person user)
        {
            if (UsernamePasswordDict.TryGetValue(user, out string password))
            {
                return password;
            }
            return string.Empty;
        }

        internal Person GetRandomUser(bool noSystemUsers = true)
        {
            int nrUsers = this.UsernamePasswordDict.Keys.Count;

            Person randomUser = UsernamePasswordDict.Keys.ToList()[Global.Rand.Next(nrUsers)];

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
            foreach (Person person in this.UsernamePasswordDict.Keys)
            {
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
            this.UsernamePasswordDict[person] = password;
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

        internal void AdminSystemCheck()
        {
            this.UsernamePasswordDict.ToList().ForEach(x => LoginIfAdmin(x));

            if (this.IsLocalEndpoint)
            {
                return;
            }

            #region logs

            //Trim Logs to 20 entries.
            List<LogItem> sysLog = this.SystemLog;
            while (sysLog.Count > 20)
            {
                sysLog.RemoveAt(sysLog.Count - 1);
            }
            this.SystemLog = sysLog;

            List<LogItem> conLog = this.SystemLog;
            while (conLog.Count > 20)
            {
                conLog.RemoveAt(conLog.Count - 1);
            }
            this.SystemLog = conLog;

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

            void LoginIfAdmin(KeyValuePair<Person, string> x)
            {
                if (UsernamePasswordAccessDict[x.Key.Name + x.Value] == AccessLevel.ADMIN)
                {
                    this.MockLocalLogInToo(x.Key.Name, x.Value);
                }
            }
        }

        internal void Discconect()
        {
            this.SoftConnection = false;
            Global.RemoteSystem = null;
            LogDisconnected();
            FileSystem.ResetConnection();
            CurrentUsername = string.Empty;
            CurrentPassword = string.Empty;
            this.ConnectedFrom = null;
            EndpointDisconnectedEventArgs args = new EndpointDisconnectedEventArgs("");
            if (OnDisconnected == null)
            {
                return;
            }
            OnDisconnected(this, args);
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
            return this.FileSystem.GetFileFromPath(folderPath, fileName, CurrentUsername);
        }

        internal string TryPrintFile(string command)
        {
            return FileSystem.TryPrintFile(command);
        }

        internal string RunProgram(string path)
        {
            this.LogFileRan(path);
            return FileSystem.TryRunFile(path);
        }

        internal string PrintActivePrograms()
        {
            string result = string.Empty;
            this.ActivePrograms.ForEach(x => result += x.Name);
            return result;
        }

        internal string NavigateTo(string command)
        {
            FileSystem.NavigateTo(command, CurrentUsername);
            return CurrentPath();
        }

        internal string UploadFileToo(string path, Program p, bool log = true)
        {
            string result = this.FileSystem.CopyFileToFonder(path, p, CurrentUsername);
            if (result == "Done" && log && !this.IsLocalEndpoint)
            {
                this.SystemLog.Add(LogItemBuilder
                    .Builder()
                    .FILE_COPIED()
                    .From(this.ConnectedFrom)
                    .User(this.CurrentUsername)
                    .AccesLevel(this.AccessLevel)
                    .TimeStamp(Global.GameTime));
            }
            return result;
        }

        internal string RemoveFileFrom(string path, Program p, bool log = true)
        {
            string result = this.FileSystem.RemoveFileFromFolder(path, p, CurrentUsername);
            if (result == "Done" && log)
            {
                this.SystemLog.Add(LogItemBuilder
                    .Builder()
                    .FILE_DELETED()
                    .From(this.ConnectedFrom)
                    .User(this.CurrentUsername)
                    .AccesLevel(this.AccessLevel)
                    .TimeStamp(Global.GameTime));
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
                if (!fromProgram)
                    throw new Exception("Username password combination not found.");
                return string.Empty;
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
            LogConnectionAttempt(username, from);
            LogConnectionSucces(username, from, this.AccessLevel);
            this.LoginHistory.Add((username, password));
            if (OnLogin != null)
            {
                EndpointLoginEventArgs args = new EndpointLoginEventArgs(from, username, password, this.MemoryHashing);
                OnLogin(this, args);
            }
        }

        /// <summary>
        /// Faking a user logging in physically on this machine.
        /// <param name="username"></param>
        /// <param name="password"></param>
        internal void MockLocalLogInToo(string username, string password)
        {
            LogConnectionSucces(username, null, this.AccessLevel);
            if (OnLogin != null)
            {
                EndpointLoginEventArgs args = new EndpointLoginEventArgs(null, username, password, this.MemoryHashing);
                OnLogin(this, args);
            }
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
        internal void AutoRestart()
        {
            if (this.IsLocalEndpoint)
            {
                return;
            }
            this.Restart();
            this.EndpointEvents.ScheduleNextRestart();
        }

        public void Restart()
        {
            shutdown();
            Task.Factory.StartNew(() =>
            {
                Global.EventTicker.SleepSeconds(120);
                startup();
            });
        }

        private void startup()
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

        private void RunStartupPrograms()
        {
            this.FileSystem.RunStartupPrograms();
        }

        private void shutdown()
        {
            State = EndpointState.SHUTTINGDOWN;
            this.SoftConnection = false;
            FileSystem.ResetConnection();
            CurrentUsername = string.Empty;
            CurrentPassword = string.Empty;
            this.FileSystem.AllFolders.ForEach(folder => folder.Programs.Values.ToList().ForEach(program => program.StopProgram()));

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

    public class EndpointConnectedEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public EndpointConnectedEventArgs(string status)
        {
            Status = status;
        }
    }

    public class EndpointDisconnectedEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public EndpointDisconnectedEventArgs(string status)
        {
            Status = status;
        }
    }

    public class EndpointLoginEventArgs : EventArgs
    {
        public Endpoint From;
        public string Username;
        public string Password;
        public EndpointHashing EndpointHashing;

        public EndpointLoginEventArgs(Endpoint from, string username, string password, EndpointHashing endpointHashing)
        {
            From = from;
            Username = username;
            Password = password;
            EndpointHashing = endpointHashing;
        }
    }
}