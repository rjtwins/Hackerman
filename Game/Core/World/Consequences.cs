namespace Game.Core.World
{
    internal class Consequences
    {
        private static readonly Consequences instance = new Consequences();

        public static Consequences Instance
        {
            get
            {
                return instance;
            }
            set
            {
            }
        }

        public void ActiveTraceCaught(params object[] arguments)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}