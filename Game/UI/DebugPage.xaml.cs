using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            Global.ActiveTraceTracker.StartTrace();
        }

        private void ActiveTraceStop_Click(object sender, RoutedEventArgs e)
        {
            Global.ActiveTraceTracker.StopTrace();
        }
    }
}
