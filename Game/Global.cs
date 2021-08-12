using Core.Events;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.UI;
using System.Collections.Generic;

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
    }
}