using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

//Internal
using Game.Core.Endpoints;
using Core.Events;
using Game;

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
            Global.App = this;
            Global.EventTicker = new EventTicker();
            Global.EndpointGenerator = new EndpointGenerator();
            Global.Console = new UI.Console();
            Global.EndPointMap = new UI.EndpointMap();
            Global.Bounce = new Bounce();

            //TODO move non UI game flow to other class
            base.OnStartup(e);
            Global.EventTicker.StartTicker();
            TestEvent ev = new TestEvent();
            ev.SetStartInterval(4);
            Global.EventTicker.RegisterEvent(ev);



        }

        public void OnMapReady()
        {
            Global.AllEndpoints = Global.EndpointGenerator.GenerateEndpoints(100);
            Global.EndPointMap.DisplayEndpoints();
        }
    }
}
