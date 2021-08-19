using Game.Core.Endpoints;
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
            CommandDictionary["bounce"] = this.ParseBounceCommand;
            CommandDictionary["cat"] = this.Concatenate;
            //CommandDictionary["cd"] = this.CurrentDirectory;
            CommandDictionary["help"] = this.PrintHelp;
            CommandDictionary["cls"] = this.ClearConsole;
            CommandDictionary["clear"] = this.ClearConsole;
            //CommandDictionary["exit"] = this.ExitConsole;
            CommandDictionary["ls"] = this.List;
            CommandDictionary["dir"] = this.List;
            CommandDictionary["cat"] = this.Concatenate;
            CommandDictionary["run"] = this.RunProgram;
            CommandDictionary["upload"] = this.Upload;
            CommandDictionary["download"] = this.Download;
            CommandDictionary["delete"] = this.Delete;
        }

        private void Delete(string obj)
        {
            throw new NotImplementedException();
        }

        //private void MakeConnection(string obj)
        //{
        //    (Endpoint from, Endpoint too, bool Succes) =  Global.Bounce.MakeConnection();
        //    if (!Succes)
        //    {
        //        return;
        //    }
        //    AttachSystem(from, too);
        //}

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
            ConsoleContent.ConsoleOutput.Add(Global.StartEndPoint.ListFromCurrentFolder());
        }

        //TODO: fix error when running exit command when not connected to a system
        //private void ExitConsole(string obj)
        //{
        //    if(AttachedSystem != null)
        //    {
        //        AttachedSystem.Discconect();
        //        ConsoleContent.ConsoleOutput.Add("Disconnected\n");
        //    }
        //    ConsoleContent.ConsolePrefix = "";
        //    this.GivingUsername = false;
        //    this.GivingPassword = false;
        //    this.AttachedSystem = null;
        //    Global.EndPointMap.UnmakeConnection();
        //}

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
            throw new NotImplementedException();
        }

        private void Concatenate(string commandBody)
        {
            string result = "";
            result = Global.StartEndPoint.TryPrintFile(commandBody);
            ConsoleContent.ConsoleOutput.Add(result);
            return;
        }

        //private void CurrentDirectory(string commandBody)
        //{
        //    if (string.IsNullOrWhiteSpace(commandBody))
        //    {
        //        return;
        //    }

        //    string result;
        //    try
        //    {
        //        result = Global.StartEndPoint.NavigateTo(commandBody);
        //        ConsoleContent.ConsolePrefix = result;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        result = ex.Message;
        //    }
        //    ConsoleContent.ConsoleOutput.Add(result);
        //}

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