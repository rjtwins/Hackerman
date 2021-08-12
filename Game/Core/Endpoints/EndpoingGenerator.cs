using Game.Properties;
using System;
using System.Collections.Generic;

namespace Game.Core.Endpoints
{
    public class EndpointGenerator
    {
        public static int XMax = 3950;
        public static int YMax = 1616;
        public static int EndpointBaseSize = 6;
        private int[,] EndpointCoordinates;
        private Random Rand = new Random();

        /// <summary>
        /// Add endpoint at cooridinates on overmap.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Endpoint AddEndpoint(int x, int y)
        {
            Endpoint e = new Endpoint();

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
        public Endpoint AddEndpoint()
        {
            Endpoint e = new Endpoint();
            (e.x, e.y) = GenerateCoordinate();
            return e;
        }

        public List<Endpoint> GenerateEndpoints(int number)
        {
            this.EndpointCoordinates = UTILS.getBoolBitmap(20, Resources.WorldMapDensity);
            List<Endpoint> EndpointList = new List<Endpoint>();
            for (int i = 0; i < number; i++)
            {
                Endpoint e = new Endpoint();
                (e.x, e.y) = GenerateCoordinate();
                EndpointList.Add(e);
            }
            EndpointList.Add(GenerateStartEndpoint());
            return EndpointList;
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

            ////Proximity check
            //bool reject = false;
            //for (int xmin = x - d; xmin < x + d; xmin++)
            //{
            //    if (xmin < 0 || xmin >= XMax)
            //    {
            //        continue;
            //    }
            //    for (int ymin = y - d; ymin < y + d; ymin++)
            //    {
            //        if (ymin < 0 || ymin >= YMax)
            //        {
            //            continue;
            //        }
            //        if (EndpointCoordinates[xmin, ymin] == 1)
            //        {
            //            reject = true;
            //            break;
            //        }
            //        ymin++;
            //    }
            //    if (reject)
            //    {
            //        break;
            //    }
            //    xmin++;
            //}
            //if (reject)
            //{
            //    (x, y) = GenerateCoordinate();
            //}

            EndpointCoordinates[x, y] = 1;
            return (x, y);
        }
    }
}