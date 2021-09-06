using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Game.UI.Pages
{
    public abstract class DisplayablePage : Page
    {
        public BitmapImage Icon;

        public bool HasClose { get; internal set; } = true;
        public bool HasMax { get; internal set; } = true;
        public bool HasMin { get; internal set; } = true;

        public abstract void Close();
        public abstract void Open();

        public void RemoteLost()
        {
            new PopUpWindow("Remote Lost", "Connection to remote lost closing program.", this);
        }
    }
}