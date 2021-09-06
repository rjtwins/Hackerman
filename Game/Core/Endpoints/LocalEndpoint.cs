using Game.Core.Console.RemotePrograms;
using Game.Core.FileSystem;
using Game.Core.UIPrograms;
using Game.Model;
using System;

namespace Game.Core.Endpoints
{
    public class LocalEndpoint : Endpoint
    {
        //this is the players endpoint
        public LocalEndpoint() : base(new Person(), EndpointType.PLAYER)
        {
            Global.LocalEndpoint = this;
            IsLocalEndpoint = true;
            this.AccessLevel = AccessLevel.ROOT;
            this.FileSystem.CurrentFolder = this.FileSystem;
            this.FileSystem.AllFolders.Clear();
            this.FileSystem.Folders.Clear();
            //this.FileSystem.AddProgram(new TextFile("BOUNCE.exe"));
            this.FileSystem.AddProgram(new Router());
            this.FileSystem.AddProgram(new TrafficListener());
            this.FileSystem.AddProgram(new MemoryScraper());
            this.FileSystem.AddProgram(new MBanking());
            this.FileSystem.AddProgram(new Worm("worm.exe", SoftwareLevel.LVL4, typeof(Router)));

        }

        internal void AddListnerTraffic(Traffic traffic)
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