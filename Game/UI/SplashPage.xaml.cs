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
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class SplashPage : Page
    {
        //TODO supply total memory and compoments
        public int Memory = 1048576;
        public SplashPage()
        {
            InitializeComponent();
            this.Loaded += SplashPage_Loaded;
        }

        private void SplashPage_Loaded(object sender, RoutedEventArgs e)
        {
            PlayStartUp();
        }

        public void PlayStartUp()
        {
            Task.Factory.StartNew(() => { UpdateMemoryTest(); });
        }

        private void UpdateMemoryTest()
        {
            for (int i = 0; i < this.Memory; i+=34952)
            {
                this.MemoryTestTextBlock.Dispatcher.Invoke(() => { this.MemoryTestTextBlock.Text = i.ToString()+"K"; });
                System.Threading.Thread.Sleep(100);
            }
            this.MemoryTestTextBlock.Dispatcher.Invoke(() => { this.MemoryTestTextBlock.Text = this.Memory.ToString() + "K OK"; });
            System.Threading.Thread.Sleep(600);
            this.MemoryTestTextBlock.Dispatcher.Invoke(() => { this.Stage1.Visibility = Visibility.Visible; });
            this.MemoryTestTextBlock.Dispatcher.Invoke(() => { this.Stage2.Visibility = Visibility.Visible; });
            System.Threading.Thread.Sleep(700);
            this.MemoryTestTextBlock.Dispatcher.Invoke(() => { this.Stage3.Visibility = Visibility.Visible; });
            System.Threading.Thread.Sleep(500);
            this.MemoryTestTextBlock.Dispatcher.Invoke(() => { this.Stage4.Visibility = Visibility.Visible; });
        }
    }
}
