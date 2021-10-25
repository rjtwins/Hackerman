using Game.Core.FileSystem;
using Newtonsoft.Json;
using System;

namespace Game.Core.Endpoints
{
    public partial class Endpoint
    {
        #region Event handlers
        public delegate void EndpointFileRunEventHandler(object sender, EndpointFileOperationEventArgs e);

        public event EndpointFileRunEventHandler OnFileRun;

        public delegate void EndpointFileAddEventHandler(object sender, EndpointFileOperationEventArgs e);
        
        public event EndpointFileAddEventHandler OnFileAdd;

        public delegate void EndpointFileRemoveEventHandler(object sender, EndpointFileOperationEventArgs e);

        public event EndpointFileRemoveEventHandler OnFileRemove;

        public delegate void EndpointFileGetEventHandler(object sender, EndpointFileOperationEventArgs e);

        public event EndpointFileGetEventHandler OnFileGet;

        public delegate void EndpointStartupEventHandler(object sender);

        public event EndpointStartupEventHandler OnStartup;

        public delegate void EndpointShutdownEventHandler(object sender);

        public event EndpointShutdownEventHandler OnShutdown;

        public delegate void EndpointDisconnectedEventHandler(object sender, EndpointDisconnectedEventArgs e);

        public event EndpointDisconnectedEventHandler OnDisconnected;

        public delegate void EndpointConnectedEventHandler(object sender, EndpointConnectedEventArgs e);

        public event EndpointConnectedEventHandler OnConnected;

        public delegate void EndpointLoginEventHandler(object sender, EndpointLoginEventArgs e);

        /// <summary>
        /// Fired when this endpoint is logged into by another endpoint
        /// </summary>
        public event EndpointLoginEventHandler OnLogin;

        public delegate void EndpointFailedLoginEventHandler(object sender, EndpointLoginEventArgs e);

        /// <summary>
        /// Fired when this endpoint is logged into by another endpoint
        /// </summary>
        public event EndpointFailedLoginEventHandler OnFailedLogin;

        public delegate void EndpointLoggedInEventHandler(object sender, EndpointLoggedInEventArgs e);

        /// <summary>
        /// Fired when this enpoint logges into another endpoint
        /// </summary>
        public event EndpointLoggedInEventHandler OnLoggedIn;
        #endregion
    }

    public class EndpointFileOperationEventArgs : EventArgs
    {
        public Endpoint From { get; private set; }
        public Program File { get; private set; }

        public EndpointFileOperationEventArgs(Endpoint from, Program file)
        {
            From = from;
            this.File = file;
        }
    }

    public class EndpointConnectedEventArgs : EventArgs
    {
        public Endpoint From { get; private set; }

        public EndpointConnectedEventArgs(Endpoint from)
        {
            From = from;
        }
    }

    public class EndpointDisconnectedEventArgs : EventArgs
    {
        public Endpoint From { get; private set; }

        public EndpointDisconnectedEventArgs(Endpoint from)
        {
            this.From = from;
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

    public class EndpointLoggedInEventArgs : EventArgs
    {
        public Endpoint Too;
        public string Username;
        public string Password;
        public EndpointHashing EndpointHashing;

        public EndpointLoggedInEventArgs(Endpoint too, string username, string password, EndpointHashing endpointHashing)
        {
            Too = too;
            Username = username;
            Password = password;
            EndpointHashing = endpointHashing;
        }
    }
}