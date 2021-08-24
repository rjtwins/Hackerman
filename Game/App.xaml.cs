//Internal
using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.Events;
using Game.Core.Mission;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Game
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //declaring globals
            UTILS.LoadPasswordAndUsernameFile();
            Global.GameState = new GameState();
            Global.GameState.SetUserName("RJ");
            Global.App = this;
            Global.EventTicker = new EventTicker();
            Global.EndpointGenerator = new EndpointGenerator();

            Global.RemoteConsole = new UI.RemoteConsole();
            Global.LocalConsole = new UI.LocalConsole();
            Global.EndPointMap = new UI.EndpointMap();
            Global.IRCWindow = new UI.IRC();

            Global.Bounce = new Bounce();
            Global.LocalSystem = new LocalSystem();
            Global.ActiveTraceTracker = new Core.ActiveTraceTracker();
            Global.PassiveTraceTracker = new Core.PassiveTraceTracker();

            //TODO move non UI game flow to other class
            base.OnStartup(e);
            Global.EventTicker.StartTicker();

            EventBuilder.BuildEvent("TESTEVENT0")
                .EventInterval(600d)
                .EventVoid(PRINTDEBUG)
                .RegisterWithVoid();

            EventBuilder.BuildEvent("TESTEVENT1")
                .EventInterval(800d)
                .EventVoid(PRINTDEBUG)
                .RegisterWithVoid();

            EventBuilder.BuildEvent("TESTEVENT2")
                .EventInterval(1000d)
                .EventVoid(PRINTDEBUG)
                .RegisterWithVoid();

            EventBuilder.BuildEvent("TESTEVENT3")
                .EventInterval(1200d)
                .EventVoid(PRINTDEBUG)
                .RegisterWithVoid();

            EventBuilder.BuildEvent("TESTEVENT4")
                .EventInterval(1400d)
                .EventVoid(PRINTDEBUG)
                .RegisterWithVoid();
        }

        public void PRINTDEBUG()
        {
            Debug.WriteLine("TESTEVENT");
        }

        private void StartupHandler(object sender, System.Windows.StartupEventArgs e)
        {
            //Elysium.Manager.Apply(this, Elysium.Theme.Dark);
        }

        public void OnMapReady()
        {
            Global.AllEndpoints = Global.EndpointGenerator.GenerateEndpoints(100);
            Global.EndPointMap.DisplayEndpoints();

            MissionDictionaries.ParseFromFiles();
            Global.MissionManager = new MissionManager();
            Global.MissionManager.EvaluateMissionListings();
        }
    }
}