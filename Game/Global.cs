using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Game
{
    public static class Global
    {
        public static string CurrentUserName = "RJ";

        public static App App;

        public static EndpointMap EndPointMap;
        public static MainWindow MainWindow;
        public static UI.RemoteConsole RemoteConsole;
        public static LocalConsole LocalConsole;
        public static IRC IRCWindow;

        public static EndpointGenerator EndpointGenerator;
        public static EventTicker EventTicker;
        public static Bounce Bounce;
        public static Endpoint StartEndPoint;
        public static List<Endpoint> AllEndpoints = new();
        public static LocalSystem LocalSystem;
        public static Endpoint RemoteSystem;
        public static DateTime GameTime;
        public static bool GamePaused;
        public static ActiveTraceTracker ActiveTraceTracker;
        public static PassiveTraceTracker PassiveTraceTracker;

        
    }

    public enum MissionType
    {
        STEAL,
        STEALMULTIPLE,
        STEALALL,
        STEALCRID,
        MOVECOIN,
        DELETE,
        DELETEMULTIPLE,
        DELETEALL,
        UPLOAD,
        UPLOADMULTIPLE,
        UPLOADRUN,
        DESTROY,
        FRAME,
        TRACE,
        CHANGEINFO
    }

    public enum AccessLevel
    {
        NONE = 0,
        USER = 1,
        ADMIN = 2,
        ROOT = 3
    }

    public enum LogType
    {
        CONNECTION_ATTEMPT = 1,
        CONNECTION_FAILED = 2,
        CONNECTION_SUCCES = 3,
        FILE_EDITED = 4,
        FILE_COPIED = 5,
        FILE_DELETED = 6,
        FILE_RUN = 7,
        CONNECTION_ROUTED = 8,
        CONNECTION_DISCONNECTED = 9
    }
}