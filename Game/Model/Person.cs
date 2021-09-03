using Game.Core.Endpoints;
using System;

namespace Game.Model
{
    public class Person
    {
        public Guid Guid;
        public string Gender;
        public string Name;
        public string Surname;
        public string Email;
        public string Username;
        public string PersonalPassword;
        public string WorkPassword;
        public string BankPassword;
        public int BankBalance;
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
        public Endpoint PersonalComputer;

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
            //person.Password = values[5];
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