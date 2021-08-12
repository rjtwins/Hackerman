namespace Game.Core.Endpoints
{
    public class LocalEndpoint : Endpoint
    {
        //this is the players endpoint
        public LocalEndpoint() : base()
        {
            IsLocalEndpoint = true;
        }
    }
}