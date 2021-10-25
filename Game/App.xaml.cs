//Internal
using Core.Events;
using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.Events;
using Game.Core.Mission;
using Game.Core.World;
using Game.Model;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using Game.Properties;

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

            MissionDictionaries.ParseFromFiles();

            StartNew();

            //LoadFromSave();
        }

        private void LoadFromSave()
        {
            UTILS.LoadExternalLists(true);

            Global.SplashPage = new UI.SplashPage();
            Global.SplashPage2 = new UI.SplashPage2();

            Global.LocalConsole = new UI.LocalConsole();
            Global.RemoteConsole = new UI.RemoteConsole();

            SaveLoad.Instance.Load();

            Global.EventTicker = new GameTicker();
            Global.EndpointGenerator = new WorldGenerator();

            Global.EndPointMap = new UI.Pages.EndpointMap();
            Global.IRCWindow = new UI.Pages.IRC();
            Global.SystemTime = new UI.Pages.SystemTime();

            Global.Bounce = new BouncePathManager();

            Global.MainWindow = new UI.MainWindow();
            MainWindow.Show();

            //Global.EndpointGenerator.GenerateEndpoints();
            TrafficSimulator.Instance.Start();

            //For testing
            ((BankEndpoint)Global.BankEndpoints[0]).Clients.Add(Global.LocalPerson);


            //MissionManager.Instance.EvaluateMissionListings();

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
        }

        private void StartNew()
        {
            UTILS.LoadExternalLists();

            GameState.Instance.SetUserName("RJ");
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
            Global.IRCWindow = new UI.Pages.IRC();
            Global.SystemTime = new UI.Pages.SystemTime();

            Global.Bounce = new BouncePathManager();

            Global.MainWindow = new UI.MainWindow();
            MainWindow.Show();

            Global.ByteMap = UTILS.getBoolBitmap(Game.Properties.Resources.WorldMapDensity);
            Global.EndpointGenerator.GenerateEndpoints();
            Global.EndpointGenerator.GenerateStartEndpoint();

            Global.AllEndpointsDict.Values.ToList().ForEach(x => Global.AllEndpoints.Add(x));
            TrafficSimulator.Instance.Start();

            //For testing
            ((BankEndpoint)Global.BankEndpoints[0]).Clients.Add(Global.LocalPerson);

            MissionManager.Instance.EvaluateMissionListings();

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

            SaveLoad.Instance.Save();
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
    }
}