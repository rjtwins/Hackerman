using Game.Core.Console.RemotePrograms;
using Game.Core.FileSystem;
using Game.Core.UIPrograms;
using Game.Model;
using Newtonsoft.Json;
using System;

namespace Game.Core.Endpoints
{
    public class LocalEndpoint : Endpoint
    {

        //Ugly but needed to have the json converter use this instead of the other constructor.
        [JsonConstructor]
        public LocalEndpoint(object o = null)
        {

        }

        //this is the players endpoint
        public LocalEndpoint() : base(new Person(), EndpointType.PLAYER)
        {
            Global.LocalEndpoint = this;
            IsLocalEndpoint = true;
            this.AccessLevel = AccessLevel.ROOT;
            this.FileSystem.CurrentFolder = this.FileSystem;
            this.FileSystem.AllFolders.Clear();
            this.FileSystem.Folders.Clear();
            this.FileSystem.AddProgram(new Router());
            this.FileSystem.AddProgram(new TrafficListener());
            this.FileSystem.AddProgram(new MemoryScraper());
            this.FileSystem.AddProgram(new MBanking());
            this.FileSystem.AddProgram(new Worm("worm4_router4.exe", SoftwareLevel.LVL4, typeof(Router), SoftwareLevel.LVL4, true, true));
            this.FileSystem.AddProgram(new Worm("worm4_trafficListner4.exe", SoftwareLevel.LVL4, typeof(TrafficListener), SoftwareLevel.LVL4, true, true));

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
        internal void AddMemoryScraperTraffic(Traffic traffic)
        {
            String fileName = "ScraperData" + Global.GameTime.ToString("yy-MM-dd");
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