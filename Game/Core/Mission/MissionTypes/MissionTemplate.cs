using Game.Core.Dialog;
using Game.Core.Endpoints;
using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Game.Core.Mission.MissionTypes
{
    [JsonObject(MemberSerialization.OptIn)]
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
            if(this.Id == Guid.Empty)
            {
                this.Id = Guid.NewGuid();
                this.ListingDate = Global.GameTime;
                Global.AllMissionsDict[Id] = this;
            }
        }

        [JsonConstructor]
        public MissionTemplate(object o)
        {

        }

        [JsonProperty]
        private Guid id;
        [JsonProperty]
        private DateTime listingDate;
        [JsonProperty]
        private DateTime acceptDate;
        [JsonProperty]
        private MissionType missionType;
        [JsonProperty]
        private MissionStatus status;
        [JsonProperty]
        private int minRating = 0;
        [JsonProperty]
        private int difficulty = 0;
        [JsonProperty]
        private int reward = 0;
        [JsonProperty]
        private int repReward = 0;
        [JsonProperty]
        private string missionChannel = string.Empty;
        [JsonProperty]
        private bool closeChannelAfterMission = false;
        [JsonProperty]
        private bool expires = false;
        [JsonProperty]
        private string contact = string.Empty;
        [JsonProperty]
        private string target = string.Empty;
        [JsonProperty]
        private GenericMissionDialogResolver dialogResolver;
        [JsonProperty]
        private Guid iRCChannel;
        [JsonProperty]
        private Guid targetEndpoint;

        public Guid Id { get => id; private set => id = value; }
        internal DateTime ListingDate { get => listingDate; set => listingDate = value; }
        internal DateTime AcceptDate { get => acceptDate; set => acceptDate = value; }
        internal MissionType MissionType { get => missionType; set => missionType = value; }
        internal MissionStatus Status { get => status; set => status = value; }
        internal int MinRating { get => minRating; set => minRating = value; }
        internal int Difficulty { get => difficulty; set => difficulty = value; }
        internal int Reward { get => reward; set => reward = value; }
        internal int RepReward { get => repReward; set => repReward = value; }
        internal string MissionChannel { get => missionChannel; set => missionChannel = value; }
        internal bool CloseChannelAfterMission { get => closeChannelAfterMission; set => closeChannelAfterMission = value; }
        internal bool Expires { get => expires; set => expires = value; }
        internal string Contact { get => contact; set => contact = value; }
        internal string Target { get => target; set => target = value; }
        
        //TODO: set dialog and IRC channels to ID reference.
        internal GenericMissionDialogResolver DialogResolver { get => dialogResolver; set => dialogResolver = value; }
        internal IRCChannel IRCChannel { get => Global.AllIRCChannelsDict[iRCChannel]; set => iRCChannel = value.Id; }
        internal Endpoint TargetEndpoint { get => Global.AllEndpointsDict[targetEndpoint]; set => targetEndpoint = value.Id; }

        public abstract bool CheckMissionCompleted();

        public abstract void Setup();

        public abstract void RemoveMission();

        internal abstract bool CheckFileNeeded(string v);

        internal abstract void FileUploaded(string v);

        internal abstract bool GetFile();
    }
}