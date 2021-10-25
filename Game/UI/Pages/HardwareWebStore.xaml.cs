using Game.Model;
using Newtonsoft.Json;
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
    /// Interaction logic for HardwareWebStore.xaml
    /// </summary>
    public partial class HardwareWebStore : DisplayablePage, INotifyPropertyChanged
    {
        #region Backing fiels
        //private List<Hardware> cpuList = new();
        //private List<Hardware> ramList = new();
        //private List<Hardware> hddList = new();
        //private List<Hardware> modumList = new();
        private Hardware hardwareSelected;
        #endregion

        #region Properties
        public List<Hardware> CPUList
        {
            get { return UTILS.CPU; }
            set 
            { 
                UTILS.CPU = value;
                OnPropertyChanged("CPUList");
            }
        }
        public List<Hardware> RAMList
        {
            get { return UTILS.RAM; }
            set
            {
                UTILS.RAM = value;
                OnPropertyChanged("RAMList");
            }
        }
        public List<Hardware> HDDList
        {
            get { return UTILS.HDD; }
            set
            {
                UTILS.HDD = value;
                OnPropertyChanged("HDDList");

            }
        }
        public List<Hardware> ModumList
        {
            get { return UTILS.MODEM; }
            set
            {
                UTILS.MODEM = value;
                OnPropertyChanged("ModumList");
            }
        }
        public Hardware HardwareSelected
        {
            get
            {
                return hardwareSelected;
            }
            set
            {
                hardwareSelected = value;
                OnPropertyChanged("HardwareSelected");
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public HardwareWebStore()
        {
            this.DataContext = this;
            InitializeComponent();
        }

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


        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new PopUpWindow("Buy?", $"Confirm purchage of {HardwareSelected.Name}.", this, true, PurchageConfirmation);
        }

        public void PurchageConfirmation(bool yesNo)
        {
            if (yesNo)
            {
                
                return;
            }
            //dont buy
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
