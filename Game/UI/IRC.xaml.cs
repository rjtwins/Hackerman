using Game.Core.Mission.MissionTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for IRC.xaml
    /// </summary>
    public partial class IRC : Page, INotifyPropertyChanged
    {
        //TODO seperate command from UI.
        public string Prefix { set; get; }

        //private ObservableCollection<StackPanel> iRCOutput = new ObservableCollection<StackPanel>();

        public IRCChannel CurrentChannel;
        public Dictionary<string, IRCChannel> NameChannelDict = new();
        private Dictionary<string, Func<string, string>> CommandDict = new();
        private Dictionary<Guid, StackPanel> GuidJobListingDict = new();

        private bool loaded = false;

        public IRC()
        {
            this.CommandDict["join"] = SetChannel;
            this.CommandDict["leave"] = LeaveChannel;
            this.CommandDict["DCC"] = DCC;
            InitializeComponent();
            Loaded += PageLoaded;
            DataContext = this;
        }

        private string DCC(string commandBody)
        {
            string[] splitCommandBody = commandBody.Split(' ');
            if (splitCommandBody.Length > 2 || splitCommandBody.Length == 0)
            {
                return "DCC invalid number of arguments, use DDC command filename";
            }
            //Get or Send commands
            if (commandBody.StartsWith("Send"))
            {
                if (Global.StartEndPoint.GetFile(null, splitCommandBody[1]) == null)
                {
                    return "DCC file not found.";
                }
                if (!CurrentChannel.Mission.CheckFileNeeded(splitCommandBody[1]))
                {
                    switch (CurrentChannel.Mission.MissionType)
                    {
                        case MissionType.STEAL:
                            return "This is not the file we requested.";

                        case MissionType.STEALMULTIPLE:
                            return "This is not a file we requested.";

                        default:
                            return "This is not a file we requested.";
                    }
                }
                AddMessage(CurrentChannel.ChannelName, "", "DDC file send.");
                CurrentChannel.Mission.FileUploaded(splitCommandBody[1]);
                CurrentChannel.DialogResolver.Next();
                return "";
            }
            else if (commandBody.StartsWith("Get"))
            {
                if (CurrentChannel.Mission.GetFile())
                {
                    AddMessage(CurrentChannel.ChannelName, "", "DDC file received.");
                    return "";
                }
                return "DDC no file in queue.";
            }
            return "DCC command no recognized.";
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (loaded)
            {
                return;
            }
            loaded = true;
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
            this.NameChannelDict.Add(channelName, IRCChannel);
        }

        public void AddHiddenChannel(IRCChannel iRCChannel)
        {
            this.NameChannelDict.Add(iRCChannel.ChannelName, iRCChannel);
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
            TextBlock timeStampBlock = new TextBlock();
            TextBlock nameBlock = new TextBlock();
            TextBlock messageBlock = new TextBlock();
            messageBlock.TextWrapping = TextWrapping.Wrap;
            messageBlock.Text = message;
            messageBlock.Background = Brushes.Black;
            messageBlock.Foreground = Brushes.White;
            messageBlock.HorizontalAlignment = HorizontalAlignment.Left;
            messageBlock.VerticalAlignment = VerticalAlignment.Top;
            //timeStampBlock.Width;
            timeStampBlock.TextWrapping = TextWrapping.Wrap;
            timeStampBlock.Text = Global.GameTime.ToString("MM-dd HH:mm:ss") + ":";
            timeStampBlock.Background = Brushes.Black;
            timeStampBlock.Foreground = Brushes.White;
            timeStampBlock.Margin = new Thickness(0, 0, 1, 0);
            timeStampBlock.HorizontalAlignment = HorizontalAlignment.Left;
            timeStampBlock.VerticalAlignment = VerticalAlignment.Top;
            nameBlock.Width = 50;
            nameBlock.TextWrapping = TextWrapping.Wrap;
            nameBlock.Text = sender;
            nameBlock.Background = Brushes.Black;
            nameBlock.Foreground = Brushes.White;
            nameBlock.Margin = new Thickness(0, 0, 10, 0);
            nameBlock.HorizontalAlignment = HorizontalAlignment.Left;
            nameBlock.VerticalAlignment = VerticalAlignment.Top;
            stp.Children.Add(timeStampBlock);
            stp.Children.Add(nameBlock);
            stp.Children.Add(messageBlock);
            stp.VerticalAlignment = VerticalAlignment.Stretch;
            stp.HorizontalAlignment = HorizontalAlignment.Stretch;
            stp.Orientation = Orientation.Horizontal;
            System.Drawing.Color senderColor;

            if (!NameChannelDict.TryGetValue(channelName, out IRCChannel toWriteTo))
            {
                return stp;
            }

            if (toWriteTo.SenderColorDict.ContainsKey(sender))
            {
                senderColor = toWriteTo.SenderColorDict[sender];
            }
            else
            {
                senderColor = UTILS.PickRandomColor(toWriteTo.SenderColorDict.Values.ToList());
                toWriteTo.SenderColorDict[sender] = senderColor;
            }
            nameBlock.Foreground = new SolidColorBrush(Color.FromArgb(senderColor.A, senderColor.R, senderColor.G, senderColor.B));

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
                if (this.CurrentChannel == null)
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
                if (result != string.Empty)
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
            if (!this.NameChannelDict.TryGetValue(ChannelName, out IRCChannel iRCChannel))
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
                this.NameChannelDict[CurrentChannel.ChannelName] = this.CurrentChannel;
            }
            if (this.CurrentChannel.ChannelNameTextBlock != null)
            {
                this.CurrentChannel.ChannelNameTextBlock.Background = Brushes.Black;
                this.CurrentChannel.ChannelNameTextBlock.Foreground = Brushes.White;
            }
        }

        public void AddChannel(IRCChannel iRCChannel)
        {
            this.NameChannelDict.Add(iRCChannel.ChannelName, iRCChannel);
            this.Channels.Children.Add(iRCChannel.ChannelNameTextBlock);
        }

        public bool ChannelExsits(string channelName)
        {
            if (this.NameChannelDict.ContainsKey(channelName))
            {
                return true;
            }
            return false;
        }

        public void AddChannelFromThread(string channelName)
        {
            this.Dispatcher.Invoke(() => { AddChannel(channelName); });
        }

        public void AddChannel(string channelName)
        {
            IRCChannel IRCChannel = new IRCChannel(channelName);
            IRCChannel.ChannelNameTextBlock.Text = channelName;
            this.NameChannelDict.Add(channelName, IRCChannel);
            this.Channels.Children.Add(IRCChannel.ChannelNameTextBlock);
        }

        public void RemoveChannel(string channelName)
        {
            if (this.NameChannelDict.TryGetValue(channelName, out IRCChannel iRCChannel))
            {
                if(CurrentChannel != null)
                {
                    if (channelName == CurrentChannel.ChannelName)
                    {
                        SetChannel("Lobby");
                    }
                }
                this.Channels.Children.Remove(iRCChannel.ChannelNameTextBlock);
                this.NameChannelDict.Remove(channelName);
            }
        }

        public void RemoveJobListing(MissionTemplate mission)
        {
            this.RemoveChannel(mission.MissionChannel);
            this.NameChannelDict["jobs"].Messages.Remove(this.GuidJobListingDict[mission.id]);
        }

        public void AddJobListing(MissionTemplate mission)
        {
            this.AddHiddenChannel(mission.IRCChannel);
            string listingMessage = mission.DialogResolver.SetInfoGetListing(mission.Contact, mission.Target, mission.Reward) +
                "\nJoin " + mission.MissionChannel + " for more info.\n";
            mission.DialogResolver.SetupDialog();
            GuidJobListingDict[mission.id] = this.AddMessage("jobs", mission.Contact, listingMessage);
        }

        public string LeaveChannel(string input)
        {
            if (CurrentChannel.ChannelName == "Lobby")
            {
                return string.Empty;
            }
            return SetChannel("Lobby");
        }

        private string Parse(string text)
        {
            string result = string.Empty;

            if (int.TryParse(text, out int choise))
            {
                CurrentChannel.DialogResolver.Next(choise);
                return string.Empty;
            }

            var SplitText = text.Split(' ');
            string commandBody = string.Empty;
            if (SplitText.Length == 0)
            {
                return result;
            }
            if (!CommandDict.TryGetValue(SplitText[0], out Func<string, string> toRun))
            {
                return "Command " + "\" " + SplitText[0] + "\" not recognized.\n";
            }
            if (SplitText.Length > 1)
            {
                commandBody = string.Join(' ', SplitText[1..SplitText.Length]);
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