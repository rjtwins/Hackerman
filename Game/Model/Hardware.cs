using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Game.Model
{
    public class Hardware
    {
        [JsonIgnore]
        public BitmapImage Image
        {
            get
            {
                return new BitmapImage(new Uri(@"pack://application:,,,/Icons/Win98Icons/" + ImageSource));
            }
        }
        public HardwareType HardwareType { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string ImageSource { get; set; }
        public int Price { get; set; }

        public Hardware()
        {
            HardwareType = HardwareType.CPU;
            ShortName = string.Empty;
            Name = string.Empty;
            Desc = string.Empty;
            ImageSource = "Computer.png";
        }
    }

    public enum HardwareType
    {
        CPU,
        RAM,
        MEMORY,
        MODUM
    }
}
