using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public static class MissionGenerator
    {
        private static Random Rand = new Random();
        public static void GenerateMission(int dificulty)
        {
            MissionType[] temp = Mission.DifficultyMissionTypeDict[dificulty];
            MissionGenerator.GenerateMission(temp[Rand.Next(temp.Length)], dificulty);

        }

        //TODO finish this:
        //TODO realistic moneys
        public static void GenerateMission(MissionType missionType, int dificulty)
        {
            Mission m = new Mission();
            m.MissionType = missionType;
            m.Difficulty = dificulty;
            m.Contact = PickRandomContact(missionType, dificulty);
            m.Reward = Rand.Next(dificulty * 1000 - 1000, dificulty * 1000 + 1000);
            m.RepReward = dificulty;
            m.MissionChannel = UTILS.GenerateRandomString(7);
        }

        private static string PickRandomContact(MissionType missionType, int dificulty)
        {
            //TODO: do something with diffuculty.
            return MissionDictionaryParser.SmallMissionGivers[Rand.Next(MissionDictionaryParser.SmallMissionGivers.Count)];
        }
    }
}
