using Game.Core.FileSystem;
using Game.Core.Mission.MissionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission.MissionTypes
{
    public class StealMission : MissionTemplate
    {
        List<Program> FilesToSteal = new List<Program>();
        string AllFilesString = string.Empty;
        public StealMission(int nrOfFiles) : base()
        {
            for (int i = 0; i < nrOfFiles; i++)
            {
                string fileName = UTILS.PickRandomFileName();
                this.FilesToSteal.Add(new Program(fileName, false));
                this.AllFilesString += fileName + "\n";
            }
            //Remove the trailing newline
            AllFilesString.Remove(AllFilesString.Length - 3, 2);
        }

        public override void Setup()
        {
            foreach(Program p in this.FilesToSteal)
            {
                TargetEndpoint.UploadFileToo(@"root\system", p, false);
            }
            this.DialogResolver.SetFilesToSteal(AllFilesString);
        }

        public override void RemoveMission()
        {
            foreach(Program p in this.FilesToSteal)
            {
                TargetEndpoint.RemoveFileFrom(@"root\system", p, false);
            }
        }

        internal override bool CheckFileNeeded(string v)
        {
            foreach (var file in this.FilesToSteal)
            {
                if(file.Name == v)
                {
                    return true;
                }
            }
            return false;
        }

        internal override void FileUploaded(string v)
        {
            List<Program> stolen = new List<Program>();
            foreach (var file in this.FilesToSteal)
            {
                if (file.Name == v)
                {
                    stolen.Add(file);
                }
            }
            stolen.ForEach(x => FilesToSteal.Remove(x));
            if(FilesToSteal.Count == 0)
            {
                this.DialogResolver.MissionCompleted();
                this.Status = MissionStatus.COMPLETED;
            }
        }

        public override bool CheckMissionCompleted()
        {
            if(this.Status == MissionStatus.COMPLETED)
            {
                return true;
            }
            return false;
        }

        internal override bool GetFile()
        {
            //There are not files to get from the client
            return false;
        }
    }
}
