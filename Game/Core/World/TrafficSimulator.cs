using Game.Core.Endpoints;
using Game.Core.Events;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

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
            Task.Factory.StartNew(() => { Simulating(new object[] { }); });
        }

        public void Stop()
        {
            stop = true;
        }

        public static void Simulating(object[] args)
        {
            if (TrafficSimulator.Instance.stop)
            {
                return;
            }
            while (Global.GamePaused)
            {
                System.Threading.Thread.Sleep(100);
            }

            //Number of traffics to simulate each run
            TrafficSimulator.Instance.SimulateTraffic();

            EventBuilder.BuildEvent("TrafficSimulatorRun")
                .EventInterval(200)
                .EventAction(TrafficSimulator.Simulating, new object[] { })
                .RegisterWithAction();
        }

        private void SimulateTraffic()
        {
            double roll = Global.Rand.NextDouble();

            if (roll < 0.20)
            {
                //A employe logs in from remote.
                SimulateRemoteCompanyEmployeLogin();
            }
            else if (roll > 0.20 && roll < 0.40)
            {
                //A employe logs in on site.
                SimulateLocalCompanyLogin();
            }
            else if (roll > 0.40 && roll < 0.60)
            {
                //A random perosonal endpoint logs in as guest on a compnay external acces server.
                SimulateExternalCompnayLogin();
            }
            else if (roll > 0.60 && roll < 0.80)
            {
                //Sombody logs in on their own machine.
                SimulatePersonalLocalLogin();
            }
            else
            {
                //Simulate people sending money to other people
                SimulateBankClientTraffic();
            }

            //People going to web servers.
            SimulateWebTraffic();
            
        }

        private void SimulateWebTraffic()
        {
            Endpoint A = UTILS.PickRandomPersonWithEndpoint().PersonalComputer;
            WebServerEndpoint B = UTILS.PickRandomWebServerEndpoint();
            B.VisitedBy(A);
        }

        private void SimulateExternalCompnayLogin()
        {
            Endpoint A = UTILS.PickRandomPersonWithEndpoint().PersonalComputer;
            Endpoint B = UTILS.PickRandomCompanyEdnpoint();
            B.MockRemoteLogInToo("guest", B.GetPassword("guest"), A);
            B.LoggedInTo("guest", B.GetPassword("guest"), B);
        }

        private void SimulateBankClientTraffic()
        {
            BankEndpoint A = UTILS.PickRandomBankEndpoint();
            Model.Person randomClietA = A.Clients.ToArray()[Global.Rand.Next(A.Clients.Count)];
            int randomAmount = Global.Rand.Next(randomClietA.BankBalance);

            BankEndpoint B = UTILS.PickRandomBankEndpoint();
            var bClients = B.Clients.ToArray();
            Model.Person randomClietB = null;
            do
            {
                randomClietB = bClients[Global.Rand.Next(bClients.Length)];
            } while (randomClietB == Global.LocalEndpoint.Owner);
            
            //We don't really care if it works(tough it will)
            if(!A.TransferMoney(randomClietA.Name, randomClietB.Name, B, randomAmount, out string result))
            {
                return;
            }
        }

        private void SimulateRemoteCompanyEmployeLogin()
        {
            //TODO filter pick on endpoint type.
            Endpoint A = UTILS.PickRandomCompanyEdnpoint();
            (string username, string password, Endpoint from) = UTILS.PickRandomPossibleLogin(A);

            if (username != null && !string.IsNullOrEmpty(password))
            {
                A.MockRemoteLogInToo(username, password, from);
                from.LoggedInTo(username, password, A);
            }
            else
            {
                Debug.WriteLine($"SimulateRemoteCompanyEmployeLogin Username {username} and password {password} has either a null username or null password.");
            }
        }

        private void SimulateLocalCompanyLogin()
        {
            Endpoint A = UTILS.PickRandomCompanyEdnpoint();
            (string username, string password) = UTILS.PickRandomPossibleLocalLogin(A);

            
            if (username != null && !string.IsNullOrEmpty(password))
            {
                A.MockLocalLogInToo(username, password);
            }
            else
            {
                Debug.WriteLine($"SimulateLocalCompanyLogin Username {username} and password {password} has either a null username or null password.");
            }
        }

        private void SimulatePersonalLocalLogin()
        {
            Endpoint A = UTILS.PickRandomEmployeEndpoint();
            (string username, string password) = UTILS.PickRandomPossibleLocalLogin(A);

            if (username != null && !string.IsNullOrEmpty(password))
            {
                A.MockLocalLogInToo(username, password);
            }
            else
            {
                Debug.WriteLine($"SimulatePersonalLocalLogin Username {username} and password {password} has either a null username or null password.");
            }
        }
    }
}