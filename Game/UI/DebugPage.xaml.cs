using Game.Core.Endpoints;
using System.Windows;
using System.Windows.Controls;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for DebugPage.xaml
    /// </summary>
    public partial class DebugPage : Page
    {
        public DebugPage()
        {
            InitializeComponent();
        }

        private void ActiveTraceStart_Click(object sender, RoutedEventArgs e)
        {
            Global.ActiveTraceTracker.StartTrace(1);
        }

        private void ActiveTraceStop_Click(object sender, RoutedEventArgs e)
        {
            Global.ActiveTraceTracker.StopTrace();
        }

        private void PassiveTraceStart_Click(object sender, RoutedEventArgs e)
        {
            Endpoint A = Global.Bounce.BounceList[Global.Bounce.BounceList.Count - 1];
            Endpoint B = Global.Bounce.BounceList[Global.Bounce.BounceList.Count - 2];
            Global.PassiveTraceTracker.StartTrace(A, B, A.ConnectionLog[A.ConnectionLog.Count - 1].TimeStamp);
        }
    }
}