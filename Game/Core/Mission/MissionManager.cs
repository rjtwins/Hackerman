using Game.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public class MissionManager
    {
        public List<Mission> ActiveListings = new List<Mission>();
        public List<Mission> AcceptedMissions = new List<Mission>();
        private readonly Random Rand = new Random();

        public MissionManager()
        {

        }

        public void AcceptMission(Mission mission)
        {
            this.ActiveListings.Remove(mission);
            this.AcceptedMissions.Add(mission);
            mission.Status = Mission.MissionStatus.ACCEPTED;
        }

        public void EvaluateMissionListings()
        {
            List<Mission> toRemove = new List<Mission>();
            foreach(Mission m in this.ActiveListings)
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

        private static bool Mission48HoursOld(Mission m)
        {
            return Global.GameTime > m.ListingDate.AddHours(48);
        }

        internal void RejectMission(Mission mission)
        {
            if (this.ActiveListings.Contains(mission))
            {
                this.ActiveListings.Remove(mission);
            }
            if (this.AcceptedMissions.Contains(mission))
            {
                this.AcceptedMissions.Remove(mission);
            }

            mission.Status = Mission.MissionStatus.REJECTED;
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