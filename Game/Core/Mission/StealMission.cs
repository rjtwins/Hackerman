using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission
{
    public class StealMission : Mission
    {
        
        List<Program> FilesToSteal = new List<Program>();
        public StealMission(int nrOfFiles) : base()
        {
            for (int i = 0; i < nrOfFiles; i++)
            {
                this.FilesToSteal.Add(new Program(UTILS.PickRandomFileName(), false));
            }
        }

        public override void Setup()
        {
            foreach(Program p in this.FilesToSteal)
            {
                TargetEndpoint.UploadFileToo(@"root\system", p, false);
            }
        }

        public override bool CheckMissionCompleted()
        {
            return false;
        }

        public override void RemoveMission()
        {
            foreach(Program p in this.FilesToSteal)
            {
                TargetEndpoint.RemoveFileFrom(@"root\system", p, false);
            }
        }
    }
}
