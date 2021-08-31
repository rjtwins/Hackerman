using Game.Core.Console;
using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.FileSystem
{
    class TrafficFile : Program
    {
        public List<Traffic> Traffic { get; private set; } = new();

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
            this.Traffic.ForEach(x => { if(x.Crack(crackingLevel) == true) succeses += 1; });
            return succeses;
        }
    }
}
