using Game.Core.Endpoints;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Game.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Person
    {
        [JsonProperty]
        public Guid Id { get; set; }
        [JsonProperty]
        public string Gender;
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public string Surname;
        [JsonProperty]
        public string Email;
        [JsonProperty]
        public string Username;
        [JsonProperty]
        public string PersonalPassword;
        [JsonProperty]
        public string WorkPassword;
        [JsonProperty]
        public string BankPassword;
        [JsonProperty]
        public int BankBalance;
        [JsonProperty]
        public string Birthday;
        [JsonProperty]
        public string Age;
        [JsonProperty]
        public string CCType;
        [JsonProperty]
        public string CCNumber;
        [JsonProperty]
        public string CCExpire;
        [JsonProperty]
        public string NationalID;
        [JsonProperty]
        public string Occupation;
        [JsonProperty]
        public string BloodType;
        [JsonProperty]
        public string Kilograms;
        [JsonProperty]
        public string Centimiters;
        [JsonProperty]
        public string Description;
        [JsonProperty]
        public string TagLine;
        [JsonProperty]
        public Guid personalComputer;

        [JsonIgnore]
        public Endpoint PersonalComputer 
        { 
            get 
            {
                return Global.AllEndpointsDict[personalComputer];
            }
            set
            {
                personalComputer = value.Id;
            }
        }

        [JsonConstructor]
        public Person(object o = null)
        {

        }

        public Person()
        {
            if (Global.SerializingDictionaries)
            {
                return;
            }
            this.Id = Guid.NewGuid();
            Global.AllPersonsDict[Id] = this;
        }

        public static Person FromCSV(string csvLine)
        {
            string[] values = csvLine.Split(',');
            Person person = new Person();
            person.Gender = values[0];
            person.Name = values[1];
            person.Surname = values[2];
            person.Email = values[3];
            person.Username = values[4];
            //person.WorkPassword = values[5];
            //person.PersonalPassword = values[5];
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
}