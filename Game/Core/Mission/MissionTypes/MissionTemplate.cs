using Game.Core.Dialog;
using Game.Core.Endpoints;
using Game.Model;
using System;
using System.Collections.Generic;

namespace Game.Core.Mission.MissionTypes
{
    public abstract class MissionTemplate
    {
        public enum MissionStatus { ONOFFER, ACCEPTED, COMPLETED, REJECTED };

        public static Dictionary<int, MissionType[]> DifficultyMissionTypeDict = new Dictionary<int, MissionType[]>()
        {
            { 0, new MissionType[] {MissionType.STEAL, MissionType.STEALMULTIPLE, MissionType.UPLOAD} },
            { 1, new MissionType[] {MissionType.DELETEMULTIPLE, MissionType.DELETEALL } },
            { 2, new MissionType[] {MissionType.CHANGEINFO, MissionType.FRAME } },
        };

        public MissionTemplate()
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

        public abstract bool CheckMissionCompleted();

        public abstract void Setup();

        public abstract void RemoveMission();

        internal abstract bool CheckFileNeeded(string v);

        internal abstract void FileUploaded(string v);

        internal abstract bool GetFile();
    }
}