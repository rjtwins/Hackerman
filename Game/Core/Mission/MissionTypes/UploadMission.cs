using Game.Core.FileSystem;
using Game.Model;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Game.Core.Mission.MissionTypes
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UploadMission : MissionTemplate
    {
        [JsonProperty]
        private ReferenceList<Program> filesToUpload = new (Global.AllProgramsDict, "AllProgramsDict");
        [JsonProperty]
        private string filePathForUpload = string.Empty;
        [JsonProperty]
        private bool filesGoten = false;

        public ReferenceList<Program> FilesToUpload { get => filesToUpload; set => filesToUpload = value; }
        public string FilePathForUpload { get => filePathForUpload; set => filePathForUpload = value; }
        public bool FilesGoten { get => filesGoten; set => filesGoten = value; }
        
        [JsonConstructor]
        public UploadMission()
        {

        }

        public UploadMission(int nrOfFiles) : base()
        {
            //TODO: Base file path for upload on dificulty
            FilePathForUpload = "root";
            for (int i = 0; i < nrOfFiles; i++)
            {
                FilesToUpload.Add(new Program(UTILS.PickRandomFileName()));
            }
            if (nrOfFiles > 1)
            {
                this.MissionType = MissionType.UPLOADMULTIPLE;
                return;
            }
            this.MissionType = MissionType.UPLOAD;
        }

        public override bool CheckMissionCompleted()
        {
            foreach (Program p in this.FilesToUpload)
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
                Global.LocalEndpoint.UploadFileToo(null, file);
            }
            return true;
        }
    }
}