using Game.Core.Events;
using Game.Core.Mission.MissionTypes;
using Game.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MissionManager
    {
        private static MissionManager instance;
        public static MissionManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new MissionManager();
                }
                return instance;
            }
            private set
            {
                
            }
        }

        [JsonProperty]
        public ReferenceList<MissionTemplate> ActiveListings = new (Global.AllMissionsDict, "AllMissionsDict");
        [JsonProperty]
        public ReferenceList<MissionTemplate> AcceptedMissions = new (Global.AllMissionsDict, "AllMissionsDict");
        
        private readonly Random Rand = new Random();

        [JsonConstructor]
        public MissionManager()
        {

        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            MissionManager.instance = this;
            ActiveListings.SetReferenceDict(Global.AllMissionsDict);
            AcceptedMissions.SetReferenceDict(Global.AllMissionsDict);
        }

        public void AcceptMission(MissionTemplate mission)
        {
            this.ActiveListings.Remove(mission);
            this.AcceptedMissions.Add(mission);
            mission.Status = MissionTemplate.MissionStatus.ACCEPTED;
        }

        public static void StaticEvaluateMissionListings(params Object[] args)
        {
            MissionManager.Instance.EvaluateMissionListings();
        }

        public void EvaluateMissionListings()
        {
            List<MissionTemplate> toRemove = new List<MissionTemplate>();
            foreach (MissionTemplate m in this.ActiveListings)
            {
                if (Mission48HoursOld(m))
                {
                    toRemove.Add(m);
                    Global.App.Dispatcher.Invoke((Action)delegate
                    {
                        Global.IRCWindow.RemoveChannel(m.MissionChannel);
                    });
                }
            }
            toRemove.ForEach(x => ActiveListings.Remove(x));


            if (ActiveListings.Count < 5)
            {
                //We must do this on the UI thread since we are editing UI elements,
                Global.App.Dispatcher.Invoke(() =>
                {
                    this.ActiveListings.Add(MissionGenerator.GenerateMission(this.Rand.Next(Math.Max(GameState.Instance.Reputation - 2, 0), GameState.Instance.Reputation)));
                    this.ActiveListings.Add(MissionGenerator.GenerateMission(this.Rand.Next(Math.Max(GameState.Instance.Reputation - 2, 0), GameState.Instance.Reputation)));
                });
            }

            EventBuilder.BuildEvent("MissionListingEval")
                .EventInterval(3600)
                .EventAction(MissionManager.StaticEvaluateMissionListings, new object[] { })
                .RegisterWithAction();
        }

        private static bool Mission48HoursOld(MissionTemplate m)
        {
            return Global.GameTime > m.ListingDate.AddHours(48);
        }

        internal void RejectMission(MissionTemplate mission)
        {
            if (this.ActiveListings.Contains(mission))
            {
                this.ActiveListings.Remove(mission);
            }
            if (this.AcceptedMissions.Contains(mission))
            {
                this.AcceptedMissions.Remove(mission);
            }

            mission.Status = MissionTemplate.MissionStatus.REJECTED;
            //Remove the listing from the UI after 10
            Task.Run(delegate
            {
                System.Threading.Thread.Sleep(10000);
                Global.App.Dispatcher.Invoke((Action)delegate
                {
                    Global.IRCWindow.RemoveJobListing(mission);
                });
            });
        }
    }
}