using Game.Core.Console;
using System.ComponentModel;

namespace Game.UI.Pages
{
    /// <summary>
    /// Interaction logic for SystemOverview.xaml
    /// </summary>
    public partial class SystemOverview : DisplayablePage, INotifyPropertyChanged
    {
        #region BackingFields
        private string cpuName;
        private string cpuStats;
        private string ramName;
        private string ramStats;
        private string modumName;
        private string modumStats;
        private string memoryName;
        private string memoryStats;
        #endregion

        #region Properties
        public string CPUName
        {
            get { return cpuName; }
            set { cpuName = value;
                OnPropertyChanged("CPUName");
            }
        }
        public string CPUStats
        {
            get { return cpuStats; }
            set { cpuStats = value;
                OnPropertyChanged("CPUStats");
            }
        }
        public string RAMName
        {
            get { return ramName; }
            set { ramName = value;
                OnPropertyChanged("RAMName");
            }
        }
        public string RAMStats
        {
            get { return ramStats; }
            set { ramStats = value;
                OnPropertyChanged("RAMStats");
            }
        }
        public string ModumName
        {
            get { return modumName; }
            set { modumName = value;
                OnPropertyChanged("ModumName");
            }
        }
        public string ModumStats
        {
            get { return modumStats; }
            set { modumStats = value;
                OnPropertyChanged("ModumStats");
            }
        }
        public string MemoryName
        {
            get { return memoryName; }
            set { memoryName = value;
                OnPropertyChanged("MemoryName");
            }
        }
        public string MemoryStats
        {
            get { return memoryStats; }
            set 
            {
                memoryStats = value;
                OnPropertyChanged("MemoryStats");
            }
        }
        #endregion

        public SystemOverview()
        {
            this.DataContext = this;
            InitializeComponent();
            CPUName = LocalSystem.Intance.ProcessorNameShort;
            CPUStats = LocalSystem.Intance.ProcessorSpeed.ToString() + "MHz";
            RAMName = LocalSystem.Intance.RAMName;
            RAMStats = LocalSystem.Intance.RAMCapacity.ToString() + "MB";
            MemoryName = LocalSystem.Intance.MemoryDiskName;
            MemoryStats = (LocalSystem.Intance.DiskMemory/1024).ToString() + "MB";
            ModumName = LocalSystem.Intance.ModumName;
            ModumStats = LocalSystem.Intance.ModumSpeed.ToString() + "Mb";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override void Close()
        {
            throw new System.NotImplementedException();
        }

        public override void Open()
        {
            throw new System.NotImplementedException();
        }
    }
}