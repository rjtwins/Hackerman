//Internal
using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.Events;
using Game.Core.Mission;
using Game.Core.World;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Game
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private bool SplashFinished = false;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //declaring globals
            UTILS.LoadExternalLists();
            MissionDictionaries.ParseFromFiles();

            Global.GameState = new GameState();
            Global.GameState.SetUserName("RJ");
            Global.App = this;
            Global.SplashPage = new UI.SplashPage();
            Global.SplashPage2 = new UI.SplashPage2();

            Global.MainWindow = new UI.MainWindow();

            MainWindow.Show();

            Global.EventTicker = new GameTicker();
            Global.EndpointGenerator = new EndpointGenerator();

            Global.RemoteConsole = new UI.RemoteConsole();
            Global.LocalConsole = new UI.LocalConsole();
            Global.EndPointMap = new UI.EndpointMap();
            Global.IRCWindow = new UI.IRC();

            Global.Bounce = new Bounce();
            Global.LocalSystem = new LocalSystem();
            Global.ActiveTraceTracker = new Core.World.ActiveTraceTracker();
            Global.PassiveTraceTracker = new Core.World.PassiveTraceTracker();

            Global.AllEndpoints = Global.EndpointGenerator.GenerateEndpoints();
            TrafficSimulator.Instance.Start();

            Global.MissionManager = new MissionManager();
            Global.MissionManager.EvaluateMissionListings();


            //TODO move non UI game flow to other class
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

            Task.Factory.StartNew(() => 
            {
                while (!SplashFinished)
                {
                    System.Threading.Thread.Sleep(250);
                }
                Global.App.Dispatcher.Invoke(() => { Global.MainWindow.ShowGameScreen(); });
                
            });
        }

        internal void FinshedPlaySetup()
        {
            SplashFinished = true;
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
            Global.EndPointMap.DisplayEndpoints();
        }

        private void MaxButton_Selected(object sender, RoutedEventArgs e)
        {
        }
    }
}