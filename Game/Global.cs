using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.Mission;
using Game.Core.World;
using Game.Model;
using Game.UI;
using Game.UI.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Game
{
    public static class Global
    {
        //TODO revert to singelton and public statics
        public static Random Rand = new Random();

        public static App App;
        public static GameState GameState;

        public static SplashPage SplashPage;
        public static SplashPage2 SplashPage2;

        public static EndpointMap EndPointMap;
        public static MainWindow MainWindow;
        public static RemoteConsole RemoteConsole;
        public static LocalConsole LocalConsole;
        public static IRC IRCWindow;
        public static SystemTime SystemTime;

        public static MissionManager MissionManager;

        public static WorldGenerator EndpointGenerator;
        public static GameTicker EventTicker;
        public static BouncePathManager Bounce;

        public static List<Endpoint> AllEndpoints = new();
        public static List<Endpoint> CompanyEndpoints = new();
        public static List<Endpoint> EmployeEndpoints = new();
        public static List<Endpoint> PersonalEndpoints = new();
        public static List<WebServerEndpoint> WebServerEndpoints = new();
        public static List<BankEndpoint> BankEndpoints { get; set; } = new();

        public static Person LocalPerson;
        public static LocalSystem LocalSystem;
        public static LocalEndpoint LocalEndpoint;
        private static Endpoint _remoteSystem;
        public static Endpoint RemoteSystem
        {
            get
            {
                return _remoteSystem;
            }
            set
            {
                _remoteSystem = value;
            }
        }


        public static DateTime GameTime;
        public static bool GamePaused;

        public static ActiveTraceTracker ActiveTraceTracker;
        public static PassiveTraceTracker PassiveTraceTracker;

        public static bool StopCurrentProgram { get; internal set; }
        public static NotifyingList<Endpoint> BounceNetwork { get; set; } = new();
    }

    public enum EndpointState { ONLINE, SHUTTINGDOWN, STARTING, CRASHED, DESTROYED1, DESTROYED2, DESTROYED3 };

    public enum EndpointHashing { NONE, LVL1, LVL2, LVL3, LVL4 };
    public enum SoftwareLevel
    {
        LVL0,
        LVL1,
        LVL2,
        LVL3,
        LVL4
    }

    public enum EndpointType
    {
        PLAYER,
        PERSONAL,
        EXTERNALACCES,
        INTERNAL,
        WEB,
        BANK,
        MEDIA,
        DATABASE,
        GOVERMENT
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
        NONE,
        USER,
        ADMIN,
        ROOT
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
        CONNECTION_DISCONNECTED = 9,
        CREDITTRANSFER = 10
    }

    public enum EndpointDifficulty { LVL0, LVL1, LVL2, LVL3, LVL4, LVL5, LVL6, LVL7, LVL8, LVL9, LVL10 }

}