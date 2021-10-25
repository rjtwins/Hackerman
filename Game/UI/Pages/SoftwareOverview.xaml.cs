using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for SoftwareOverview.xaml
    /// </summary>
    public partial class SoftwareOverview : DisplayablePage, INotifyPropertyChanged
    {
        public ObservableCollection<Software> SoftwareList { get; set; } = new();
        private Software selectedSoftware;
        public Software SelectedSoftware
        {
            get { return selectedSoftware; }
            set { selectedSoftware = value;
                OnPropertyChanged("SelectedSoftware");
            }
        }


        public SoftwareOverview()
        {
            InitializeComponent();
            SoftwareList.Add(new Software(null, "TEST123", "A testing piece of \n software"));
            SoftwareList.Add(new Software(null, "TEST456", "Another testing piece of \n software also"));
            SoftwareListView.ItemsSource = this.SoftwareList;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }

        public struct Software
        {
            public Software(ImageSource icon, string softwareName, string softwareDesc)
            {
                this.Icon = icon;
                this.SoftwareName = softwareName;
                this.SoftwareDesc = softwareDesc;
            }
            public ImageSource Icon { get; set; }
            public String SoftwareName { get; set; }
            public string SoftwareDesc { get; set; }
        }

        private void SoftwareListView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedSoftware = (Software)e.NewValue;
        }
    }
}
