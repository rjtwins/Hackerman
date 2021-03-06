using Game.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Game.Core.FileSystem
{
    internal class TrafficFile : Program
    {
        [JsonProperty]
        private List<Traffic> traffic = new();
        [JsonIgnore]
        public List<Traffic> Traffic { get => traffic; set => traffic = value; }

        [JsonConstructor]
        public TrafficFile()
        {

        }

        public TrafficFile(string fileName) : base(fileName, false)
        {
        }

        public void AddTraffic(Traffic t)
        {
            this.Traffic.Add(t);
        }

        public override string ToString()
        {
            string result = string.Empty;
            this.Traffic.ForEach(x => result += x.ToString() + "\n");
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="crackingLevel"></param>
        /// <returns>int nr of successfully cracked entries in file.</returns>
        public int TryCrack(int crackingLevel)
        {
            int succeses = 0;
            this.Traffic.ForEach(x => { if (x.Crack(crackingLevel) == true) succeses += 1; });
            return succeses;
        }
    }
}