using Game.Core.Endpoints;
using Game.Model;
using System;
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
        //private List<string> banks = new List<string>();
        //public List<string> Banks
        //{
        //    get { return banks; }
        //    set
        //    {
        //        if (value != banks)
        //        {
        //            banks = value;
        //            OnPropertyChanged("Banks");
        //        }
        //    }
        //}

        private string selectedBank = string.Empty;

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

        private string password = string.Empty;

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

        private string username = string.Empty;

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

        private string amount = string.Empty;

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

        private string balance = string.Empty;

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

        private string loan = string.Empty;

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

        private string receiver = string.Empty;

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

        private BankEndpoint BankLoggedInto;

        public BankPage()
        {
            InitializeComponent();
            this.DataContext = this;
            this.BalanceTab.IsEnabled = false;
            this.TransferTab.IsEnabled = false;
            this.Icon = new BitmapImage(
                new Uri("pack://application:,,,/Game;component/Icons/OtherIcons/001-network.png"));
        }

        public string start()
        {
            this.BalanceTab.IsEnabled = false;
            this.TransferTab.IsEnabled = false;
            return "M-Banking started";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Person p = null;

            foreach (BankEndpoint b in Global.BankEndpoints)
            {
                if (b.Name != this.SelectedBank)
                {
                    continue;
                }
                p = b.LoginBankAcount(username, password);
                if (p == null)
                {
                    new PopUpWindow("Login error", "Invalid username password combination.", this);
                    return;
                }
                this.Balance = p.BankBalance.ToString();
                this.BalanceTab.IsEnabled = true;
                this.TransferTab.IsEnabled = true;
                this.BankLoggedInto = b;
                new PopUpWindow("Succes", "Login succesfull.", this);
                return;
            }
            new PopUpWindow("Bank not found", "Selected bank could not be reached.", this);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.Balance = string.Empty;
            this.BalanceTab.IsEnabled = false;
            this.TransferTab.IsEnabled = false;
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            string result = BankLoggedInto.TransferMoney(Username, Receiver, BankLoggedInto, int.Parse(Amount));
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
    }
}