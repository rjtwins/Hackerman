using System;
using System.Collections.Generic;
using System.Net;

namespace Game.Core.Endpoints
{
    /// <summary>
    /// The endpoint represents a machine that can be connected to.
    /// It has a filesystem to interact with.
    /// </summary>
    public class EndpointBackend
    {
        public bool IsLocalEndpoint { protected set; get; } = false;
        public List<LogItem> SystemLog = new List<LogItem>();
        public List<LogItem> ConnectionLog = new List<LogItem>();

        protected Dictionary<string, AccessLevel> UsernamePasswordDict = new Dictionary<string, AccessLevel>();
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

        //Location and icon data
        private FileSystem.FileSystem FileSystem;

        public Endpoint ConnectedFrom;

        public EndpointBackend()
        {
            this.Id = Guid.NewGuid();

            //Generate IPAdress with Guid as seed
            var data = new byte[4];
            new Random(Id.GetHashCode()).NextBytes(data);
            data[0] |= 1;
            iPAddress = new IPAddress(data);
            this.FileSystem = new FileSystem.FileSystem(this);
        }

        internal void BounceTo(Endpoint from, Endpoint too)
        {
            if(from == null || too == null)
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
                LogType = LogType.CONNECTION_ROUTED
            });
        }

        internal void Discconect()
        {
            LogDisconnected();
            FileSystem.ResetConnection();
            CurrentUsername = string.Empty;
            CurrentPassword = string.Empty;
            this.ConnectedFrom = null;
        }

        private void LogDisconnected()
        {
            this.ConnectionLog.Add(new LogItem
            {
                From = this.ConnectedFrom,
                LogType = LogType.CONNECTION_DISCONNECTED
            });
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

        internal string TryPrintFile(string command)
        {
            return FileSystem.TryPrintFile(command);
        }

        internal string NavigateTo(string command)
        {
            FileSystem.NavigateTo(command);
            return CurrentPath();
        }

        internal string ConnectTo(string username, string password, Endpoint from)
        {
            if (from == null)
            {
                //??? not possible
                throw new Exception("Attempted to connect to endpoint from a null adrress this should not be possible.");
            }

            LoggConnectionAttempt(username, from);

            try
            {
                this.AccessLevel = UsernamePasswordDict[username + password];
                FileSystem.ConnectTo(this.AccessLevel);
                LoggConnectionSucces(username, from, this.AccessLevel);
                CurrentUsername = username;
                CurrentPassword = password;
                this.ConnectedFrom = from;
                return "Logged in as: " + username;
            }
            catch (KeyNotFoundException)
            {
                LoggConnectionFailed(username, from);
                throw new Exception("Username password combination not found.");
            }
        }

        private void LoggConnectionFailed(string username, Endpoint from)
        {
            this.ConnectionLog.Add(new LogItem
            {
                From = from,
                LogType = LogType.CONNECTION_FAILED,
                userName = username
            });
        }

        private void LoggConnectionSucces(string username, Endpoint from, AccessLevel accessLevel)
        {
            this.ConnectionLog.Add(new LogItem
            {
                From = from,
                LogType = LogType.CONNECTION_SUCCES,
                userName = username,
                AccessLevel = accessLevel
            }) ;
        }

        private void LoggConnectionAttempt(string username, Endpoint from)
        {
            this.ConnectionLog.Add(new LogItem
            {
                From = from,
                LogType = LogType.CONNECTION_ATTEMPT,
                userName = username
            });
        }

        public void Restart()
        {

        }
    }
}