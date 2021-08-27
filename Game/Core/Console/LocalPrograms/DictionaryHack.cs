﻿using System;
using System.Threading.Tasks;

namespace Game.Core.Console.LocalPrograms
{
    //TODO: Add software level
    //TODO: Password dificulty
    public class DictionaryHack
    {
        public bool Running { get; private set; }
        private static readonly DictionaryHack instance = new DictionaryHack();
        private string PasswordToGet = string.Empty;
        private double HackTime = 100; // in attempts before getting the password
        private bool Stop = false;
        

        public static DictionaryHack Instance
        {
            get
            {
                return instance;
            }
            set
            {
            }
        }

        private DictionaryHack()
        {
            Global.RemoteConsole.CommandParser.OnDisconnected += CommandParser_OnDisconnected;
        }

        private void CommandParser_OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            this.Stop = true;
        }

        public string StartHack(string userName)
        {
            this.Stop = false;
            if (this.Running)
            {
                return null;
            }
            if (Global.RemoteSystem == null)
            {
                return "No remote connection.";
            }
            if (!RemoteCommandParser.Instance.GivingPassword && !RemoteCommandParser.Instance.GivingUsername)
            {
                return "Unable to attach to login.";
            }
            string PossiblePassword = Global.RemoteSystem.GetPassword(userName);
            if (PossiblePassword != string.Empty)
            {
                this.PasswordToGet = PossiblePassword;
            }
            Global.RemoteSystem.UnderDictHack();

            this.Running = true;
            Task.Factory.StartNew(() => this.HackOnRemote(new object[] { userName, 0 }));
            return "Starting dictionary attack";
        }

        private void HackOnRemote(object[] arguments)
        {
            string userName = (string)arguments[0];
            int index = (int)arguments[1];

            //TODO: Base hacktime on difficulty

            //If we have a non exsistant username we should never get the password.
            if (this.PasswordToGet == string.Empty)
            {
                this.HackTime = UTILS.PasswordList.Count;
            }

            this.HackTime = Math.Min(HackTime, UTILS.PasswordList.Count);
            string resultString = string.Empty;

            for (index = 0; index < UTILS.PasswordList.Count; index++)
            {
                while (Global.GamePaused)
                {
                    System.Threading.Thread.Sleep(100);
                }

                if (Global.StopCurrentProgram || this.Stop)
                {
                    PasswordToGet = string.Empty;
                    Global.StopCurrentProgram = false;
                    this.Running = false;
                    return;
                }

                string randomPassword = UTILS.GetPasswordByIndex(index);

                if (index > HackTime ||
                    PasswordToGet == randomPassword ||
                    (!RemoteCommandParser.Instance.GivingPassword && !RemoteCommandParser.Instance.GivingUsername))
                {
                    HackSuccesfull(userName);
                    this.PasswordToGet = string.Empty;
                    this.Running = false;
                    return;
                }

                resultString += userName + Environment.NewLine
                    + new string('*', randomPassword.Length) + Environment.NewLine
                    + "Username password combination not found.\n";

                Global.RemoteSystem.LogInToo(userName, randomPassword, Global.Bounce.BounceList[Global.Bounce.BounceList.Count - 1], true);

                switch (Global.EventTicker.GameSpeed)
                {
                    case 1:
                        UpdateUI(resultString);
                        resultString = string.Empty;
                        break;

                    case 2:
                        if (index % 2 == 0)
                        {
                            UpdateUI(resultString);
                            resultString = string.Empty;
                        }
                        break;

                    case 3:
                        if (index % 20 == 0)
                        {
                            UpdateUI(resultString);
                            resultString = string.Empty;
                        }
                        break;

                    case 4:
                        if (index % 100 == 0)
                        {
                            UpdateUI(resultString);
                            resultString = string.Empty;
                        }
                        break;

                    default:
                        break;
                }

                Global.EventTicker.SleepSeconds(1);
            }
            this.PasswordToGet = string.Empty;
            this.Running = false;
        }

        private void HackSuccesfull(string userName)
        {
            RemoteCommandParser RCP = RemoteCommandParser.Instance;

            if (Global.RemoteSystem == null)
            {
                return;
            }
            else if (RCP.GivingPassword)
            {
                //Give password
                Global.App.Dispatcher.Invoke(() =>
                {
                    //Password
                    Global.RemoteConsole.ConsoleContent.RunExternalCommand(this.PasswordToGet);
                    Global.RemoteConsole.ExternalAfterAddAction();
                    Global.RemoteConsole.ShowExternalInput(this.PasswordToGet);
                    Global.RemoteConsole.ClearInputBlock();
                    //Display on local
                    Global.LocalConsole.AddLine("USERNAME: " + userName);
                    Global.LocalConsole.AddLine("PASSWORD: " + this.PasswordToGet);
                });
            }
            else //Give Username and password
            {
                Global.App.Dispatcher.Invoke(() =>
                {
                    //Username
                    Global.RemoteConsole.ConsoleContent.RunExternalCommand(userName);
                    Global.RemoteConsole.ExternalAfterAddAction();
                    Global.RemoteConsole.ShowExternalInput(userName);
                    //Password
                    Global.RemoteConsole.ConsoleContent.RunExternalCommand(this.PasswordToGet);
                    Global.RemoteConsole.ExternalAfterAddAction();
                    Global.RemoteConsole.ShowExternalInput(this.PasswordToGet);
                    Global.RemoteConsole.ClearInputBlock();
                    //Display on local
                    Global.LocalConsole.AddLine("USERNAME: " + userName);
                    Global.LocalConsole.AddLine("PASSWORD: " + this.PasswordToGet);
                });
            }
        }

        public void UpdateUI(string resultString)
        {
            Global.App.Dispatcher.Invoke(() =>
            {
                Global.RemoteConsole.AddOutput(resultString);
            });
        }
    }
}