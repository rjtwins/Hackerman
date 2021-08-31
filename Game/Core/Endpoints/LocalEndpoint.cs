using Game.Core.Console.RemotePrograms;
using Game.Core.FileSystem;
using System;
using static Game.UTILS;

namespace Game.Core.Endpoints
{
    public class LocalEndpoint : Endpoint
    {
        //TODO: Modify so this is the users system
        //this is the players endpoint
        public LocalEndpoint() : base(new Person(), EndpointType.PLAYER)
        {
            Global.LocalEndpoint = this;
            IsLocalEndpoint = true;
            this.AccessLevel = AccessLevel.ROOT;
            this.FileSystem.CurrentFolder = this.FileSystem;
            this.FileSystem.AllFolders.Clear();
            this.FileSystem.Folders.Clear();
            this.FileSystem.AddProgram(new TextFile("BOUNCE.exe"));
            
            this.FileSystem.AddProgram(new TrafficListener());
            this.FileSystem.AddProgram(new MemoryScraper());
        }

        internal void AddListnerTraffic(Console.Traffic traffic)
        {
            String fileName = "ListnerData" + Global.GameTime.ToString("yy-MM-dd");
            TrafficFile dataFile = (TrafficFile)this.FileSystem.GetFileFromPath(@"root\data", fileName, null);

            if (dataFile == null)
            {
                Folder dataFolder;
                try
                {
                    dataFolder = this.FileSystem.GetFolderFromPath(@"root\data");
                }
                catch (Exception)
                {

                    dataFolder = this.FileSystem.MakeFolderFromPath(@"root\data");
                }
                dataFile = (TrafficFile)dataFolder.AddProgram(new TrafficFile(fileName));
            }
            dataFile.AddTraffic(traffic);
        }
    }
}