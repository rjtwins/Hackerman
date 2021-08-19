using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public static class MissionDictionaryParser
    {
        public static Dictionary<MissionType, List<string>> TypeGiverListDict = new Dictionary<MissionType, List<string>>();
        public static Dictionary<MissionType, List<string>> TypeSequenceListDict = new Dictionary<MissionType, List<string>>();
    }
}
