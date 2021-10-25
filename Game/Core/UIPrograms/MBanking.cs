using Game.Core.Endpoints;
using Game.Core.FileSystem;
using Game.Model;
using Game.UI;
using Game.UI.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.UIPrograms
{
    public class MBanking : Program
    {
        private BankPage _bankPage;
        public BankPage BankPage
        {
            get { return _bankPage; }
            set { _bankPage = value; }
        }

        public BankEndpoint BankLoggedInto { get; private set; }
        public Person User { get; set; }

        public MBanking() : base("M-Banking.exe", true)
        {

        }

        public override string RunProgram(Endpoint ranOn)
        {
            base.RunProgram(ranOn);
            this.BankPage = new BankPage(this);
            this.ProgramWindow = new ProgramWindow(this.BankPage);
            this.ProgramWindow.OnClosed += ProgramWindow_OnClosed;
            return "M-Banking started";
        }

        private void ProgramWindow_OnClosed(object sender, WindowClosedEventArgs e)
        {
            this.StopProgram();
        }

        public override void StopProgram(bool remoteLost = false)
        {
            if (remoteLost)
            {
                this.BankPage.RemoteLost();
            }
            base.StopProgram();
        }

        internal bool Login(string selectedBank, string username, string password, out Person p, out BankEndpoint bankLoggedInto, out string[] result)
        {
            p = null;
            bankLoggedInto = null;
            result = new string[2];
            foreach (BankEndpoint b in Global.BankEndpoints)
            {
                if (b.Name != selectedBank)
                {
                    continue;
                }
                p = b.LoginBankAcount(username, password);
                this.User = p;
                if (p == null)
                {
                    result[0] = "Login error";
                    result[1] = "Invalid username password combination.";
                    return false;
                }
                this.BankLoggedInto = b;
                bankLoggedInto = b;
                result[0] = "Succes";
                result[1] = "Login succesfull.";
                return true;
            }
            result[0] = "Bank not found";
            result[1] = "Selected bank could not be reached.";
            return false;
        }

        internal void Logout()
        {
            this.User = null;
            this.BankLoggedInto = null;
        }

        internal string Transfer(string from, string too, BankEndpoint tooBank, int amount)
        {
            if(BankLoggedInto.TransferMoney(from, too, tooBank, amount, out string result))
            {
                //super ugly this should not be here and in the method above.
                Person personToo = null;
                foreach (Person p in tooBank.Clients)
                {
                    if (p.Name == too)
                    {
                        personToo = p;
                        break;
                    }
                }

                this.RanOn.SystemLog.Add(LogItemBuilder.Builder()
                    .CREDITTRANSFER(this.User, BankLoggedInto, personToo, tooBank, amount)
                    .From(RanOn.ConnectedFrom)
                    .User(RanOn.CurrentUsername)
                    .AccesLevel(RanOn.AccessLevel)
                    .TimeStamp(Global.GameTime));
            }
            return result;
        }

        internal string Transfer(string from, string too, string receiverBank, int amount)
        {
            BankEndpoint recieverBankEndpoint = null;
            foreach (BankEndpoint b in Global.BankEndpoints)
            {
                if (b.Name != receiverBank)
                {
                    continue;
                }
                recieverBankEndpoint = b;
                return Transfer(from, too, recieverBankEndpoint, amount);
            }
            return "Receiving bank not found.";
        }
    }
}
