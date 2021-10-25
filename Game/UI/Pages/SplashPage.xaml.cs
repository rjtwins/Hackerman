using Game.Core.Console;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class SplashPage : Page
    {
        //TODO supply total memory and compoments
        public long Memory = LocalSystem.Intance.GetDiskMemory();

        public SplashPage()
        {
            InitializeComponent();
            this.Loaded += SplashPage_Loaded;
        }

        private void SplashPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.CPUTextBlock1.Text = LocalSystem.Intance.ProcessorName + this.CPUTextBlock1.Text;
            this.CPUTextBlock2.Text = LocalSystem.Intance.ProcessorNameShort + " at " + LocalSystem.Intance.ProcessorSpeed + "MHz";
            PlayStartUp();
        }

        public void PlayStartUp()
        {
            Task.Factory.StartNew(() => { UpdateMemoryTest(); });
        }

        private void UpdateMemoryTest()
        {
            for (int i = 0; i < this.Memory; i += 34952)
            {
                this.MemoryTestTextBlock.Dispatcher.Invoke(() => { this.MemoryTestTextBlock.Text = i.ToString() + "K"; });
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