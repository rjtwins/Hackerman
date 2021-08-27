using Game.Core.FileSystem;
using static Game.UTILS;

namespace Game.Core.Endpoints
{
    public class LocalEndpoint : Endpoint
    {
        //TODO: Modify so this is the users system
        //this is the players endpoint
        public LocalEndpoint() : base(new Person(), EndpointType.PLAYER)
        {
            Global.StartEndPoint = this;
            IsLocalEndpoint = true;
            this.AccessLevel = AccessLevel.ROOT;
            this.FileSystem.CurrentFolder = this.FileSystem;
            this.FileSystem.AllFolders.Clear();
            this.FileSystem.Folders.Clear();
            this.FileSystem.AddProgram(new TextFile("BOUNCE.exe"));
            this.FileSystem.AddProgram(new TextFile("TEST1.info"));
            this.FileSystem.AddProgram(new TextFile("TEST2.info"));
            this.FileSystem.AddProgram(new TextFile("TEST3.info"));
        }
    }
}