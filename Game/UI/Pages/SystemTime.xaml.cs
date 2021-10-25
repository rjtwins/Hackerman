using Game.UI.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Game.UI.Pages
{
    /// <summary>
    /// Interaction logic for SystemTime.xaml
    /// </summary>
    public partial class SystemTime : DisplayablePage, INotifyPropertyChanged
    {
        private string _timeString;

        public string TimeString
        {
            get { return _timeString; }
            set 
            { 
                _timeString = value;
                OnPropertyChanged("TimeString");
            }
        }


        public SystemTime()
        {
            this.DataContext = this;
            InitializeComponent();
            this.HasMin = false;
            this.HasMax = false;
            this.HasClose = false;
            this.Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Icons/Win98Icons/SystemTimeIcon.png"));
        }

        private void GameSpeedButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            ContentControl parrent = radioButton.Parent as ContentControl;
            parrent.Style = FindResource("TaskBarButtonBoxInverted") as Style;
            switch (radioButton.Name)
            {
                case "PauseButton":
                    Global.EventTicker.StopTicker();
                    break;

                case "Speed1Button":
                    Global.EventTicker.ChangeSpeed(1);
                    break;

                case "Speed2Button":
                    Global.EventTicker.ChangeSpeed(2);
                    break;

                case "Speed3Button":
                    Global.EventTicker.ChangeSpeed(3);
                    break;

                case "Speed4Button":
                    Global.EventTicker.ChangeSpeed(4);
                    break;

                default:
                    break;
            }
        }

        private void GameSpeedButton_Unchecked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            ContentControl parrent = radioButton.Parent as ContentControl;
            parrent.Style = FindResource("TaskBarButtonBox") as Style;
        }
        public override void Open()
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        internal void UpdateTime(string text1)
        {
            TimeString = text1;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
