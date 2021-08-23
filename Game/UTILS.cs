using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Properties;

namespace Game
{
    public static class UTILS
    {
        private readonly static Random Rand = new Random();
        public static List<string> PasswordList = new List<string>();
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

        internal static string GetPasswordByIndex(int index)
        {
            return PasswordList[index];
        }
        internal static string PickrandomPassword()
        {
            return PasswordList[UTILS.Rand.Next(PasswordList.Count)];
        }

        public static string GenerateRandomString(int n)
        {
            //ABCDEFGHIJKLMNOPQRSTUVWXYZ
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[n];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[UTILS.Rand.Next(chars.Length)];
            }

            return new String(stringChars);
        }

        public static void LoadPasswordFile()
        {
            UTILS.PasswordList.Add("");
            //StreamReader reader = File.OpenText(Resources.top100000);
            //Task<string>[] taskArray = new Task<string>[100000];
            //for (int i = 0; i < 100000; i++)
            //{
            //    taskArray[i] = reader.ReadLineAsync();
            //}
            //Task.WaitAll(taskArray);
            //for (int i = 0; i < taskArray.Length; i++)
            //{
            //    UTILS.PasswordList.Add(taskArray[i].Result);
            //}

            using (FileStream fs = File.Open(Environment.CurrentDirectory + "\\Misc\\top100000.txt", FileMode.Open ))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    UTILS.PasswordList.Add(s);
                }
            }
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