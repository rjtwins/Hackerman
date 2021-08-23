using Game.Core.Dialog;
using Game.Core.Endpoints;
using Game.Core.Mission.MissionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    /// <summary>
    /// For generating random missions with parameters
    /// </summary>
    public static class MissionGenerator
    {
        private static Random Rand = new Random();
        public static MissionTemplate GenerateMission(int dificulty)
        {
            MissionType[] temp = MissionTemplate.DifficultyMissionTypeDict[dificulty];
            return MissionGenerator.GenerateMission(temp[Rand.Next(temp.Length)], dificulty);
        }

        //TODO finish this:
        //TODO realistic moneys
        public static MissionTemplate GenerateMission(MissionType missionType, int dificulty)
        {
            MissionTemplate m  = null;
            switch (missionType)
            {
                case MissionType.STEAL:
                    m = new StealMission(1);
                    break;
                case MissionType.STEALMULTIPLE:
                    m = new StealMission(Rand.Next(2,5));
                    break;
                case MissionType.STEALALL:
                    break;
                case MissionType.STEALCRID:
                    break;
                case MissionType.MOVECOIN:
                    break;
                case MissionType.DELETE:
                    break;
                case MissionType.DELETEMULTIPLE:
                    break;
                case MissionType.DELETEALL:
                    break;
                case MissionType.UPLOAD:
                    m = new UploadMission(1);
                    break;
                case MissionType.UPLOADMULTIPLE:
                    m = new UploadMission(Rand.Next(2, 5));
                    break;
                case MissionType.UPLOADRUN:
                    break;
                case MissionType.DESTROY:
                    break;
                case MissionType.FRAME:
                    break;
                case MissionType.TRACE:
                    break;
                case MissionType.CHANGEINFO:
                    break;
                default:
                    throw new Exception("Mission type provided in GenerateMission(MissionType missionType, int dificulty) was not a valid mission type");
            }

            //Make this more consise 
            m.MissionType = missionType;
            m.Difficulty = dificulty;
            m.Contact = PickRandomContact(missionType, dificulty);
            m.Target = PickRandomContact(missionType, dificulty);
            m.TargetEndpoint = PickRandomEndpoint(missionType, dificulty);
            m.Reward = Rand.Next(Math.Max(dificulty * 1000 - 1000, 0), dificulty * 1000 + 1000);
            m.RepReward = dificulty;
            m.MissionChannel = UTILS.GenerateRandomString(7);
            m.Status = MissionTemplate.MissionStatus.ONOFFER;
            GenericMissionDialogResolver dialogResolver = new GenericMissionDialogResolver(m.MissionChannel);
            dialogResolver.SelectSequenceFromMissionDictionaries(PickRandomSequeceWithMissionType(missionType));
            IRCChannel iRCChannel = new IRCChannel(m.MissionChannel, dialogResolver);
            iRCChannel.Mission = m;
            m.DialogResolver = dialogResolver;
            m.IRCChannel = iRCChannel;
            m.DialogResolver.Mission = m;
            m.Setup();

            Global.IRCWindow.AddJobListing(m);
            return m;
        }

        private static Endpoint PickRandomEndpoint(MissionType missionType, int dificulty)
        {
            //For now just pick any random endpoint
            return UTILS.PickRandomEndpoint();
        }

        private static string PickRandomContact(MissionType missionType, int dificulty)
        {
            //TODO: do something with diffuculty.
            //TODO: do something with missiontype.
            return MissionDictionaries.SmallMissionGivers[Rand.Next(MissionDictionaries.SmallMissionGivers.Count)];
        }

        private static string PickRandomSequeceWithMissionType(MissionType m)
        {
            var options = MissionDictionaries.TypeSequenceListDict[m];
            return options[Rand.Next(options.Count)];
        }
    }
}
