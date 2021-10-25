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

        public static void StaticActiveTraceCaught(object[] args)
        {
            Consequences.Instance.ActiveTraceCaught(args);
        }

        public void ActiveTraceCaught(object[] args)
        {
            System.Media.SystemSounds.Asterisk.Play();
        }
    }
}