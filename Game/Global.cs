using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Dialog;
using Game.Core.Endpoints;
using Game.Core.Mission;
using Game.Core.Mission.MissionTypes;
using Game.Core.World;
using Game.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

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
        public static UI.RemoteConsole RemoteConsole;
        public static LocalConsole LocalConsole;
        public static IRC IRCWindow;

        public static MissionManager MissionManager;

        public static EndpointGenerator EndpointGenerator;
        public static GameTicker EventTicker;
        public static Bounce Bounce;

        public static List<Endpoint> AllEndpoints = new();
        public static List<Endpoint> CompanyEndpoints = new();
        public static List<Endpoint> EmployeEndpoints = new();
        public static List<Endpoint> PersonalEndpoints = new();
        public static LocalSystem LocalSystem;
        public static Endpoint StartEndPoint;

        private static Endpoint remoteSystem;
        public static Endpoint RemoteSystem
        {
            get {
                return remoteSystem; 
            }
            set {
                remoteSystem = value; 
            }
        }

        public static DateTime GameTime;
        public static bool GamePaused;

        public static ActiveTraceTracker ActiveTraceTracker;
        public static PassiveTraceTracker PassiveTraceTracker;

        public static bool StopCurrentProgram { get; internal set; }
    }

    public enum EndpointType
    {
        PLAYER,
        PERSONAL,
        EXTERNALACCES,
        INTERNAL,
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

    public class IRCChannel
    {
        public IRCChannel(string channelName, GenericMissionDialogResolver dialogResolver)
        {
            this.Messages = new List<StackPanel>();
            this.ChannelName = channelName;
            this.ChannelNameTextBlock = new TextBlock();
            this.ChannelNameTextBlock.Text = channelName;
            this.ChannelNameTextBlock.Foreground = Brushes.White;
            this.ChannelNameTextBlock.Background = Brushes.Black;
            this.DialogResolver = dialogResolver;
        }

        public IRCChannel(string channelName)
        {
            this.Messages = new List<StackPanel>();
            this.ChannelName = channelName;
            this.ChannelNameTextBlock = new TextBlock();
            this.ChannelNameTextBlock.Text = channelName;
            this.ChannelNameTextBlock.Foreground = Brushes.White;
            this.ChannelNameTextBlock.Background = Brushes.Black;
        }

        public string ChannelName;
        public List<StackPanel> Messages;
        public TextBlock ChannelNameTextBlock;
        public MissionTemplate Mission;
        public GenericMissionDialogResolver DialogResolver { get; set; }
        public Dictionary<string, System.Drawing.Color> SenderColorDict = new();
    }
}