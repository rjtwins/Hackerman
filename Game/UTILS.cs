﻿using Game.Core.Endpoints;
using Game.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Game
{
    public static class UTILS
    {
        private readonly static Random Rand = new();
        public static List<string> PasswordList = new();
        public static List<Person> PersonList = new();
        public static List<Company> Companies = new();
        public static List<Company> BankList = new();

        public static Color[] GlobalExcludedColors = new Color[]
        {
            Color.Black,
            Color.Gray,
            Color.DarkGray,
            Color.DarkSlateGray
        };

        public static double[,] getBoolBitmap(Bitmap b)
        {
            double[,] ar = new double[b.Width, b.Height];
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    if (b.GetPixel(x, y).A >= 200)
                    {
                        ar[x, y] = Math.Max((double)b.GetPixel(x, y).R, 1d) / 255d;
                    }
                    else
                    {
                        ar[x, y] = 0;
                    }
                }
            }
            return ar;
        }

        internal static (bool GuestUser, Endpoint) PickRandomEndpointWithAccess(Endpoint e)
        {
            if (e.AllowedConnections.Count == 0)
            {
                Person randomUser = e.GetRandomUser();
                if (randomUser.Name == "guest")
                {
                    return (true, PickRandomPersonWithEndpoint().PersonalComputer);
                }
                return (false, randomUser.PersonalComputer);
            }
            return (false, e.AllowedConnections[Rand.Next(e.AllowedConnections.Count)]);
        }

        internal static Endpoint PickRandomCompanyEdnpoint()
        {
            int randomIndex = Rand.Next(Global.CompanyEndpoints.Count);
            var temp = Global.CompanyEndpoints[randomIndex];

            if (temp.Id == Global.LocalEndpoint.Id)
            {
                return PickRandomEndpoint();
            }

            if (Global.RemoteSystem == temp)
            {
                return PickRandomEndpoint();
            }

            if (temp.HasConnection())
            {
                return PickRandomEndpoint();
            }

            return temp;
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

        /// <summary>
        /// Recursivly pick a color and check if its not in the exluded list
        /// </summary>
        /// <param name="toExclude"></param>
        /// <returns></returns>
        public static Color PickRandomColor(List<Color> toExclude = null)
        {
            KnownColor[] names = (KnownColor[])Enum.GetValues(typeof(KnownColor));
            KnownColor randomColorName = names[Rand.Next(names.Length)];
            Color randomColor = Color.FromKnownColor(randomColorName);
            if (GlobalExcludedColors.Contains<Color>(randomColor))
            {
                return PickRandomColor(toExclude);
            }
            if (toExclude.Contains(randomColor))
            {
                return PickRandomColor(toExclude);
            }
            return randomColor;
        }

        internal static BankEndpoint PickRandomBankEndpoint()
        {
            return Global.BankEndpoints[UTILS.Rand.Next(Global.BankEndpoints.Count)];
        }

        internal static string GetPasswordByIndex(int index)
        {
            return PasswordList[index];
        }

        internal static string PickRandomPassword()
        {
            return PasswordList[UTILS.Rand.Next(PasswordList.Count)];
        }

        internal static Person PickRandomPerson()
        {
            Person p = PersonList[UTILS.Rand.Next(PersonList.Count)];
            if (string.IsNullOrEmpty(p.PersonalPassword))
            {
                p.PersonalPassword = UTILS.PickRandomPassword();
            }
            if (string.IsNullOrEmpty(p.WorkPassword))
            {
                p.WorkPassword = UTILS.PickRandomPassword();
            }
            return p;
        }

        internal static Person PickRandomPersonWithEndpoint()
        {
            return Global.PersonalEndpoints[Rand.Next(Global.PersonalEndpoints.Count)].Owner;
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

        public static Person PickRandomBank()
        {
            return BankList[UTILS.Rand.Next(BankList.Count)];
        }

        public static Company PickRandomCompany()
        {
            return Companies[UTILS.Rand.Next(Companies.Count)];
        }

        public static void LoadExternalLists()
        {
            //Passwords
            using (FileStream fs = File.Open(Environment.CurrentDirectory + "\\Misc\\top100000.txt", FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    UTILS.PasswordList.Add(s);
                }
            }
            //Usernames
            using (FileStream fs = File.Open(Environment.CurrentDirectory + "\\Misc\\persons.csv", FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    UTILS.PersonList.Add(Person.FromCSV(s));
                }
            }

            //Banks
            using (FileStream fs = File.Open(Environment.CurrentDirectory + "\\Misc\\banks.txt", FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    UTILS.BankList.Add(Company.FromNameLine(s));
                }
            }

            //Fake companies
            using (FileStream fs = File.Open(Environment.CurrentDirectory + "\\Misc\\faux_id_fake_companies.csv", FileMode.Open))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string s;
                while ((s = sr.ReadLine()) != null)
                {
                    s = s.Replace("\"", "");
                    UTILS.Companies.Add(Company.FromCSV(s));
                }
            }
        }

        /// <summary>
        /// Pick a random endpoint that is not the user and is not currently beeing connected to.
        /// </summary>
        /// <returns></returns>
        public static Endpoint PickRandomEndpoint()
        {
            int randomIndex = Rand.Next(Global.AllEndpoints.Count);
            var temp = Global.AllEndpoints[randomIndex];

            if (temp.Id == Global.LocalEndpoint.Id)
            {
                return PickRandomEndpoint();
            }

            if (Global.RemoteSystem == temp)
            {
                return PickRandomEndpoint();
            }

            if (temp.HasConnection())
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

        public static Endpoint PickRandomEmployeEndpoint()
        {
            return Global.EmployeEndpoints[Rand.Next(Global.EmployeEndpoints.Count)];
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            }
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString();
        }

        public static void PlayBoob()
        {
            
            SoundPlayer player = new SoundPlayer(Environment.CurrentDirectory + @"\Resources\boop.wav");
            player.Play();

            //Uri uri = new Uri(@"pack://application:,,,/Resources/beepbeep.wav");
            //var player = new MediaPlayer();
            //player.Open(uri);
            //player.Play();
        }

        public class FrequencyBooper
        {
            public Double Frequency = 1;
            private bool StopBooper = false;
            private SoundPlayer Player;
            public FrequencyBooper(double frequency)
            {
                this.Frequency = frequency;
                Player = new SoundPlayer(Environment.CurrentDirectory + @"\Resources\boop.wav");
            }

            public void Start()
            {
                this.StopBooper = false;

                Task.Factory.StartNew(() =>
                {
                    while (!StopBooper)
                    {
                        Player.Play();
                        Global.EventTicker.SleepSeconds((1 / Frequency));
                    }
                });
            }

            public void Stop()
            {
                this.StopBooper = true;
            }
        }
    }
}