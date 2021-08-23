using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Mission.MissionTypes
{
    class UploadMission : MissionTemplate
    {
        private List<Program> FilesToUpload = new List<Program>();
        private string FilePathForUpload = string.Empty;
        bool FilesGoten = false;
        public UploadMission(int nrOfFiles)
        {
            //TODO: Base file path for upload on dificulty
            FilePathForUpload = "root";
            for (int i = 0; i < nrOfFiles; i++)
            {
                FilesToUpload.Add(new Program(UTILS.PickRandomFileName()));
            }
            if(nrOfFiles > 1)
            {
                this.MissionType = MissionType.UPLOADMULTIPLE;
                return;
            }
            this.MissionType = MissionType.UPLOAD;
        }
        public override bool CheckMissionCompleted()
        {
            foreach(Program p in this.FilesToUpload)
            {
                if (this.TargetEndpoint.GetFile(FilePathForUpload, p.Name) == null)
                {
                    return false;
                }
            }
            return true;
        }

        public override void RemoveMission()
        {
            //We don't need to remove any overhead for this mission type.
        }

        public override void Setup()
        {
            //No in world setup required.
        }

        internal override bool CheckFileNeeded(string v)
        {
            return false;
        }

        internal override void FileUploaded(string v)
        {
            //This is for when we DCC Send a file to a client not when we upload a file to a server.
        }

        internal override bool GetFile()
        {
            if (FilesGoten)
            {
                return false;
            }
            foreach (var file in this.FilesToUpload)
            {
                Global.StartEndPoint.UploadFileToo(null, file);
            }
            return true;
        }
    }
}
