using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.FileSystem;
using Game.Core.Mission;
using Game.Core.Mission.MissionTypes;
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
        public static bool CanSave
        {
            get
            {
                if (
                    ActiveTraceTracker.Instance.Tracing ||
                    Global.RemoteSystem != null
                    )
                {
                    return false;
                }
                return true;
            }
            private set
            {

            }
        }

        //Dicts for reference
        public static Dictionary<Guid, MissionTemplate> AllMissionsDict { get; set; } = new();
        public static Dictionary<Guid, IRCChannel> AllIRCChannelsDict { get; set; } = new();
        public static Dictionary<Guid, Person> AllPersonsDict { get; set; } = new();
        public static Dictionary<Guid, Endpoint> AllEndpointsDict { get; set; } = new();
        public static Dictionary<Guid, Folder> AllFoldersDict { get; internal set; } = new();
        public static Dictionary<Guid, Program> AllProgramsDict { get; set; } = new();

        //Map:
        public static byte[,] ByteMap { get; set; }


        //TODO revert to singelton and public statics
        public static Random Rand = new Random();

        public static App App;

        public static SplashPage SplashPage;
        public static SplashPage2 SplashPage2;

        public static EndpointMap EndPointMap;
        public static MainWindow MainWindow;
        public static RemoteConsole RemoteConsole;
        public static LocalConsole LocalConsole;
        public static IRC IRCWindow;
        public static SystemTime SystemTime;

        public static WorldGenerator EndpointGenerator;
        public static GameTicker EventTicker;
        public static BouncePathManager Bounce;

        public static ReferenceList<Endpoint> AllEndpoints { get; set; } = new(AllEndpointsDict, "AllEndpointsDict");
        public static ReferenceList<Endpoint> CompanyEndpoints { get; set; } = new(AllEndpointsDict, "AllEndpointsDict");
        public static ReferenceList<Endpoint> EmployeEndpoints { get; set; } = new(AllEndpointsDict, "AllEndpointsDict");
        public static ReferenceList<Endpoint> PersonalEndpoints { get; set; } = new(AllEndpointsDict, "AllEndpointsDict");
        public static ReferenceList<Endpoint> WebServerEndpoints { get; set; } = new(AllEndpointsDict, "AllEndpointsDict");
        public static ReferenceList<Endpoint> BankEndpoints { get; set; } = new(AllEndpointsDict, "AllEndpointsDict");

        private static Guid localPerson;
        public static Person LocalPerson
        {
            get
            {
                return Global.AllPersonsDict[localPerson];
            }
            set
            {
                localPerson = value.Id;
            }
        }
        public static LocalEndpoint LocalEndpoint { get; set; }

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

        public static bool StopCurrentProgram { get; internal set; }
        public static NotifyingList<Endpoint> BounceNetwork { get; set; } = new();
        public static bool SerializingDictionaries { get; internal set; }
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