using Game.Core.Events;
using Game.Core.Mission.MissionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public class MissionManager
    {
        public List<MissionTemplate> ActiveListings = new List<MissionTemplate>();
        public List<MissionTemplate> AcceptedMissions = new List<MissionTemplate>();
        private readonly Random Rand = new Random();

        public MissionManager()
        {

        }

        public void AcceptMission(MissionTemplate mission)
        {
            this.ActiveListings.Remove(mission);
            this.AcceptedMissions.Add(mission);
            mission.Status = MissionTemplate.MissionStatus.ACCEPTED;
        }

        public void EvaluateMissionListings()
        {
            List<MissionTemplate> toRemove = new List<MissionTemplate>();
            foreach(MissionTemplate m in this.ActiveListings)
            {
                if (Mission48HoursOld(m))
                {
                    toRemove.Add(m);
                    Global.App.Dispatcher.Invoke((Action)delegate {
                        Global.IRCWindow.RemoveChannel(m.MissionChannel);
                    });
                }
            }
            ActiveListings = ActiveListings.Except(toRemove).ToList();

            if(ActiveListings.Count < 5)
            {
                //We must do this on the UI thread sinse we are eding UI elements,
                //TODO move the invoker to the actual UI
                Global.App.Dispatcher.Invoke((Action)delegate{
                    this.ActiveListings.Add(MissionGenerator.GenerateMission(this.Rand.Next(Math.Max(Global.GameState.Reputation - 2, 0), Global.GameState.Reputation)));
                    this.ActiveListings.Add(MissionGenerator.GenerateMission(this.Rand.Next(Math.Max(Global.GameState.Reputation - 2, 0), Global.GameState.Reputation)));
                });
            }

            EventBuilder.BuildEvent("MissionListingEval")
                .EventInterval(3600)
                .EventVoid(this.EvaluateMissionListings)
                .RegisterWithVoid();
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
            Task.Run(delegate {
                System.Threading.Thread.Sleep(10000);
                Global.App.Dispatcher.Invoke((Action)delegate {
                    Global.IRCWindow.RemoveJobListing(mission);
                });
            });
        }
    }
}