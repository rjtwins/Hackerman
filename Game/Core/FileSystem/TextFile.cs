using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Game.Core.FileSystem
{
    public class TextFile : Program
    {
        private List<string> textLines = new List<string>();

        [JsonIgnore]
        public List<string> TextLines { get => textLines; set => textLines = value; }

        public TextFile(string name) : base(name, false)
        {

        }

        [JsonConstructor]
        public TextFile() : base(null, false)
        {

        }

        public override string ToString()
        {
            return string.Join('\n', TextLines);
        }
    }
}