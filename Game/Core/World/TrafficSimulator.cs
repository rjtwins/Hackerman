using Game.Core.Endpoints;
using Game.Core.Events;
using System.Diagnostics;
using System.Threading.Tasks;
using static Game.UTILS;

namespace Game.Core.World
{
    internal class TrafficSimulator
    {
        private static readonly TrafficSimulator instance = new TrafficSimulator();
        public static TrafficSimulator Instance
        {
            get
            {
                return instance;
            }
            set
            {
            }
        }

        private bool stop = false;

        private TrafficSimulator()
        {
            // just to make it private
        }

        public void Start()
        {
            stop = false;
            //Start the simulator on another thread.
            Task.Factory.StartNew(() => { Simulating(); });
        }

        public void Stop()
        {
            stop = true;
        }

        private void Simulating()
        {
            if (stop)
            {
                return;
            }
            while (Global.GamePaused)
            {
                System.Threading.Thread.Sleep(100);
            }

            //TODO filter pick on endpoint type.
            Endpoint A = UTILS.PickRandomCompanyEdnpoint();
            Endpoint B = UTILS.PickRandomEndpointWithAccess(A);

            Person person = B.Owner;
            string password = B.Owner.WorkPassword;

            if (person != null && password != string.Empty)
            {
                Debug.WriteLine("Simulated Traffic Between:");
                Debug.WriteLine("--From " + A.IPAddress);
                Debug.WriteLine("--Too " + B.IPAddress);
                A.LogInToo(person.Name, password, B, true);
                A.Discconect();
            }

            EventBuilder.BuildEvent("TrafficSimulatorRun")
                .EventInterval(600)
                .EventVoid(this.Simulating)
                .RegisterWithVoid();
        }
    }
}