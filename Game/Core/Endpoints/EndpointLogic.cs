using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static Game.UTILS;

namespace Game.Core.Endpoints
{
    /// <summary>
    /// The endpoint represents a machine that can be connected to.
    /// It has a filesystem to interact with.
    /// </summary>
    public partial class Endpoint
    {
        public bool BounceInstalled = false;
        public int BounceVersion = 0;

        public bool ConnectedToo = false;

        public enum EndpointMonitor { NONE, LVL1, LVL2, LVL3, LVL4 }
        public enum EndpointFirewall { NONE, LVL1, LVL2, LVL3, LVL4 }
        public enum EndpointState { ONLINE, SHUTTINGDOWN, STARTING, CRASHED, DESTROYED1, DESTROYED2, DESTROYED3 };

        public List<Endpoint> AllowedConnections = new();

        public bool MonitorActive = false;
        public bool FirewallActive = false;
        private bool ActiveTrace = false;

        public bool IsLocalEndpoint { protected set; get; } = false;
        public List<LogItem> SystemLog = new();

        public List<LogItem> ConnectionLog
        {
            get
            {
                return FileSystem.GetConnectionLog();
            }
            set
            {
                FileSystem.SetConnectionLog(value);
            }
        }
        public List<Endpoint> WatchedEndpoints = new();

        public Dictionary<Person, string> UsernamePasswordDict = new();
        protected Dictionary<string, AccessLevel> UsernamePasswordAccessDict = new();
        public AccessLevel AccessLevel { protected set; get; } = AccessLevel.USER;
        protected string CurrentUsername;
        protected string CurrentPassword;
        public Guid Id { private set; get; }
        private IPAddress iPAddress;

        public string IPAddress
        {
            get
            {
                return iPAddress.ToString();
            }
            protected set
            {
                this.iPAddress = System.Net.IPAddress.Parse(value);
            }
        }

        private bool SoftConnection { get; set; }

        //Location and icon data
        protected FileSystem.FileSystem FileSystem;
        public Endpoint ConnectedFrom;

        public delegate void EndpointDisconnectedEventHandler(object sender, EndpointDisconnectedEventArgs e);
        public event EndpointDisconnectedEventHandler OnDisconnected;
        public delegate void EndpointConnectedEventHandler(object sender, EndpointConnectedEventArgs e);
        public event EndpointConnectedEventHandler OnConnected;
        public delegate void EndpointLoginEventHandler(object sender, EndpointLoginEventArgs e);
        public event EndpointLoginEventHandler OnLogin;

        public void UnderDictHack()
        {
            if (ActiveTrace)
            {
                return;
            }
            if(((int)this.Monitor) <= ((int)EndpointMonitor.NONE))
            {
                return;
            }
            if(((int)Global.LocalSystem.MonitorBypass) > ((int)this.Monitor) 
                && Global.LocalSystem.MonitorBypassActive)
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
            if(((int)Global.LocalSystem.FirewallBypass) > ((int)this.Firewall) 
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

            //Generate IPAdress with Guid as seed
            var data = new byte[4];
            new Random(Id.GetHashCode()).NextBytes(data);
            data[0] |= 1;
            iPAddress = new IPAddress(data);
            this.FileSystem = new FileSystem.FileSystem(this);

            switch (this.EndpointType)
            {
                case EndpointType.PERSONAL:
                    this.name = this.Owner.Name + "'s Desktop";
                    //TODO: Set back to actual values
                    this.isHidden = true;
                    break;

                case EndpointType.EXTERNALACCES:
                    this.name = this.Owner.Name + " External Acces Server";
                    break;

                case EndpointType.INTERNAL:
                    this.name = this.Owner.Name + " Internal Services";
                    this.isHidden = false;
                    this.MonitorActive = false;
                    this.Monitor = EndpointMonitor.LVL1;
                    break;

                case EndpointType.BANK:
                    this.name = this.Owner.Name;
                    break;

                case EndpointType.MEDIA:
                    break;

                case EndpointType.DATABASE:
                    this.name = this.Owner.Name + " Storage Server";
                    this.isHidden = true;
                    break;

                case EndpointType.GOVERMENT:
                    this.name = this.Owner.Name;
                    break;

                default:
                    break;
            }
        }

        internal bool HasConnection()
        {
            if (this.SoftConnection)
            {
                return true;
            }
            return false;

        }

        private void GenerateAdminUsers(int nr)
        {
            for (int i = 0; i < nr; i++)
            {
                this.AddUser(UTILS.PickRandomPerson(), UTILS.PickRandomPassword(), AccessLevel.ADMIN);
            }
        }
        private void GenerateRootUsers(int nr)
        {
            for (int i = 0; i < nr; i++)
            {
                this.AddUser(UTILS.PickRandomPerson(), UTILS.PickRandomPassword(), AccessLevel.ROOT);
            }
        }
        private void GenerateRandomUsers(int nr)
        {
            for (int i = 0; i < nr; i++)
            {
                this.AddUser(UTILS.PickRandomPerson(), UTILS.PickRandomPassword(), AccessLevel.USER);
            }
        }
        internal string GetPassword(string user)
        {
            foreach (var person in UsernamePasswordDict.Keys)
            {
                if(person.Name != user)
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

            if (nrUsers == 3)
            {
                return null;
            }

            Person randomUser = UsernamePasswordDict.Keys.ToList()[Global.Rand.Next(nrUsers)];

            if (!noSystemUsers)
            {
                return randomUser;
            }

            switch (randomUser.Name)
            {
                case "guest":
                    return GetRandomUser();

                case "admin":
                    return GetRandomUser();

                case "root":
                    return GetRandomUser();

                default:
                    break;
            }

            return randomUser;
        }

        internal string PrintUsers()
        {
            string result = "USER LIST:\n";
            foreach (Person person in this.UsernamePasswordDict.Keys)
            {
                result += person.Name + "\n";
            }
            return result;
        }

        internal void AddEmployes(Endpoint[] employes)
        {
            foreach(Endpoint e in employes)
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

        private void LogConnectionRouted(Endpoint from, Endpoint too)
        {
            this.ConnectionLog.Add(new LogItem
            {
                From = from,
                Too = too,
                LogType = LogType.CONNECTION_ROUTED,
                TimeStamp = Global.GameTime
            });
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
            EndpointDisconnectedEventArgs args = new EndpointDisconnectedEventArgs(null);
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
                this.ConnectionLog.Add(LogItemBuilder
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
                this.ConnectionLog.Add(LogItemBuilder
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
            Debug.WriteLine("Connecting from: " + from.name);
            Debug.WriteLine("AllowedConnections: ");
            foreach (Endpoint e in AllowedConnections)
            {
                Debug.WriteLine(e.name);
            }

            if(AllowedConnections.Count == 0)
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
            EndpointConnectedEventArgs args = new EndpointConnectedEventArgs(null);
            OnConnected(this, args);
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

            Task.Factory.StartNew(() => IncomingConnectionEvent(username, password, from));

            LoggConnectionAttempt(username, from);
            if (UsernamePasswordAccessDict.TryGetValue(username + password, out AccessLevel temp))
            {
                this.AccessLevel = temp;
                FileSystem.ConnectTo(this.AccessLevel, username);
                LoggConnectionSucces(username, from, this.AccessLevel);
                CurrentUsername = username;
                CurrentPassword = password;
                this.ConnectedFrom = from;
                EndpointLoginEventArgs args = new EndpointLoginEventArgs(from, username, password);
                OnLogin(this, args);
                return "Logged in as: " + username;
            }
            else
            {
                LoggConnectionFailed(username, from);
                if (!fromProgram)
                    throw new Exception("Username password combination not found.");
                return string.Empty;
            }
        }

        #region Connection Events

        private void ConnectionFailedEvent(string username, string password, Endpoint from)
        {
            //throw new NotImplementedException();
        }

        private void ConnectionEstablishedEvent(string username, string password, Endpoint from)
        {
            //throw new NotImplementedException();
        }

        private void IncomingConnectionEvent(string username, string password, Endpoint from)
        {

        }

        #endregion Connection Events

        #region Connection logs

        private void LoggConnectionFailed(string username, Endpoint from)
        {
            this.ConnectionLog.Add(LogItemBuilder
                .Builder()
                .CONNECTION_FAILED()
                .From(from)
                .User(username)
                .AccesLevel(AccessLevel.NONE)
                .TimeStamp(Global.GameTime)
                );
        }

        private void LoggConnectionSucces(string username, Endpoint from, AccessLevel accessLevel)
        {
            this.ConnectionLog.Add(LogItemBuilder
                .Builder()
                .CONNECTION_SUCCES()
                .From(from)
                .User(username)
                .AccesLevel(accessLevel)
                .TimeStamp(Global.GameTime)
                );
        }

        private void LoggConnectionAttempt(string username, Endpoint from)
        {
            this.ConnectionLog.Add(LogItemBuilder
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
            this.ConnectionLog.Add(new LogItem
            {
                From = this.ConnectedFrom,
                LogType = LogType.CONNECTION_DISCONNECTED,
                TimeStamp = Global.GameTime
            }); ;
        }

        #endregion Connection logs

        public void Restart()
        {
            shutdown();
            startup();
        }

        private void startup()
        {
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
            if(Global.RemoteSystem == this)
            {
                Global.RemoteConsole.CommandParser.ExitDisconnect();
                Discconect();
            }
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

        public EndpointLoginEventArgs(Endpoint from, string username, string password)
        {
            From = from;
            Username = username;
            Password = password;
        }
    }

}