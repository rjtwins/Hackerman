﻿using Game.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console.LocalPrograms
{
    class HashLookup
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

        int level = 1;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>string result</returns>
        public string DecodeTraffic(string filePath)
        {
            TrafficFile t = null;
            string[] splitFilePath = filePath.Split('\\');
            if(splitFilePath.Length == 1)
            {
                t = (TrafficFile)Global.LocalEndpoint.GetFile(null, filePath);
            }
            else
            {
                string path = string.Join("\\", splitFilePath[0..(splitFilePath.Length - 2)]);
                string fileName = splitFilePath[splitFilePath.Length - 1];
                t = (TrafficFile)Global.LocalEndpoint.GetFile(path, fileName);
            }
            if(t == null)
            {
                return "File not found.";
            }
            return DecodeTraffic(t);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns>string result</returns>
        public string DecodeTraffic(TrafficFile t)
        {
            int decodedLines = t.TryCrack(this.level);
            return decodedLines.ToString() + " lines decoded.";
        }
    }
}
