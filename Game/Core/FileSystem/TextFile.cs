using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.FileSystem
{
    public class TextFile : Program
    {
        public List<string> TextLines = new List<string>();
        public TextFile(string name) : base(name, false)
        {

        }

        public override string ToString()
        {
            return string.Join('\n',TextLines);
        }
    }
}
