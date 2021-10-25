using Game.Core;
using Game.Core.Console;
using Game.Core.Endpoints;
using Game.Core.FileSystem;
using Game.Core.Mission;
using Game.Core.Mission.MissionTypes;
using Game.Core.World;
using Game.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    class SaveLoad
    {
        private static readonly SaveLoad instance = new SaveLoad();
        private JsonSerializer Serializer = new JsonSerializer();

        private SaveData SaveData = new();
        private RefDict RefDict = new();

        public static SaveLoad Instance
        {
            get
            {
                return instance;
            }
            private set
            {

            }
        }

        private SaveLoad()
        {
            Serializer.TypeNameHandling = TypeNameHandling.All;
            Serializer.Formatting = Formatting.Indented;
            Serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //Serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
        }

        public void Save()
        {
            SafeInstanceDictionaries();
            //string json = JsonConvert.SerializeObject(this.SaveData);


            if (!Directory.Exists(Environment.CurrentDirectory + @"\Saves)"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\Saves");
            }
            using (FileStream file = new FileStream(Environment.CurrentDirectory + @"\Saves\save_data.json", FileMode.Create, System.IO.FileAccess.Write))
            {
                //BsonWriter bsonWriter = new BsonWriter(file);
                JsonWriter jsonWriter = new JsonTextWriter(new StreamWriter(file));
                Serializer.Serialize(jsonWriter, this.RefDict);
                jsonWriter.Flush();
                jsonWriter.Close();
            }

            using (FileStream file = new FileStream(Environment.CurrentDirectory + @"\Saves\save_world.json", FileMode.Create, System.IO.FileAccess.Write))
            {
                //BsonWriter bsonWriter = new BsonWriter(file);
                JsonWriter jsonWriter = new JsonTextWriter(new StreamWriter(file));
                Serializer.Serialize(jsonWriter, this.SaveData);
                jsonWriter.Flush();
                jsonWriter.Close();

                //Stream jsonStream = new MemoryStream();
                ////BsonWriter bsonWriter = new BsonWriter(file);
                //JsonWriter jsonWriter = new JsonTextWriter(new StreamWriter(jsonStream));
                //Serializer.Serialize(jsonWriter, this.SaveData);
                ////jsonWriter.Flush();
                //byte[] Compressed = SaveLoad.Compress(jsonStream);
                //file.Write(Compressed);
            }
        }


        public void Load()
        {
            Global.SerializingDictionaries = true;
            using (FileStream file = new FileStream(Environment.CurrentDirectory + @"\Saves\save_data.json", FileMode.Open, System.IO.FileAccess.Read))
            {
                //BsonReader bsonReader = new BsonReader(file);
                JsonReader jsonReader = new JsonTextReader(new StreamReader(file));
                this.RefDict = Serializer.Deserialize<RefDict>(jsonReader);
            }

            Global.AllEndpointsDict = RefDict.AllEndpointsDict;
            Global.AllPersonsDict = RefDict.AllPersonsDict;
            Global.AllProgramsDict = RefDict.AllProgramsDict;
            Global.AllFoldersDict = RefDict.AllFoldersDict;
            Global.AllMissionsDict = RefDict.AllMissionsDict;
            Global.AllIRCChannelsDict = RefDict.ALLIRCChannelsDict;

            using (FileStream file = new FileStream(Environment.CurrentDirectory + @"\Saves\save_world.json", FileMode.Open, System.IO.FileAccess.Read))
            {
                //BsonReader bsonReader = new BsonReader(file);
                JsonReader jsonReader = new JsonTextReader(new StreamReader(file));
                this.SaveData = Serializer.Deserialize<SaveData>(jsonReader);
            }

            Global.ByteMap = SaveData.WorldMap;

            Global.LocalPerson = Global.AllPersonsDict[SaveData.LocalPersonId];
            Global.LocalEndpoint = (LocalEndpoint)Global.AllEndpointsDict[SaveData.LocalEndpointId];

            Global.SerializingDictionaries = false;

            Global.AllEndpoints = SaveData.AllEndpoints;
            Global.CompanyEndpoints = SaveData.CompanyEndpoints;
            Global.EmployeEndpoints = SaveData.EmployeEndpoints;
            Global.PersonalEndpoints = SaveData.PersonalEndpoints;
            Global.WebServerEndpoints = SaveData.WebServerEndpoints;
            Global.BankEndpoints = SaveData.BankEndpoints;
            UTILS.PersonList = SaveData.PersonList;
            UTILS.Companies = SaveData.Companies;
            UTILS.BankList = SaveData.BankList;

            Global.AllEndpoints.SetReferenceDict(RefDict.AllEndpointsDict);
            Global.CompanyEndpoints.SetReferenceDict(RefDict.AllEndpointsDict);
            Global.EmployeEndpoints.SetReferenceDict(RefDict.AllEndpointsDict);
            Global.PersonalEndpoints.SetReferenceDict(RefDict.AllEndpointsDict);
            Global.WebServerEndpoints.SetReferenceDict(RefDict.AllEndpointsDict);
            Global.BankEndpoints.SetReferenceDict(RefDict.AllEndpointsDict);
            UTILS.PersonList.SetReferenceDict(RefDict.AllPersonsDict);
            UTILS.Companies.SetReferenceDict(RefDict.AllPersonsDict);
            UTILS.BankList.SetReferenceDict(RefDict.AllPersonsDict);
        }

        public void SafeInstanceDictionaries()
        {
            RefDict.AllPersonsDict = Global.AllPersonsDict;
            RefDict.AllEndpointsDict = Global.AllEndpointsDict;
            RefDict.AllFoldersDict = Global.AllFoldersDict;
            RefDict.AllProgramsDict = Global.AllProgramsDict;
            RefDict.AllMissionsDict = Global.AllMissionsDict;
            RefDict.ALLIRCChannelsDict = Global.AllIRCChannelsDict;

            SaveData.LocalEndpointId = Global.LocalEndpoint.Id;
            SaveData.LocalPersonId = Global.LocalPerson.Id;
            SaveData.LocalSystem = LocalSystem.Intance;
            SaveData.GameState = GameState.Instance;
            SaveData.PersonList = UTILS.PersonList;
            SaveData.Companies = UTILS.Companies;
            SaveData.BankList = UTILS.BankList;


            SaveData.AllEndpoints = Global.AllEndpoints;
            SaveData.CompanyEndpoints = Global.CompanyEndpoints;
            SaveData.EmployeEndpoints = Global.EmployeEndpoints;
            SaveData.PersonalEndpoints = Global.PersonalEndpoints;
            SaveData.WebServerEndpoints = Global.WebServerEndpoints;
            SaveData.BankEndpoints = Global.BankEndpoints;
            SaveData.WorldMap = Global.ByteMap;

            SaveData.ActiveTraceTracker = ActiveTraceTracker.Instance;
            SaveData.PassiveTraceTracker = PassiveTraceTracker.Instance;
            SaveData.MissionManager = MissionManager.Instance;
        }

    }
    public struct RefDict
    {
        
        //Instance Dictionaries
        public Dictionary<Guid, Person> AllPersonsDict { get; set; }
        public Dictionary<Guid, Endpoint> AllEndpointsDict { get; set; }
        public Dictionary<Guid, Folder> AllFoldersDict { get; set; }
        public Dictionary<Guid, Program> AllProgramsDict { get; set; }
        public Dictionary<Guid, MissionTemplate> AllMissionsDict { get; set; }
        public Dictionary<Guid, IRCChannel> ALLIRCChannelsDict { get; set; }
    }

    public struct SaveData
    {
        //Player information
        public Guid LocalPersonId { get; set; }
        public Guid LocalEndpointId { get; set; }
        public LocalSystem LocalSystem { get; set; }
        public GameState GameState { get; set; }


        //Reference Lists
        public ReferenceList<Endpoint> AllEndpoints { get; set; }
        public ReferenceList<Endpoint> CompanyEndpoints { get; set; }
        public ReferenceList<Endpoint> EmployeEndpoints { get; set; }
        public ReferenceList<Endpoint> PersonalEndpoints { get; set; }
        public ReferenceList<Endpoint> WebServerEndpoints { get; set; }
        public ReferenceList<Endpoint> BankEndpoints { get; set; }
        //Utils lists
        public ReferenceList<Person> PersonList { get; set; }
        public ReferenceList<Person> Companies { get; set; }
        public ReferenceList<Person> BankList { get; set; }

        //Map
        public byte[,] WorldMap { get; set; }

        //Trace trackers
        public ActiveTraceTracker ActiveTraceTracker { get; set; }
        public PassiveTraceTracker PassiveTraceTracker { get; set; }

        //Missions
        public MissionManager MissionManager { get; set; }


    }
}
