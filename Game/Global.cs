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
        public static App App;
        public static EndpointMap EndPointMap;
        public static MainWindow MainWindow;
        public static UI.Console Console;
        public static EndpointGenerator EndpointGenerator;
        public static EventTicker EventTicker;
        public static Bounce Bounce;
        public static Endpoint StartEndPoint;
        public static List<Endpoint> AllEndpoints = new();
        public static LocalSystem LocalSystem;
        public static DateTime GameTime;
        public static bool GamePaused;
        public static ActiveTraceTracker ActiveTraceTracker;
    }
}