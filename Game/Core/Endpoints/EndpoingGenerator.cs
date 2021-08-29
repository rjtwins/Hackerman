using Game.Properties;
using System;
using System.Collections.Generic;
using static Game.UTILS;

namespace Game.Core.Endpoints
{
    public class EndpointGenerator
    {
        public static int XMax = 4984;
        public static int YMax = 2576;
        public static int EndpointBaseSize = 6;
        private int[,] EndpointCoordinates;
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
            Endpoint e = new LocalEndpoint();
            (e.x, e.y) = GenerateCoordinate();
            e.name = "LOCAL";
            Global.StartEndPoint = e;
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
            this.EndpointCoordinates = UTILS.getBoolBitmap(20, Resources.WorldMapDensity);
            List<Endpoint> EndpointList = new List<Endpoint>();
            List<Endpoint> AvailableEmployes = new();

            //Generate 250 personal machines:
            for (int i = 0; i < 250; i++)
            {
                Person Person = UTILS.PickRandomPerson();
                Endpoint e = new Endpoint(Person, EndpointType.PERSONAL);
                (e.x, e.y) = GenerateCoordinate();
                e.AddUser(Person, Person.PersonalPassword, AccessLevel.ROOT);
                Person.PersonalComputer = e;
                EndpointList.Add(e);
                Global.PersonalEndpoints.Add(e);
                AvailableEmployes.Add(e);
            }

            //Generate 20 random companies machines:
            for (int i = 0; i < 20; i++)
            {
                Endpoint[] employes = PickRandomEmploye(10, AvailableEmployes);
                Endpoint Admin = PickRandomEmploye(1, AvailableEmployes)[0];

                Person Person = UTILS.PickRandomCompany();
                Endpoint external = new Endpoint(Person, EndpointType.EXTERNALACCES);
                (external.x, external.y) = GenerateCoordinate();
                external.AddEmployes(employes);
                external.AddUser(Admin.Owner, Admin.Owner.WorkPassword, AccessLevel.ADMIN);
                EndpointList.Add(external);
                Global.CompanyEndpoints.Add(external);

                Endpoint inter = new Endpoint(Person, EndpointType.INTERNAL);
                (inter.x, inter.y) = GenerateCoordinate();
                inter.AddEmployes(employes);
                inter.AddUser(Admin.Owner, Admin.Owner.WorkPassword, AccessLevel.ADMIN);
                EndpointList.Add(inter);
                Global.CompanyEndpoints.Add(inter);

                Endpoint database = new Endpoint(Person, EndpointType.DATABASE);
                (database.x, database.y) = GenerateCoordinate();
                database.AddEmployes(employes);
                database.AddUser(Admin.Owner, Admin.Owner.WorkPassword, AccessLevel.ADMIN);
                EndpointList.Add(database);
                Global.CompanyEndpoints.Add(database);

                inter.AllowedConnections.Add(database);
                inter.AllowedConnections.Add(external);

                database.AllowedConnections.Add(inter);
                database.AllowedConnections.Add(external);
            }

            //Generate 5 bank machines:
            for (int i = 0; i < 5; i++)
            {
                Person Person = UTILS.PickRandomBank();
                Endpoint e = new Endpoint(Person, EndpointType.BANK);
                (e.x, e.y) = GenerateCoordinate();
                EndpointList.Add(e);
            }

            EndpointList.Add(GenerateStartEndpoint());
            return EndpointList;
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
            int d = 100;
            int x = Rand.Next(0, XMax);
            int y = Rand.Next(0, YMax);

            //water check
            if (EndpointCoordinates[x, y] == 0)
            {
                //var range = EndpointCoordinates[new Range(x-5, x+5), new Range(y-5, y+5)];
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
                (x, y) = GenerateCoordinate();
            }

            EndpointCoordinates[x, y] = 1;
            return (x, y);
        }
    }
}