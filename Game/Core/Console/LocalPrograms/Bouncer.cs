using Game.Core.Endpoints;
using System;
using System.Collections.Generic;

namespace Game.Core.Console.LocalPrograms
{
    public class Bouncer
    {
        private Dictionary<string, Func<string, string>> CommandDictionary = new Dictionary<string, Func<string, string>>();

        public Bouncer()
        {
            FillCommands();
        }

        public void FillCommands()
        {
            this.CommandDictionary["LOAD"] = LoadBounceList;
            this.CommandDictionary["SAVE"] = SaveBounceList;
            this.CommandDictionary["HELP"] = PrintHelp;
            this.CommandDictionary["LIST"] = ListSavedBounceLists;
            this.CommandDictionary["SHOW"] = PrintBounceList;
            this.CommandDictionary["CLEAR"] = ClearCurrentBounceList;
            this.CommandDictionary["CLR"] = ClearCurrentBounceList;
            this.CommandDictionary["DELETE"] = DeleteBonceList;
        }

        private string DeleteBonceList(string commandBody)
        {
            (bool check, string result) = CheckNameParameter(commandBody);
            if (!check)
            {
                return result;
            }
            if (!Global.LocalSystem.RemoveBounceList(commandBody))
            {
                return "KEY " + "\"" + commandBody + "\"" + " NOT FOUND";
            }
            return "\"" + commandBody + "\"" + " REMOVED";
        }

        private string ClearCurrentBounceList(string arg)
        {
            Global.Bounce.BounceList.Clear();
            Global.EndPointMap.DrawBouncePath();
            return "NODES CLEARED\n";
        }

        private string PrintBounceList(string commandBody)
        {
            string result = "ADDRESS\t\t\tNODE NR:\n";
            Endpoint[] bounceLict = null;
            try
            {
                bounceLict = Global.LocalSystem.LoadBounceList(commandBody);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            for (int i = 0; i < bounceLict.Length; i++)
            {
                result += bounceLict[i].IPAddress + "\t\t\t" + i + "\n";
            }
            return result;
        }

        private string ListSavedBounceLists(string arg)
        {
            string result = "KEY:\t\t\tNODES:\n";
            Dictionary<string, Endpoint[]> BounceLists = Global.LocalSystem.GetSavedBouncelists();
            foreach (KeyValuePair<string, Endpoint[]> entry in BounceLists)
            {
                result += entry.Key + "\t\t\t" + entry.Value.Length + "\n";
            }
            return result;
        }

        private string PrintHelp(string obj)
        {
            return string.Join(Environment.NewLine, CommandDictionary.Keys);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="command"></param>
        /// <returns>string result</returns>
        public string ParseCommand(string command)
        {
            List<string> splitCommand = new List<string>(command.Split(' '));
            string commandType = splitCommand[0];
            string commandBody = string.Empty;
            //If we have more then 3 sections we have need to add the other two together again.
            if (splitCommand.Count == 2)
            {
                commandBody = splitCommand[1];
            }
            else if (splitCommand.Count >= 3)
            {
                commandBody = string.Join(' ', splitCommand.GetRange(1, splitCommand.Count - 1));
            }

            if (!CommandDictionary.ContainsKey(commandType))
            {
                return "\"" + command + "\"" + " COMMAND NOT RECOGNIZED BY BOUNCE.\n";
            }
            return CommandDictionary[commandType](commandBody);
        }

        private (bool, string) CheckNameParameter(string nameString)
        {
            if (string.IsNullOrWhiteSpace(nameString) || string.IsNullOrEmpty(nameString))
            {
                return (false, "MISSING PARAMETER \"NAME\"\n");
            }
            if (nameString.Split(' ').Length != 1)
            {
                return (false, "ILLEGAL CHARACTER IN PARAMETER \"NAME\"\n");
            }
            return (true, nameString);
        }

        private string SaveBounceList(string commandBody)
        {
            (bool check, string result) = CheckNameParameter(commandBody);
            if (!check)
            {
                return result;
            }
            Global.LocalSystem.SaveCurrentBounceListsAs(commandBody);
            return "SAVED: " + commandBody;
        }

        private string LoadBounceList(string commandBody)
        {
            Endpoint[] bounceLict = Global.LocalSystem.LoadBounceList(commandBody);
            Global.Bounce.BounceList.Clear();
            Global.Bounce.BounceList.AddRange(bounceLict);
            Global.EndPointMap.DrawBouncePath();
            return commandBody + " loaded.";
        }
    }
}