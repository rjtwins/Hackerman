using System;
using System.Drawing;

namespace Game
{
    public static class UTILS
    {
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
                    throw new Exception("AccessLeve outside of bounds!");
            }
        }
    }
}