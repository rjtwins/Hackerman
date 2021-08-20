using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public static class MissionDictionaryParser
    {
        public static List<string> SmallMissionGivers = new List<string>();
        public static List<string> BigMissionGivers = new List<string>();
        public static Dictionary<MissionType, List<string>> TypeSequenceListDict = new Dictionary<MissionType, List<string>>();

        public static void ParseFromFiles()
        {
            //Parse the mission giver files
            SmallMissionGivers = File.ReadAllLines(Environment.CurrentDirectory + "\\Core\\Mission\\Dictionaries\\" + "smallNames.txt").ToList<string>();
            BigMissionGivers = File.ReadAllLines(Environment.CurrentDirectory + "\\Core\\Mission\\Dictionaries\\" + "bigNames.txt").ToList<string>();

            //Parse find the sequence files.
            foreach (string value in Enum.GetNames(typeof(MissionType)))
            {
                MissionType missionType = (MissionType)Enum.Parse(typeof(MissionType), value);
                int i = 0;
                string filePath = Environment.CurrentDirectory + "\\Core\\Mission\\Dictionaries\\" + value.ToLower() + i.ToString() + ".spd";
                while (File.Exists(filePath))
                {
                    if (!TypeSequenceListDict.ContainsKey(missionType))
                    {
                        TypeSequenceListDict[missionType] = new List<string>();
                    }
                    TypeSequenceListDict[missionType].Add(value.ToLower() + "_" + i + ".spd");
                    i++;
                    filePath = Environment.CurrentDirectory + "\\Core\\Mission\\Dictionaries\\" + value.ToLower() + i.ToString() + ".spd";
                }
            }
        }
    }
}
