using Game.Core.Endpoints;
using System;
using System.Drawing;

namespace Game
{
    public static class UTILS
    {
        private readonly static Random Rand = new Random();
        public static int[,] getBoolBitmap(uint treshold, Bitmap b)
        {
            int[,] ar = new int[b.Width, b.Height];

            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    if (b.GetPixel(x, y).A >= treshold)
                    {
                        ar[x, y] = 2;
                        //Debug.WriteLine(false);
                    }
                    else
                    {
                        ar[x, y] = 0;
                    }
                }
            }
            return ar;
        }

        public static string AccessLevelString(AccessLevel a)
        {
            switch (a)
            {
                case AccessLevel.USER:
                    return "USER";

                case AccessLevel.ADMIN:
                    return "ADMIN";

                case AccessLevel.ROOT:
                    return "ROOT";

                default:
                    throw new Exception("AccessLevel outside of bounds!");
            }
        }

        public static string GenerateRandomString(int n)
        {
            //ABCDEFGHIJKLMNOPQRSTUVWXYZ
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[n];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public static Endpoint PickRandomEndpoint()
        {
            int randomIndex = Rand.Next(Global.AllEndpoints.Count);
            var temp = Global.AllEndpoints[randomIndex];
            if(temp.Id == Global.StartEndPoint.Id)
            {
                return PickRandomEndpoint();
            }
            return temp;
        }

        //TODO: make some believable file names.
        public static string PickRandomFileName()
        {
            return GenerateRandomString(4) + ".data";
        }
    }
}