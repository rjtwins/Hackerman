using Game.Core.FileSystem;

namespace Game.Core.Console.LocalPrograms
{
    internal class HashLookup
    {
        private static readonly HashLookup instance = new HashLookup();

        public static HashLookup Instance
        {
            get
            {
                return instance;
            }
            private set
            {
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>string result</returns>
        public string DecodeTraffic(string filePath)
        {
            TrafficFile t = null;
            t = GetFileFromFilePath(filePath);
            if (t == null)
            {
                return "File not found.";
            }
            return DecodeTraffic(t);
        }

        //TODO replace all split file path stuff with this
        private static TrafficFile GetFileFromFilePath(string filePath)
        {
            TrafficFile t;
            string[] splitFilePath = filePath.Split('\\');
            if (splitFilePath.Length == 1)
            {
                t = (TrafficFile)Global.LocalEndpoint.GetFile(null, filePath);
            }
            else
            {
                string path = string.Join("\\", splitFilePath[0..(splitFilePath.Length - 2)]);
                string fileName = splitFilePath[splitFilePath.Length - 1];
                t = (TrafficFile)Global.LocalEndpoint.GetFile(path, fileName);
            }
            return t;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="t"></param>
        /// <returns>string result</returns>
        public string DecodeTraffic(TrafficFile t)
        {
            int decodedLines = t.TryCrack(((int)LocalSystem.Intance.HashLookup));
            return decodedLines.ToString() + " lines decoded.";
        }
    }
}