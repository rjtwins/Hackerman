using Game.Core.FileSystem;
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

        public Dictionary<string, string> UsernamePasswordDict = new();
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

        //Location and icon data
        protected FileSystem.FileSystem FileSystem;

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

        public void AddUser(string userName, string password, AccessLevel accessLevel)
        {
            this.UsernamePasswordDict[userName] = password;
            this.UsernamePasswordAccessDict[userName + password] = accessLevel;
            Folder newFolder = FileSystem.MakeFolderFromPath(@"root\users\" + userName);
            if(newFolder != null)
            {
                newFolder.AccessLevel = accessLevel;
                newFolder.Owner = userName;
            }
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
                LogType = LogType.CONNECTION_ROUTED,
                TimeStamp = Global.GameTime
            });
        }

        internal void Discconect()
        {
            Global.RemoteSystem = null;
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
            if(result == "Done" && log)
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
                this.AccessLevel = UsernamePasswordAccessDict[username + password];
                FileSystem.ConnectTo(this.AccessLevel, username);
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

        public void Restart()
        {

        }
    }
}