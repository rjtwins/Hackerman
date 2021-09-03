using System.Windows;
using System.Windows.Controls;

namespace Game.UI.Pages
{
    /// <summary>
    /// Interaction logic for IPDailer.xaml
    /// </summary>
    public partial class IPDailer : DisplayablePage
    {
        public IPDailer()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            string input = btn.Content.ToString();
            IPTextBox.Text += input;
        }

        private void DailButton_Click(object sender, RoutedEventArgs e)
        {
            string ip = IPTextBox.Text;
        }
    }
}