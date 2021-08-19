using Game.Core.Endpoints;
using Game.Core.FileSystem;
using Game.UI;
using System;
using System.Collections.Generic;

namespace Game.Core.Console
{
    public class RemoteCommandParser : ICommandParser
    {
        private ConsoleContent ConsoleContent;
        public Endpoint AttachedSystem { private set; get; }
        private Endpoint ConnectingFrom;
        public Dictionary<string, Action<string>> CommandDictionary = new Dictionary<string, Action<string>>();
        private string username;
        private string password;

        public bool GivingUsername { get; private set; } = false;
        public bool GivingPassword { get; private set; } = false;

        public RemoteCommandParser(ConsoleContent c)
        {
            this.ConsoleContent = c;
            //this.AttachSystem(new Endpoint());
            FillCommandDictionary();
        }

        private void FillCommandDictionary()
        {
            CommandDictionary["cat"] = this.Concatenate;
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
            CommandDictionary["delete"] = this.Delete;
        }

        private void Delete(string obj)
        {
            throw new NotImplementedException();
        }

        private void MakeConnection()
        {
            (Endpoint from, Endpoint too, bool Succes) =  Global.Bounce.MakeConnection();
            if (!Succes)
            {
                return;
            }
            AttachSystem(from, too);
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
            string result = string.Empty;
            string[] splitCommand = commandBody.Split(" ");

            
            Program P = Global.StartEndPoint.GetFile("root", splitCommand[0]);
            if (P == null)
            {
                ConsoleContent.ConsoleOutput.Add("\"" + splitCommand[0] + "\"" + " cannot be found on the local machine.\n");
                return;
            }

            try
            {
                Global.RemoteSystem.UploadFileToo(splitCommand[1], P);
            }
            catch (Exception ex)
            {
                ConsoleContent.ConsoleOutput.Add(ex.Message);
            }
        }

        public void ParseCommand(string command, string prefix)
        {
            if (command == "exit")
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
            if (splitCommand.Count == 2)
            {
                commandBody = splitCommand[1];
            }

            else if (splitCommand.Count > 2)
            {
                commandBody = string.Join(' ', splitCommand.GetRange(1, splitCommand.Count - 1));
            }



            if (this.AttachedSystem == null && commandType != "connect")
            {
                ConsoleContent.ConsoleOutput.Add("Unable to excute: " + commandType + " on remote: \"NONE\"\n");
                return;
            }
            if(this.AttachedSystem == null && commandType == "connect")
            {
                MakeConnection();
                return;
            }

            //we are not connected to anything
            if (CommandDictionary.ContainsKey(commandType))
            {
                CommandDictionary[commandType](commandBody);
                return;
            }

            ConsoleContent.ConsoleOutput.Add("\"" + command + "\"" + " is not recognized as an internal or external command or operable program.\n");
            if (this.AttachedSystem == null)
            {
                ConsoleContent.ConsoleOutput.Add("If you are trying to execute a remote command, please make sure you are connected to a remote machine first.\n");
                return;
            }
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
            if(AttachedSystem != null)
            {
                AttachedSystem.Discconect();
                ConsoleContent.ConsoleOutput.Add("Disconnected\n");
            }
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

        public void AttachSystem(Endpoint from, Endpoint too)
        {
            this.ConnectingFrom = from;
            this.AttachedSystem = too;
            Global.RemoteSystem = too;
            this.GivingUsername = true;
            this.ConsoleContent.ConsoleOutput.Add("Connected to: " + too.IPAddress + "\nPlease input username and password.\n");
            this.ConsoleContent.ConsolePrefix = "LOGIN USERNAME:";
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