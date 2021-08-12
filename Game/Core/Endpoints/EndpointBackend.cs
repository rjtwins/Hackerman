using Game.Core.Console;
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

        private Endpoint ConnectedFrom;

        public EndpointBackend()
        {
            this.Id = Guid.NewGuid();

            //Generate IPAdress with Guid as seed
            var data = new byte[4];
            new Random(Id.GetHashCode()).NextBytes(data);
            data[0] |= 1;
            iPAddress = new IPAddress(data);
            this.FileSystem = new FileSystem.FileSystem();
        }

        internal void BounceTo(Endpoint from, Endpoint too)
        {
            LogConnectionRouted(from, too);
        }

        private void LogConnectionRouted(Endpoint from, Endpoint too)
        {
            //throw new NotImplementedException();
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
            //throw new NotImplementedException();
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

            LoggConnectionAttempt(from);

            try
            {
                this.AccessLevel = UsernamePasswordDict[username + password];
                FileSystem.ConnectTo(this.AccessLevel);
                LoggConnectionSucces(username, from);
                CurrentUsername = username;
                CurrentPassword = password;
                this.ConnectedFrom = from;
                return "Logged in as: " + username;
            }
            catch (KeyNotFoundException)
            {
                LoggConnectionFailed(from);
                throw new Exception("Username password combination not found.");
            }
        }

        private void LoggConnectionFailed(Endpoint from)
        {
            //throw new NotImplementedException();
        }

        private void LoggConnectionSucces(string username, Endpoint from)
        {
            //throw new NotImplementedException();
        }

        private void LoggConnectionAttempt(Endpoint from)
        {
            //throw new NotImplementedException();
        }

        public void Restart()
        {
        }
    }
}