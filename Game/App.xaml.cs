//Internal
using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.Events;
using Game.Core.Mission;
using Game.Core.World;
using Game.Model;
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
            Global.App = this;

            Global.LocalSystem = new LocalSystem();

            UTILS.LoadExternalLists();
            MissionDictionaries.ParseFromFiles();

            Global.GameState = new GameState();
            Global.GameState.SetUserName("RJ");
            Global.LocalPerson = new Person() // for testing
            {
                Username = "RJ",
                BankBalance = 1234567890,
                Name = "RJName",
                BankPassword = "1234",
                WorkPassword = "1234",
                PersonalPassword = "1234",
            };
            Global.SplashPage = new UI.SplashPage();
            Global.SplashPage2 = new UI.SplashPage2();

            Global.EventTicker = new GameTicker();
            Global.EndpointGenerator = new WorldGenerator();
            Global.LocalConsole = new UI.LocalConsole();

            Global.RemoteConsole = new UI.RemoteConsole();
            Global.EndPointMap = new UI.Pages.EndpointMap();
            Global.IRCWindow = new UI.IRC();

            Global.Bounce = new Bounce();

            Global.MainWindow = new UI.MainWindow();
            MainWindow.Show();

            Global.ActiveTraceTracker = new Core.World.ActiveTraceTracker();
            Global.PassiveTraceTracker = new Core.World.PassiveTraceTracker();

            Global.AllEndpoints = Global.EndpointGenerator.GenerateEndpoints();
            TrafficSimulator.Instance.Start();

            Global.BankEndpoints[0].Clients.Add(Global.LocalPerson);

            Global.MissionManager = new MissionManager();
            Global.MissionManager.EvaluateMissionListings();

            //TODO move non UI game flow to other class
            Global.EventTicker.StartUpTicker();

            Task.Factory.StartNew(() =>
            {
                while (!SplashFinished)
                {
                    System.Threading.Thread.Sleep(250);
                }
                Global.App.Dispatcher.Invoke(() => { Global.MainWindow.ShowGameScreen(); });
            });

            UTILS.PlayBoob();
            UTILS.PlayBoob();
            UTILS.PlayBoob();
            UTILS.PlayBoob();
            UTILS.PlayBoob();
        }

        internal void FinshedPlaySetup()
        {
            SplashFinished = true;
        }

        public void PRINTDEBUG()
        {
            Debug.WriteLine("TESTEVENT");
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