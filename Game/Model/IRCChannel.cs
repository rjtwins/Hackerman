using Game.Core.Dialog;
using Game.Core.Mission.MissionTypes;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Game.Model
{
    public class IRCChannel
    {
        public IRCChannel(string channelName, GenericMissionDialogResolver dialogResolver)
        {
            this.Messages = new List<StackPanel>();
            this.ChannelName = channelName;
            this.ChannelNameTextBlock = new TextBlock();
            this.ChannelNameTextBlock.Text = channelName;
            this.ChannelNameTextBlock.Foreground = Brushes.White;
            this.ChannelNameTextBlock.Background = Brushes.Black;
            this.DialogResolver = dialogResolver;
        }

        public IRCChannel(string channelName)
        {
            this.Messages = new List<StackPanel>();
            this.ChannelName = channelName;
            this.ChannelNameTextBlock = new TextBlock();
            this.ChannelNameTextBlock.Text = channelName;
            this.ChannelNameTextBlock.Foreground = Brushes.White;
            this.ChannelNameTextBlock.Background = Brushes.Black;
        }

        public string ChannelName;
        public List<StackPanel> Messages;
        public TextBlock ChannelNameTextBlock;
        public MissionTemplate Mission;
        public GenericMissionDialogResolver DialogResolver { get; set; }
        public Dictionary<string, System.Drawing.Color> SenderColorDict = new();
    }
}