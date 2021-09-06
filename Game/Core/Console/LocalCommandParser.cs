using Game.Core.Console.LocalPrograms;
using Game.UI;
using System;
using System.Collections.Generic;

namespace Game.Core.Console
{
    public class LocalCommandParser : ICommandParser
    {
        private ConsoleContent ConsoleContent;
        public Dictionary<string, Action<string>> CommandDictionary = new Dictionary<string, Action<string>>();

        public bool GivingUsername { get; private set; } = false;
        public bool GivingPassword { get; private set; } = false;

        public LocalCommandParser(ConsoleContent c)
        {
            this.ConsoleContent = c;
            FillCommandDictionary();
        }

        private void FillCommandDictionary()
        {
            CommandDictionary["decoder"] = this.RunHashLookup;
            CommandDictionary["bounce"] = this.ParseBounceCommand;
            CommandDictionary["cat"] = this.Concatenate;
            CommandDictionary["cd"] = this.CurrentDirectory;
            CommandDictionary["help"] = this.PrintHelp;
            CommandDictionary["cls"] = this.ClearConsole;
            CommandDictionary["clear"] = this.ClearConsole;
            //CommandDictionary["exit"] = this.ExitConsole;
            CommandDictionary["ls"] = this.List;
            CommandDictionary["dir"] = this.List;
            CommandDictionary["cat"] = this.Concatenate;
            CommandDictionary["run"] = this.RunProgram;
            CommandDictionary["delete"] = this.Delete;
            CommandDictionary["DictHack"] = this.DictHack;
        }

        private void RunHashLookup(string obj)
        {
            ConsoleContent.ConsoleOutput.Add(HashLookup.Instance.DecodeTraffic(obj));
        }

        private void DictHack(string userName)
        {
            ConsoleContent.ConsoleOutput.Add(DictionaryHack.Instance.StartHack(userName));
        }

        private void Delete(string obj)
        {
            throw new NotImplementedException();
        }

        private void CurrentDirectory(string commandBody)
        {
            if (string.IsNullOrWhiteSpace(commandBody))
            {
                return;
            }

            string result;
            try
            {
                result = Global.LocalEndpoint.NavigateTo(commandBody);
                ConsoleContent.ConsolePrefix = result;
            }
            catch (System.Exception ex)
            {
                result = ex.Message;
            }
            ConsoleContent.ConsoleOutput.Add(result);
        }

        private void ParseBounceCommand(string commandBody)
        {
            ConsoleContent.ConsoleOutput.Add(Global.LocalSystem.Bouncer.ParseCommand(commandBody));
        }

        private void Download(string commandBody)
        {
            throw new NotImplementedException();
        }

        private void Upload(string commandBody)
        {
            throw new NotImplementedException();
        }

        public void ParseCommand(string command, string prefix)
        {
            //split at space
            List<string> splitCommand = new List<string>(command.Split(' '));
            string commandType = splitCommand[0];
            string commandBody = string.Empty;
            //If we have more then 3 sections we have need to add the other two together again.
            if (splitCommand.Count == 2)
            {
                commandBody = splitCommand[1];
            }
            else if (splitCommand.Count > 2)
            {
                commandBody = string.Join(' ', splitCommand.GetRange(1, splitCommand.Count - 1));
            }

            if (CommandDictionary.ContainsKey(commandType))
            {
                CommandDictionary[commandType](commandBody);
                return;
            }

            ConsoleContent.ConsoleOutput.Add("\"" + command + "\"" + " is not recognized as an internal or external command or operable program.\n");
        }

        private void List(string obj)
        {
            ConsoleContent.ConsoleOutput.Add(Global.LocalEndpoint.ListFromCurrentFolder());
        }

        private void ClearConsole(string obj)
        {
            ConsoleContent.ConsoleOutput.Clear();
        }

        private void Copy(string commandBody)
        {
            throw new NotImplementedException();
        }

        private void RunProgram(string commmandBody)
        {
            if (string.IsNullOrEmpty(commmandBody))
            {
                ConsoleContent.ConsoleOutput.Add("Invalid number of arguments");
            }
            string result = Global.LocalEndpoint.RunProgram(commmandBody);
            ConsoleContent.ConsoleOutput.Add(result);
        }

        private void Concatenate(string commandBody)
        {
            string result = "";
            result = Global.LocalEndpoint.TryPrintFile(commandBody);
            ConsoleContent.ConsoleOutput.Add(result);
            return;
        }

        public void AttachConsole(ConsoleContent consoleContent)
        {
            this.ConsoleContent = consoleContent;
        }

        private void PrintHelp(string commandBody)
        {
            string result = string.Join('\n', CommandDictionary.Keys);
            ConsoleContent.ConsoleOutput.Add(result);
        }
    }
}