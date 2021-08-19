using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    class Mission
    {
        public static Dictionary<int, MissionType[]> DifficultyMissionTypeDict = new Dictionary<int, MissionType[]>()
        {
            { 1, new MissionType[] {MissionType.STEAL, MissionType.DELETE } }
        };

        MissionType MissionType;
        int MinRating = 0;
        int Difficulty = 0;
        int Reward = 0;
        int RepReward = 0;
        string MissionChannel = string.Empty;
        bool CloseChannelAfterMission = false;
        string Contact = string.Empty;
    }
}

