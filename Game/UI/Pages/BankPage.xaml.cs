using Game.Core.Endpoints;
using Game.Core.FileSystem;
using Game.Core.UIPrograms;
using Game.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Game.UI.Pages
{
    //TODO log from where we run this app

    /// <summary>
    /// Interaction logic for BankPage.xaml
    /// </summary>
    public partial class BankPage : DisplayablePage, INotifyPropertyChanged
    {
        #region Backingfields
        private string selectedBank = string.Empty;
        private string password = string.Empty;
        private string username = string.Empty;
        private string amount = string.Empty;
        private string balance = string.Empty;
        private string loan = string.Empty;
        private string receiver = string.Empty;
        private BankEndpoint bankLoggedInto;
        private string receiverBank;
        private List<BankEndpoint> bankEndpoints = new();
        #endregion

        #region Properties
        public string SelectedBank
        {
            get { return selectedBank; }
            set
            {
                if (value != selectedBank)
                {
                    selectedBank = value;
                    OnPropertyChanged("SelectedBank");
                }
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                if (value != password)
                {
                    password = value;
                    OnPropertyChanged("Password");
                }
            }
        }
        public string Username
        {
            get { return username; }
            set
            {
                if (value != username)
                {
                    username = value;
                    OnPropertyChanged("Username");
                }
            }
        }
        public string Amount
        {
            get { return amount; }
            set
            {
                if (value != amount)
                {
                    amount = value;
                    OnPropertyChanged("Amount");
                }
            }
        }
        public string Balance
        {
            get { return balance; }
            set
            {
                if (value != balance)
                {
                    balance = value;
                    OnPropertyChanged("Balance");
                }
            }
        }
        public string Loan
        {
            get { return loan; }
            set
            {
                if (value != loan)
                {
                    loan = value;
                    OnPropertyChanged("Loan");
                }
            }
        }
        public string Receiver
        {
            get { return receiver; }
            set
            {
                if (value != receiver)
                {
                    receiver = value;
                    OnPropertyChanged("Receiver");
                }
            }
        }
        public BankEndpoint BankLoggedInto
        {
            get { return bankLoggedInto; }
            set { bankLoggedInto = value; }
        }
        private MBanking Program { get; set; }
        public string ReceiverBank
        {
            get { return receiverBank; }
            set 
            {
                receiverBank = value;
                OnPropertyChanged("ReceiverBank");
            }
        }
        public List<BankEndpoint> BankEndpoints
        {
            get
            {
                this.bankEndpoints.Clear();
                Global.BankEndpoints.ToList().ForEach(x => this.bankEndpoints.Add((BankEndpoint)x));
                return bankEndpoints;
            }
            private set
            {
                bankEndpoints = value;
            }
        }
        #endregion

        public BankPage(MBanking program)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Program = program;
            this.BalanceTab.IsEnabled = false;
            this.TransferTab.IsEnabled = false;
            this.Icon = new BitmapImage(
                new Uri("pack://application:,,,/Game;component/Icons/OtherIcons/001-network.png"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if(!Program.Login(selectedBank, username, password, out Person p, out BankEndpoint bankLoggedInto,out string[] result))
            {
                new PopUpWindow(result[0], result[1], this);
                return;
            }
            this.BankLoggedInto = bankLoggedInto;
            this.Balance = p.BankBalance.ToString();
            this.BalanceTab.IsEnabled = true;
            this.TransferTab.IsEnabled = true;
            new PopUpWindow(result[0], result[1], this);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.Balance = string.Empty;
            this.BalanceTab.IsEnabled = false;
            this.TransferTab.IsEnabled = false;
            this.Program.Logout();
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            string result = this.Program.Transfer(this.Username, this.Receiver, this.ReceiverBank, Convert.ToInt32(this.Amount));
            new PopUpWindow("Transfer", result, this);
        }

        private void AmountTextBox_PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void AmountTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        private bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }
    }
}