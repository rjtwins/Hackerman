using Newtonsoft.Json;
using System;

namespace Game.Model
{
    public class Company : Person
    {
        [JsonConstructor]
        public Company(object o = null)
        {

        }

        public Company() : base()
        {

        }

        public static Company FromCSV(string csvLine)
        {
            string[] values = csvLine.Split(',');
            Company company = new Company();
            company.Name = values[1];
            company.Description = values[2];
            company.TagLine = values[3];
            return company;
        }

        public static Company FromNameLine(string name)
        {
            Company company = new Company();
            company.Name = name;
            return company;
        }
    }
}