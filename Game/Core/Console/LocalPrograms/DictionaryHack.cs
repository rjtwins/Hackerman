using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Console.LocalPrograms
{
    public class DictionaryHack
    {
        private static readonly DictionaryHack instance = new DictionaryHack();
        private string PasswordToGet = string.Empty;
        private int HackTime = 100;
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

        public string StartHack(string userName)
        {
            if(Global.RemoteSystem == null)
            {
                return "No remote connection.";
            }
            if(!RemoteCommandParser.Instance.GivingPassword && !RemoteCommandParser.Instance.GivingUsername)
            {
                return "Unable to attach to login.";
            }
            if(Global.RemoteSystem.UsernamePasswordDict.TryGetValue(userName, out string PossiblePassword))
            {
                this.PasswordToGet = PossiblePassword;
            }
            Task.Factory.StartNew(() => this.HackOnRemote(userName));
            return "Starting dictionary attack";
        }

        private void HackOnRemote(string userName)
        {
            //TODO: Base hacktime on difficulty

            //If we have a non exsistant username we should never get the password.
            if (this.PasswordToGet == string.Empty)
            {
                this.HackTime = UTILS.PasswordList.Count;
            }
            this.HackTime = Math.Min(HackTime, UTILS.PasswordList.Count);
            RemoteCommandParser RCP = RemoteCommandParser.Instance;
            Debug.WriteLine("Starting dict hack");
            Debug.WriteLine(this.PasswordToGet);
            Debug.WriteLine(this.HackTime);

            System.Threading.Thread.Sleep(500);

            for (int i = 0; i < this.HackTime; i++)
            {
                if (Global.RemoteSystem == null)
                {
                    return;
                }
                if (!RemoteCommandParser.Instance.GivingPassword && !RemoteCommandParser.Instance.GivingUsername)
                {
                    //SUCCES!
                    break;
                }
                else if (RCP.GivingPassword)
                {
                    string randomPassword = UTILS.GetPasswordByIndex(i);
                    Global.App.Dispatcher.Invoke(() => {
                        Global.RemoteConsole.ConsoleContent.RunExternalCommand(randomPassword);
                        Global.RemoteConsole.ExternalAfterAddAction();
                        Global.RemoteConsole.ShowExternalInput(randomPassword);
                    });
                }
                else //Giving username
                {
                    Global.App.Dispatcher.Invoke(() => {
                        Global.RemoteConsole.ConsoleContent.RunExternalCommand(userName);
                        Global.RemoteConsole.ExternalAfterAddAction();
                        Global.RemoteConsole.ShowExternalInput(userName);
                    });
                }
                System.Threading.Thread.Sleep(100);
            }
            HackSuccesfull(userName);
            this.PasswordToGet = string.Empty;
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
                string randomPassword = UTILS.PickrandomPassword();
                Global.App.Dispatcher.Invoke(() => {
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
                Global.App.Dispatcher.Invoke(() => {
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
    }
}
