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
    /// Interaction logic for SplashPage2.xaml
    /// </summary>
    public partial class SplashPage2 : Page
    {
        public SplashPage2()
        {
            InitializeComponent();
            this.Loaded += SplashPage2_Loaded;
        }

        private void SplashPage2_Loaded(object sender, RoutedEventArgs e)
        {
            PlayStartUp();
        }

        public void PlayStartUp()
        {
            Task.Factory.StartNew(() => { ActualPlayStartUp(); });
        }

        private void ActualPlayStartUp()
        {
            System.Threading.Thread.Sleep(500);
            Global.App.Dispatcher.Invoke(() => { Stage1.Visibility = Visibility.Visible; });
            
            System.Threading.Thread.Sleep(250);
            Global.App.Dispatcher.Invoke(() => { Stage1.Text = Stage1.Text + " ."; });
           
            System.Threading.Thread.Sleep(250);
            Global.App.Dispatcher.Invoke(() => { Stage1.Text = Stage1.Text + "."; });
            
            System.Threading.Thread.Sleep(250);
            Global.App.Dispatcher.Invoke(() => { Stage1.Text = Stage1.Text + "."; });
            
            System.Threading.Thread.Sleep(250);
            Global.App.Dispatcher.Invoke(() => { Stage1.Text = Stage1.Text + "."; });
            
            System.Threading.Thread.Sleep(250);
            Global.App.Dispatcher.Invoke(() => { Stage2.Visibility = Visibility.Visible; });
            
        }
    }
}
