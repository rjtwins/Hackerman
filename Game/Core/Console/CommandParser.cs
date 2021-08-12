using Game.Core.Endpoints;
using Game.UI;
using System;
using System.Collections.Generic;

namespace Game.Core.Console
{
    public class CommandParser
    {
        private ConsoleContent ConsoleContent;
        private Endpoint AttachedSystem;
        private Endpoint ConnectingFrom;
        public Dictionary<string, Action<string>> CommandDictionary = new Dictionary<string, Action<string>>();
        public Dictionary<string, Action<string>> LocalCommandDictionary = new Dictionary<string, Action<string>>();
        private string username;
        private string password;

        public bool GivingUsername { get; private set; } = false;
        public bool GivingPassword { get; private set; } = false;

        public CommandParser(ConsoleContent c)
        {
            this.ConsoleContent = c;
            //this.AttachSystem(new Endpoint());
            FillCommandDictionary();
        }

        private void FillCommandDictionary()
        {
            CommandDictionary["cat"] = this.Concatenate;
            CommandDictionary["copy"] = this.Copy;
            CommandDictionary["cd"] = this.CurrentDirectory;
            CommandDictionary["help"] = this.PrintHelp;
            CommandDictionary["cls"] = this.ClearConsole;
            CommandDictionary["clear"] = this.ClearConsole;
            CommandDictionary["exit"] = this.ExitConsole;
            CommandDictionary["ls"] = this.List;
            CommandDictionary["dir"] = this.List;
            CommandDictionary["cat"] = this.Concatenate;
            CommandDictionary["run"] = this.RunProgram;
            CommandDictionary["upload"] = this.Upload;
            CommandDictionary["download"] = this.Download;
        }

        private void FillLocalCommandDictionary()
        {
            LocalCommandDictionary["connect"] = this.TryConnectToAdress;
        }

        private void TryConnectToAdress(string obj)
        {
            throw new NotImplementedException();
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
            if(command == "exit")
            {
                CommandDictionary["exit"](command);
                return;
            }

            //We are looking for ausername and then a password
            if (this.GivingUsername)
            {
                HandleUsername(command);
                ConsoleContent.ConsolePrefix = "LOGIN PASSWORD:";
                return;
            }
            else if (this.GivingPassword)
            {
                HandlePassword(command);
                this.ConsoleContent.ConsoleOutput.RemoveAt(ConsoleContent.ConsoleOutput.Count - 2);
                string password = "";
                Array.ForEach<char>(command.ToCharArray(), x => password += "*");
                this.ConsoleContent.ConsoleOutput.Insert(ConsoleContent.ConsoleOutput.Count - 1, password);
                return;
            }


            //split at space
            List<string> splitCommand = new List<string>(command.Split(' '));
            string commandType = splitCommand[0];
            string commandBody = string.Empty;
            //If we have more then 3 sections we have need to add the other two together again.
            if(splitCommand.Count == 2)
            {
                commandBody = splitCommand[1];
            }
            else if (splitCommand.Count >= 3)
            {
                commandBody = string.Join(' ', splitCommand.GetRange(1, splitCommand.Count - 2));
            }

            if (this.AttachedSystem == null)
            {
                if (!LocalCommandDictionary.ContainsKey(commandType))
                {
                    ConsoleContent.ConsoleOutput.Add("\"" + command + "\"" + " is not recognized as an internal or external command or operable program.\n");
                    ConsoleContent.ConsoleOutput.Add("If you are trying to execute a remote command, please make sure you are connected to a remote machine first.\n");
                    return;
                }
                LocalCommandDictionary[commandType](commandBody);
                return;
            }

            //we are not connected to anything
            if (!CommandDictionary.ContainsKey(commandType))
            {
                ConsoleContent.ConsoleOutput.Add("\"" + command + "\"" + " is not recognized as an internal or external command or operable program.\n");
                return;
            }
            CommandDictionary[commandType](commandBody);
            return;
        }

        private void HandleUsername(string command)
        {
            try
            {
                this.username = command;
                this.GivingUsername = false;
                this.GivingPassword = true;
                return;
            }
            catch (Exception ex)
            {
                ConsoleContent.ConsoleOutput.Add(ex.Message); ;
            }
        }

        private void HandlePassword(string command)
        {
            try
            {
                this.password = command;
                string result = AttachedSystem.ConnectTo(this.username, this.password, this.ConnectingFrom);
                ConsoleContent.ConsoleOutput.Add(result);
                this.GivingPassword = false;
                ConsoleContent.ConsolePrefix = AttachedSystem.CurrentPath();
                return;
            }
            catch (Exception ex)
            {
                ConsoleContent.ConsoleOutput.Add(ex.Message);
                this.GivingUsername = true;
                this.GivingPassword = false;
                this.ConsoleContent.ConsolePrefix = "LOGIN USERNAME:";
            }

        }
        private void List(string obj)
        {
            ConsoleContent.ConsoleOutput.Add(AttachedSystem.ListFromCurrentFolder());
        }

        //TODO: fix error when running exit command when not connected to a system
        private void ExitConsole(string obj)
        {
            AttachedSystem.Discconect();
            ConsoleContent.ConsoleOutput.Add("Disconnected\n");
            ConsoleContent.ConsolePrefix = "";
            this.GivingUsername = false;
            this.GivingPassword = false;
            this.AttachedSystem = null;
            Global.EndPointMap.UnmakeConnection();
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

        }

        private void Concatenate(string commandBody)
        {
            string result = "";
            result = AttachedSystem.TryPrintFile(commandBody);
            ConsoleContent.ConsoleOutput.Add(result);
            return;
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
                result = AttachedSystem.NavigateTo(commandBody);
                ConsoleContent.ConsolePrefix = result;
            }
            catch (System.Exception ex)
            {
                result = ex.Message;
            }
            ConsoleContent.ConsoleOutput.Add(result);
        }

        public void AttachSystem(Endpoint from , Endpoint too)
        {
            this.ConnectingFrom = from;
            this.AttachedSystem = too;
            this.GivingUsername = true;
            this.ConsoleContent.ConsoleOutput.Add("Connected to: " + "TODO" + "\nPlease input username and password.");
            this.ConsoleContent.ConsolePrefix = "LOGIN USERNAME:";
        }

        internal void AttachConsole(ConsoleContent consoleContent)
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