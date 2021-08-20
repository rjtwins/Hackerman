using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public class Mission
    {
        public static Dictionary<int, MissionType[]> DifficultyMissionTypeDict = new Dictionary<int, MissionType[]>()
        {
            { 1, new MissionType[] {MissionType.STEAL, MissionType.DELETE } }
        };


        public Mission()
        {
            this.id = Guid.NewGuid();

        }

        internal Guid id;
        internal MissionType MissionType;
        internal int MinRating = 0;
        internal int Difficulty = 0;
        internal int Reward = 0;
        internal int RepReward = 0;
        internal string MissionChannel = string.Empty;
        internal bool CloseChannelAfterMission = false;
        internal string Contact = string.Empty;
    }
}

