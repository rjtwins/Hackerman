using Game.Core.Console;
using Game.Core.Dialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for IRC.xaml
    /// </summary>
    public partial class IRC : Page , INotifyPropertyChanged
    {
        public string Prefix { set; get; }
        private ObservableCollection<string> iRCOutput = new ObservableCollection<string>();

        public IRCChannel CurrentChannel;
        public Dictionary<string, IRCChannel> IDChannelDict = new Dictionary<string, IRCChannel>();
        Dictionary<string, Func<string, string>> CommandDict = new Dictionary<string, Func<string, string>>();

        public IRC()
        {
            this.CommandDict["join"] = SetChannel;
            this.CommandDict["leave"] = LeaveChannel;
            InitializeComponent();
            Loaded += PageLoaded;
            DataContext = this;
            //IRCContent.ConsoleOutput.Add("Remote Console [Version 11.0.19042.1110]");
            //IRCContent.ConsoleOutput.Add("(c) TracON LLC All Rights Reserved\n");
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            SetChannel(AddBaseChannel().ChannelName);
            this.IRCPrefix = Global.CurrentUserName;
            InputBlock.KeyDown += IRC_KeyDown;
            InputBlock.Focus();
            AddChannel("missions");
            AddMessage("missions", "test123");

            AddChannel("Grery");
            AddChannel("Bob");
            AddChannel("Joe");
            AddChannel("Alice");

            AddHiddenChannel("Hidden123");

            IRCChannel BobsChannel = IDChannelDict["Bob"];
            BobsChannel.DialogResolver = new DialogResolver("example", BobsChannel.ChannelName);
            IDChannelDict["Bob"] = BobsChannel;
            BobsChannel.DialogResolver.StartDialog();
        }


        public void AddHiddenChannel(string channelName)
        {
            TextBlock Channel = new TextBlock();
            Channel.Text = channelName;
            IRCChannel IRCChannel = new IRCChannel()
            {
                ChannelName = channelName,
                Messages = new List<string>(),
                ChannelNameTextBlock = Channel
            };
            this.IDChannelDict.Add(channelName, IRCChannel);
            //this.Channels.Children.Add(Channel);
        }

        public void AddMessageFromThread(string channelName, string message)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    AddMessage(channelName, message);
                });

            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                //Do nothing
            }
        }

        public void AddMessage(string channelName, string message)
        {
            if(!IDChannelDict.TryGetValue(channelName, out IRCChannel toWriteTo))
            {
                return;
            }
            if (this.CurrentChannel.ChannelName == toWriteTo.ChannelName)
            {
                IRCOutput.Add(message);
                return;
            }
            toWriteTo.Messages.Add(message);
            toWriteTo.ChannelNameTextBlock.Foreground = Brushes.Red;
            toWriteTo.ChannelNameTextBlock.Background = Brushes.Black;
        }

        public IRCChannel GetCurrentChannel()
        {
            return this.CurrentChannel;
        }

        public string GetCurrentChannelName()
        {
            return this.CurrentChannel.ChannelName;
        }

        private void IRC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (InputBlock.Text == "\n" || string.IsNullOrEmpty(InputBlock.Text) || string.IsNullOrWhiteSpace(InputBlock.Text))
                {
                    InputBlock.Text = string.Empty;
                    return;
                }
                string result = Parse(InputBlock.Text);
                if(result != string.Empty)
                {
                    this.IRCOutput.Add(result);
                }
                this.InputBlock.Text = string.Empty;
                InputBlock.Focus();
                Scroller.ScrollToBottom();
            }
        }

        public string SetChannel(string ChannelName)
        {
            //Save current convo
            if(CurrentChannel.Messages != null)
            {
                this.CurrentChannel.Messages = IRCOutput.ToList<string>();
                this.IDChannelDict[CurrentChannel.ChannelName] = this.CurrentChannel;
            }
            if(this.CurrentChannel.ChannelNameTextBlock != null)
            {
                this.CurrentChannel.ChannelNameTextBlock.Background = Brushes.Black;
                this.CurrentChannel.ChannelNameTextBlock.Foreground = Brushes.White;
            }

            //Get channel and port messages back
            if (!this.IDChannelDict.TryGetValue(ChannelName, out IRCChannel iRCChannel))
            {
                return "Channel not found.\n";
            }
            this.CurrentChannel = iRCChannel;
            this.IRCOutput.Clear();
            this.IRCOutput = new ObservableCollection<string>(CurrentChannel.Messages);

            if (!this.Channels.Children.Contains(iRCChannel.ChannelNameTextBlock)
                && iRCChannel.ChannelNameTextBlock != null)
            {
                this.Channels.Children.Add(iRCChannel.ChannelNameTextBlock);
            }

            if(CurrentChannel.ChannelNameTextBlock != null)
            {
                CurrentChannel.ChannelNameTextBlock.Background = Brushes.White;
                CurrentChannel.ChannelNameTextBlock.Foreground = Brushes.Black;
            }

            return string.Empty; /*"Channel "+ "\"" + ChannelName + "\" joined.\n";*/
        }

        public IRCChannel AddBaseChannel()
        {
            IRCChannel IRCChannel = new IRCChannel()
            {
                ChannelName = "Lobby",
                Messages = new List<string>(),
                ChannelNameTextBlock = null
            };
            this.IDChannelDict.Add("Lobby", IRCChannel);
            return IRCChannel;
        }

        public string AddChannel(string channelName)
        {
            TextBlock Channel = new TextBlock();
            Channel.Text = channelName;
            IRCChannel IRCChannel = new IRCChannel()
            {
                ChannelName = channelName,
                Messages = new List<string>(),
                ChannelNameTextBlock = Channel
            };
            this.IDChannelDict.Add(channelName, IRCChannel);
            this.Channels.Children.Add(Channel);
            return channelName;
        }

        public string LeaveChannel(string input)
        {
            if(CurrentChannel.ChannelName == "Lobby")
            {
                return string.Empty;
            }
            return SetChannel("Lobby");
        }

        private string Parse(string text)
        {
            string result = string.Empty;

            if(int.TryParse(text, out int choise))
            {
                CurrentChannel.DialogResolver.Next(choise);
                return string.Empty;
            }


            var SplitText = text.Split(' ');
            string commandBody = string.Empty;
            if(SplitText.Length == 0)
            {
                return result;
            }
            if(!CommandDict.TryGetValue(SplitText[0], out Func<string , string> toRun))
            {
                return "Command " + "\" " + SplitText[0] + "\" not recognized.\n";
            }
            if(SplitText.Length > 1)
            {
                commandBody = SplitText[1];
            }
            return toRun(commandBody);
        }

        private void InputBlock_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //Do nothing for now
        }

        public string IRCPrefix
        {
            get
            {
                return Prefix + ">";
            }
            set
            {
                Prefix = value;
                OnPropertyChanged("IRCPrefix");
            }
        }

        public ObservableCollection<string> IRCOutput
        {
            get
            {
                return iRCOutput;
            }
            set
            {
                iRCOutput = value;
                OnPropertyChanged("IRCOutput");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public struct IRCChannel
        {
            public string ChannelName;
            public List<string> Messages;
            public TextBlock ChannelNameTextBlock;
            public DialogResolver DialogResolver { get; set; }
        }
    }
}
