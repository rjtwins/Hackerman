using Game.Core.Console;
using Game.Core.Dialog;
using Game.Core.Mission;
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
        //private ObservableCollection<StackPanel> iRCOutput = new ObservableCollection<StackPanel>();

        public IRCChannel CurrentChannel;
        public Dictionary<string, IRCChannel> IDChannelDict = new Dictionary<string, IRCChannel>();
        Dictionary<string, Func<string, string>> CommandDict = new Dictionary<string, Func<string, string>>();
        Dictionary<Guid, StackPanel> GuidJobListingDict = new Dictionary<Guid, StackPanel>();
        public IRC()
        {
            this.CommandDict["join"] = SetChannel;
            this.CommandDict["leave"] = LeaveChannel;
            InitializeComponent();
            Loaded += PageLoaded;
            DataContext = this;

        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            AddHiddenChannel("Lobby");
            SetChannel("Lobby");

            this.IRCPrefix = Global.GameState.UserName;
            InputBlock.KeyDown += IRC_KeyDown;
            InputBlock.Focus();
            
            AddChannel("jobs");
        }

        public void AddHiddenChannel(string channelName)
        {
            IRCChannel IRCChannel = new IRCChannel(channelName);
            IRCChannel.ChannelNameTextBlock.Text = channelName;
            this.IDChannelDict.Add(channelName, IRCChannel);
        }

        public void AddHiddenChannel(IRCChannel iRCChannel)
        {
            this.IDChannelDict.Add(iRCChannel.ChannelName, iRCChannel);
        }

        public void AddMessageFromThread(string channelName, string sender, string message)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    AddMessage(channelName, sender, message);
                });

            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                //Do nothing
            }
        }

        public StackPanel AddMessage(string channelName, string sender, string message)
        {
            StackPanel stp = new StackPanel();
            TextBlock nameBlock = new TextBlock();
            TextBlock messageBlock = new TextBlock();
            messageBlock.TextWrapping = TextWrapping.Wrap;
            messageBlock.Text = message;
            messageBlock.Background = Brushes.Black;
            messageBlock.Foreground = Brushes.White;
            nameBlock.Width = 150;
            nameBlock.TextWrapping = TextWrapping.Wrap;
            nameBlock.Text = sender;
            nameBlock.Background = Brushes.Black;
            nameBlock.Foreground = Brushes.White;
            nameBlock.Margin = new Thickness(0, 0, 10, 0);
            stp.Children.Add(nameBlock);
            stp.Children.Add(messageBlock);
            stp.VerticalAlignment = VerticalAlignment.Stretch;
            stp.HorizontalAlignment = HorizontalAlignment.Stretch;

            stp.Orientation = Orientation.Horizontal;

            if (!IDChannelDict.TryGetValue(channelName, out IRCChannel toWriteTo))
            {
                return stp;
            }
            if (IsCurrentChannel(toWriteTo))
            {
                IRCOutput.Children.Add(stp);
                return stp;
            }
            toWriteTo.Messages.Add(stp);
            toWriteTo.ChannelNameTextBlock.Foreground = Brushes.Red;
            toWriteTo.ChannelNameTextBlock.Background = Brushes.Black;

            bool IsCurrentChannel(IRCChannel toWriteTo)
            {
                if(this.CurrentChannel == null)
                {
                    return false;
                }
                return this.CurrentChannel.ChannelName == toWriteTo.ChannelName;
            }
            return stp;
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
                    StackPanel stp = new StackPanel();
                    TextBlock nameBlock = new TextBlock();
                    TextBlock messageBlock = new TextBlock();
                    messageBlock.TextWrapping = TextWrapping.Wrap;
                    nameBlock.Width = 0;
                    nameBlock.TextWrapping = TextWrapping.Wrap;
                    nameBlock.Text = string.Empty;
                    messageBlock.Text = result;
                    nameBlock.Background = Brushes.Black;
                    nameBlock.Foreground = Brushes.White;
                    nameBlock.Margin = new Thickness(0, 0, 10, 0);
                    messageBlock.Background = Brushes.Black;
                    messageBlock.Foreground = Brushes.White;
                    stp.Children.Add(nameBlock);
                    stp.Children.Add(messageBlock);
                    stp.VerticalAlignment = VerticalAlignment.Stretch;
                    stp.HorizontalAlignment = HorizontalAlignment.Stretch;

                    this.IRCOutput.Children.Add(stp);
                }
                this.InputBlock.Text = string.Empty;
                InputBlock.Focus();
                Scroller.ScrollToBottom();
            }
        }

        public string SetChannel(string ChannelName)
        {
            //Save current convo
            if (this.CurrentChannel != null)
            {
                SaveCurrentChannelMessages();
            }

            //Get channel and port messages back
            if (!this.IDChannelDict.TryGetValue(ChannelName, out IRCChannel iRCChannel))
            {
                return "Channel not found.\n";
            }
            this.CurrentChannel = iRCChannel;
            this.IRCOutput.Children.Clear();
            CurrentChannel.Messages.ForEach(x => this.IRCOutput.Children.Add(x));

            if (!this.Channels.Children.Contains(iRCChannel.ChannelNameTextBlock)
                && iRCChannel.ChannelNameTextBlock != null
                && iRCChannel.ChannelName != "Lobby")
            {
                this.Channels.Children.Add(iRCChannel.ChannelNameTextBlock);
            }

            if (CurrentChannel.ChannelNameTextBlock != null)
            {
                CurrentChannel.ChannelNameTextBlock.Background = Brushes.White;
                CurrentChannel.ChannelNameTextBlock.Foreground = Brushes.Black;
            }

            if (CurrentChannel.DialogResolver != null)
            {
                CurrentChannel.DialogResolver.CheckStartDialog();
            }
            return string.Empty; /*"Channel "+ "\"" + ChannelName + "\" joined.\n";*/
        }

        private void SaveCurrentChannelMessages()
        {
            if (CurrentChannel.Messages != null)
            {
                this.CurrentChannel.Messages = IRCOutput.Children.OfType<StackPanel>().ToList();
                this.IDChannelDict[CurrentChannel.ChannelName] = this.CurrentChannel;
            }
            if (this.CurrentChannel.ChannelNameTextBlock != null)
            {
                this.CurrentChannel.ChannelNameTextBlock.Background = Brushes.Black;
                this.CurrentChannel.ChannelNameTextBlock.Foreground = Brushes.White;
            }
        }

        public void AddChannel(IRCChannel iRCChannel)
        {
            this.IDChannelDict.Add(iRCChannel.ChannelName, iRCChannel);
            this.Channels.Children.Add(iRCChannel.ChannelNameTextBlock);
        }

        public void AddChannel(string channelName)
        {
            IRCChannel IRCChannel = new IRCChannel(channelName);
            IRCChannel.ChannelNameTextBlock.Text = channelName;
            this.IDChannelDict.Add(channelName, IRCChannel);
            this.Channels.Children.Add(IRCChannel.ChannelNameTextBlock);
        }

        public void RemoveChannel(string channelName)
        {
            if(this.IDChannelDict.TryGetValue(channelName, out IRCChannel iRCChannel))
            {
                if(channelName == CurrentChannel.ChannelName)
                {
                    SetChannel("Lobby");
                }
                this.Channels.Children.Remove(iRCChannel.ChannelNameTextBlock);
                this.IDChannelDict.Remove(channelName);
            }
        }
        public void RemoveJobListing(Mission mission)
        {
            this.RemoveChannel(mission.MissionChannel);
            this.IDChannelDict["jobs"].Messages.Remove(this.GuidJobListingDict[mission.id]);
        }
        public void AddJobListing(Mission mission)
        {
            this.AddHiddenChannel(mission.IRCChannel);
            string listingMessage = mission.DialogResolver.SetInfoGetListing(mission.Contact, mission.Target, mission.Reward) +
                "\nJoin " + mission.MissionChannel + " for more info.";
            mission.DialogResolver.SetupDialog();
            GuidJobListingDict[mission.id] = this.AddMessage("jobs", mission.Contact, listingMessage);
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

        //public ObservableCollection<StackPanel> IRCOutput
        //{
        //    get
        //    {
        //        return iRCOutput;
        //    }
        //    set
        //    {
        //        iRCOutput = value;
        //        OnPropertyChanged("IRCOutput");
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
