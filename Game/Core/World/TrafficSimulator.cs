using Game.Core.Endpoints;
using Game.Core.Events;
using System.Threading.Tasks;

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

            for (int i = 0; i < 5; i++)
            {
                SimulateTraffic();
            }

            EventBuilder.BuildEvent("TrafficSimulatorRun")
                .EventInterval(600)
                .EventVoid(this.Simulating)
                .RegisterWithVoid();
        }

        private void SimulateTraffic()
        {
            double roll = Global.Rand.NextDouble();

            if (roll < 0.33)
            {
                SimulateRemoteCompanyLogin();
            }
            else if (roll > 0.33 && roll < 0.66)
            {
                SimulateLocalCompanyLogin();
            }
            else if (roll > 0.66)
            {
                SimulatePersonalLogin();
            }
        }

        private void SimulateRemoteCompanyLogin()
        {
            //TODO filter pick on endpoint type.
            Endpoint A = UTILS.PickRandomCompanyEdnpoint();
            (bool isGuestUser, Endpoint B) = UTILS.PickRandomEndpointWithAccess(A);

            string password = "guest";
            string userName = "guest";
            if (!isGuestUser)
            {
                password = B.Owner.WorkPassword;
                userName = B.Owner.Name;
            }

            if (userName != null && password != string.Empty)
            {
                A.MockRemoteLogInToo(userName, password, B);
            }
        }

        private void SimulateLocalCompanyLogin()
        {
            Endpoint A = UTILS.PickRandomCompanyEdnpoint();
            (bool isGuestUser, Endpoint B) = UTILS.PickRandomEndpointWithAccess(A);

            string password = "guest";
            string userName = "guest";
            if (!isGuestUser)
            {
                password = B.Owner.WorkPassword;
                userName = B.Owner.Name;
            }

            if (userName != null && password != string.Empty)
            {
                A.MockLocalLogInToo(userName, password);
            }
        }

        private void SimulatePersonalLogin()
        {
            Endpoint A = UTILS.PickRandomEmployeEndpoint();
            (bool isGuestUser, Endpoint B) = UTILS.PickRandomEndpointWithAccess(A);

            string password = "guest";
            string userName = "guest";
            if (!isGuestUser)
            {
                password = B.Owner.WorkPassword;
                userName = B.Owner.Name;
            }

            if (userName != null && password != string.Empty)
            {
                A.MockLocalLogInToo(userName, password);
            }
        }
    }
}