using System.Collections.Generic;

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
            return string.Join('\n', TextLines);
        }
    }
}