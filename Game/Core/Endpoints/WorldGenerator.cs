using Game.Model;
using Game.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using static Game.Core.Endpoints.Endpoint;

namespace Game.Core.Endpoints
{
    public class WorldGenerator
    {
        public static int XMax = 4984;
        public static int YMax = 2576;
        public static int EndpointBaseSize = 6;
        private double[,] EndpointCoordinates;
        private Random Rand = new Random();

        /// <summary>
        /// Add endpoint at cooridinates on overmap.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Endpoint MakeEndpoint(int x, int y, Person Person, EndpointType endpointType)
        {
            Endpoint e = new Endpoint(Person, endpointType);

            e.x = x;
            e.y = y;
            return e;
        }

        public Endpoint GenerateStartEndpoint()
        {
            LocalEndpoint e = new LocalEndpoint();
            (e.x, e.y) = GenerateCoordinate();
            e.Name = "LOCAL";
            Global.LocalEndpoint = e;
            return e;
        }

        /// <summary>
        /// Add Endpoint with random coordinates.
        /// </summary>
        /// <returns></returns>
        public Endpoint MakeEndpoint(Person Person, EndpointType endpointType)
        {
            Endpoint e = new Endpoint(Person, endpointType);
            (e.x, e.y) = GenerateCoordinate();
            return e;
        }

        public List<Endpoint> GenerateEndpoints()
        {
            this.EndpointCoordinates = UTILS.getBoolBitmap(Resources.WorldMapDensity);

            List<Endpoint> EndpointList = new List<Endpoint>();
            List<Endpoint> AvailableEmployes = new();

            //Generate 20 random companies machines:
            for (int i = 0; i < 10; i++)
            {
                GenerateCompanyEndpoint(10, EndpointDifficulty.LVL0, EndpointList, AvailableEmployes);
            }
            for (int i = 0; i < 10; i++)
            {
                GenerateCompanyEndpoint(10, EndpointDifficulty.LVL1, EndpointList, AvailableEmployes);
            }
            for (int i = 0; i < 10; i++)
            {
                GenerateCompanyEndpoint(10, EndpointDifficulty.LVL2, EndpointList, AvailableEmployes);
            }
            for (int i = 0; i < 10; i++)
            {
                GenerateCompanyEndpoint(10, EndpointDifficulty.LVL3, EndpointList, AvailableEmployes);
            }
            //for (int i = 0; i < 10; i++)
            //{
            //    GenerateCompanyEndpoint(10, EndpointDifficulty.LVL4, EndpointList, AvailableEmployes);
            //}


            //Generate 5 bank machines:
            for (int i = 0; i < 5; i++)
            {
                GeneatePersonalEndpoint(EndpointList, AvailableEmployes);
                Endpoint[] employes = PickRandomEmploye(10, AvailableEmployes);
                Endpoint Admin = PickRandomEmploye(1, AvailableEmployes)[0];

                Person Person = UTILS.PickRandomBank();
                BankEndpoint e = new BankEndpoint(Person, EndpointType.BANK);
                e.AddEmployes(employes);
                e.AddUser(Admin.Owner, Admin.Owner.WorkPassword, AccessLevel.ADMIN);
                (e.x, e.y) = GenerateCoordinate();
                e.isHidden = false;
                Global.BankEndpoints.Add(e);
                EndpointList.Add(e);
            }

            //Populate banks with users
            foreach (Endpoint e in Global.PersonalEndpoints)
            {
                BankEndpoint b = UTILS.PickRandomBankEndpoint();

                b.Clients.Add(e.Owner);
                e.Owner.BankBalance = Global.Rand.Next(100, 1000000); //For testing;
            }

            EndpointList.Add(GenerateStartEndpoint());
            //EndpointList.Add(GenerateTestEndpoint(AvailableEmployes));
            return EndpointList;
        }
        private void GeneatePersonalEndpoint(List<Endpoint> EndpointList, List<Endpoint> AvailableEmployes)
        {
            Person Person = UTILS.PickRandomPerson();
            Endpoint e = new Endpoint(Person, EndpointType.PERSONAL);
            e.isHidden = true;
            (e.x, e.y) = GenerateCoordinate();
            e.AddUser(Person, Person.PersonalPassword, AccessLevel.ROOT);
            Person.PersonalComputer = e;
            EndpointList.Add(e);
            Global.PersonalEndpoints.Add(e);
            AvailableEmployes.Add(e);
        }
        private void GenerateCompanyEndpoint(int nrOfEmployes, EndpointDifficulty dificulty, List<Endpoint> EndpointList, List<Endpoint> AvailableEmployes)
        {
            for (int i = 0; i < nrOfEmployes + 10; i++)
            {
                GeneatePersonalEndpoint(EndpointList, AvailableEmployes);
            }

            EndpointMonitor Monitor = EndpointMonitor.NONE;
            EndpointFirewall Firewall = EndpointFirewall.NONE;
            EndpointHashing MemoryHashing = EndpointHashing.NONE;
            bool hidden = false;

            switch (dificulty)
            {
                case EndpointDifficulty.LVL0:
                    hidden = false;
                    break;

                case EndpointDifficulty.LVL1:
                    Monitor = EndpointMonitor.LVL1;
                    hidden = true;
                    break;

                case EndpointDifficulty.LVL2:
                    Monitor = EndpointMonitor.LVL1;
                    Firewall = EndpointFirewall.LVL1;
                    hidden = true;
                    break;

                case EndpointDifficulty.LVL3:
                    Monitor = EndpointMonitor.LVL1;
                    Firewall = EndpointFirewall.LVL1;
                    MemoryHashing = EndpointHashing.LVL1;
                    hidden = true;
                    break;

                case EndpointDifficulty.LVL4:
                    break;

                case EndpointDifficulty.LVL5:
                    break;

                case EndpointDifficulty.LVL6:
                    break;

                case EndpointDifficulty.LVL7:
                    break;

                case EndpointDifficulty.LVL8:
                    break;

                case EndpointDifficulty.LVL9:
                    break;

                case EndpointDifficulty.LVL10:
                    break;

                default:
                    break;
            }

            Endpoint[] employes = PickRandomEmploye(nrOfEmployes, AvailableEmployes);
            Endpoint Admin = PickRandomEmploye(1, AvailableEmployes)[0];

            //For testing
            if (!hidden)
            {
                Admin.isHidden = false;
                employes.ToList().ForEach(x => x.isHidden = false);
            }

            Global.EmployeEndpoints.Add(Admin);
            Global.EmployeEndpoints.AddRange(employes);

            Person Person = UTILS.PickRandomCompany();
            Endpoint external = new Endpoint(Person, EndpointType.EXTERNALACCES);
            external.isHidden = hidden;
            external.MemoryHashing = MemoryHashing;
            external.Firewall = Firewall;
            external.Monitor = Monitor;
            (external.x, external.y) = GenerateCoordinate();
            external.AddEmployes(employes);
            external.AddUser(Admin.Owner, Admin.Owner.WorkPassword, AccessLevel.ADMIN);
            EndpointList.Add(external);
            Global.CompanyEndpoints.Add(external);

            Endpoint inter = new Endpoint(Person, EndpointType.INTERNAL);
            (inter.x, inter.y) = GenerateCoordinate();
            inter.isHidden = hidden;
            inter.MemoryHashing = MemoryHashing;
            inter.Firewall = Firewall;
            inter.Monitor = Monitor;
            inter.AddEmployes(employes);
            inter.AddUser(Admin.Owner, Admin.Owner.WorkPassword, AccessLevel.ADMIN);
            EndpointList.Add(inter);
            Global.CompanyEndpoints.Add(inter);

            Endpoint database = new Endpoint(Person, EndpointType.DATABASE);
            (database.x, database.y) = GenerateCoordinate();
            database.isHidden = hidden;
            database.MemoryHashing = MemoryHashing;
            database.Firewall = Firewall;
            database.Monitor = Monitor;
            database.AddEmployes(employes);
            database.AddUser(Admin.Owner, Admin.Owner.WorkPassword, AccessLevel.ADMIN);
            EndpointList.Add(database);
            Global.CompanyEndpoints.Add(database);

            inter.AllowedConnections.Add(database);
            inter.AllowedConnections.Add(external);

            database.AllowedConnections.Add(inter);
            database.AllowedConnections.Add(external);
        }
        private Endpoint[] PickRandomEmploye(int nr, List<Endpoint> availableEmployes)
        {
            Endpoint[] randomEmployes = new Endpoint[nr];
            for (int i = 0; i < nr; i++)
            {
                randomEmployes[i] = availableEmployes[Rand.Next(availableEmployes.Count)];
                availableEmployes.Remove(randomEmployes[i]);
            }
            return randomEmployes;
        }
        private (int, int) GenerateCoordinate()
        {
            int d = 50;
            int x = Rand.Next(0, XMax);
            int y = Rand.Next(0, YMax);

            double prop = EndpointCoordinates[x, y];

            //water check
            if (prop == 0)
            {
                return GenerateCoordinate();
            }

            double roll = Global.Rand.NextDouble();

            if (roll > prop)
            {
                return GenerateCoordinate();
            }

            //Proximity check
            bool reject = false;
            for (int xmin = x - d; xmin < x + d; xmin++)
            {
                if (xmin < 0 || xmin >= XMax)
                {
                    continue;
                }
                for (int ymin = y - d; ymin < y + d; ymin++)
                {
                    if (ymin < 0 || ymin >= YMax)
                    {
                        continue;
                    }
                    if (EndpointCoordinates[xmin, ymin] == 1)
                    {
                        reject = true;
                        break;
                    }
                    ymin++;
                }
                if (reject)
                {
                    break;
                }
                xmin++;
            }
            if (reject)
            {
                return GenerateCoordinate();
            }

            EndpointCoordinates[x, y] = 1;
            return (x, y);
        }
    }
}