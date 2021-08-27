using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
            return PersonList[UTILS.Rand.Next(PersonList.Count)];
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

        public static Owner PickRandomBank()
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

            if (temp.Id == Global.StartEndPoint.Id)
            {
                return PickRandomEndpoint();
            }

            if (Global.RemoteSystem == temp)
            {
                return PickRandomEndpoint();
            }

            if (temp.SoftConnection)
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

        public class Owner
        {
            public Guid Guid;
            public string Gender;
            public string Name;
            public string Surname;
            public string Email;
            public string Username;
            public string Password;
            public string Birthday;
            public string Age;
            public string CCType;
            public string CCNumber;
            public string CCExpire;
            public string NationalID;
            public string Occupation;
            public string BloodType;
            public string Kilograms;
            public string Centimiters;
            public string Description;
            public string TagLine;
        }

        public class Person : Owner
        {
            public static Person FromCSV(string csvLine)
            {
                string[] values = csvLine.Split(',');
                Person person = new Person();
                person.Guid = Guid.NewGuid();
                person.Gender = values[0];
                person.Name = values[1];
                person.Surname = values[2];
                person.Email = values[3];
                person.Username = values[4];
                person.Password = values[5];
                person.Birthday = values[6];
                person.Age = values[7];
                person.CCType = values[8];
                person.CCNumber = values[9];
                person.CCExpire = values[10];
                person.NationalID = values[11];
                person.Occupation = values[12];
                person.BloodType = values[13];
                person.Kilograms = values[14];
                person.Centimiters = values[15];
                return person;
            }
        }

        public class Company : Owner
        {
            public static Company FromCSV(string csvLine)
            {
                string[] values = csvLine.Split(',');
                Company company = new Company();
                company.Guid = Guid.NewGuid();
                company.Name = values[1];
                company.Description = values[2];
                company.TagLine = values[3];
                return company;
            }

            public static Company FromNameLine(string name)
            {
                Company company = new Company();
                company.Guid = Guid.NewGuid();
                company.Name = name;
                return company;
            }
        }
    }
}