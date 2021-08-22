using Game.Core.Dialog;
using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public class Mission
    {
        public enum MissionStatus { ONOFFER, ACCEPTED, COMPLETED, REJECTED };

        public static Dictionary<int, MissionType[]> DifficultyMissionTypeDict = new Dictionary<int, MissionType[]>()
        {
            { 0, new MissionType[] {MissionType.STEAL} },
            { 1, new MissionType[] {MissionType.DELETEMULTIPLE, MissionType.DELETEALL } },
            { 2, new MissionType[] {MissionType.CHANGEINFO, MissionType.FRAME } },
        };


        public Mission()
        {
            this.id = Guid.NewGuid();
            this.ListingDate = Global.GameTime;
        }

        internal Guid id;
        internal DateTime ListingDate;
        internal DateTime AcceptDate;
        internal MissionType MissionType;
        internal MissionStatus Status;
        internal int MinRating = 0;
        internal int Difficulty = 0;
        internal int Reward = 0;
        internal int RepReward = 0;
        internal string MissionChannel = string.Empty;
        internal bool CloseChannelAfterMission = false;
        internal bool Expires = false;
        internal string Contact = string.Empty;
        internal string Target = string.Empty;
        internal GenericMissionDialogResolver DialogResolver;
        internal IRCChannel IRCChannel;
        internal Endpoint TargetEndpoint;


        public virtual bool CheckMissionCompleted()
        {
            return false;
        }

        public virtual void Setup()
        {
            //do nothing
        }

        public virtual void RemoveMission()
        {
            //Do nothing
        }
    }
}