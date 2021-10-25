using Game.Core.Dialog;
using Game.Core.Mission.MissionTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Game.Model
{
    public class IRCChannel
    {
        [JsonConstructor]
        public IRCChannel()
        {
            this.Messages = new();
        }


        public IRCChannel(string channelName, GenericMissionDialogResolver dialogResolver)
        {
            this.Id = Guid.NewGuid();
            this.Messages = new List<StackPanel>();
            this.ChannelName = channelName;
            this.ChannelNameTextBlock = new TextBlock();
            this.ChannelNameTextBlock.Text = channelName;
            this.ChannelNameTextBlock.Foreground = Brushes.White;
            this.ChannelNameTextBlock.Background = Brushes.Black;
            this.DialogResolver = dialogResolver;
            Global.AllIRCChannelsDict[Id] = this;
        }

        public IRCChannel(string channelName)
        {
            this.Id = Guid.NewGuid();
            this.Messages = new List<StackPanel>();
            this.ChannelName = channelName;
            this.ChannelNameTextBlock = new TextBlock();
            this.ChannelNameTextBlock.Text = channelName;
            this.ChannelNameTextBlock.Foreground = Brushes.White;
            this.ChannelNameTextBlock.Background = Brushes.Black;
            Global.AllIRCChannelsDict[Id] = this;
        }
        
        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            this.ChannelNameTextBlock = new TextBlock();
            this.ChannelNameTextBlock.Text = this.ChannelName;
            this.ChannelNameTextBlock.Foreground = Brushes.White;
            this.ChannelNameTextBlock.Background = Brushes.Black;

            for (int i = 0; i < this.StringNames.Count; i++)
            {
                StackPanel stp = new StackPanel();
                TextBlock timeStampBlock = new TextBlock();
                TextBlock nameBlock = new TextBlock();
                TextBlock messageBlock = new TextBlock();
                timeStampBlock.Name = "timeStampBlock";
                nameBlock.Name = "nameBlock";
                messageBlock.Name = "messageBlock";

                messageBlock.TextWrapping = TextWrapping.Wrap;
                messageBlock.Text = StringMessages[i];
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
                nameBlock.Text = StringNames[i];
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
                if (this.SenderColorDict.ContainsKey(StringNames[i]))
                {
                    senderColor = this.SenderColorDict[StringNames[i]];
                }
                else
                {
                    senderColor = UTILS.PickRandomColor(this.SenderColorDict.Values.ToList());
                    this.SenderColorDict[StringNames[i]] = senderColor;
                }
                nameBlock.Foreground = new SolidColorBrush(Color.FromArgb(senderColor.A, senderColor.R, senderColor.G, senderColor.B));

                this.Messages.Add(stp);
            }
        }
        
        [OnSerializing]
        public void OnSerializing(StreamingContext streamingContext)
        {
            foreach (StackPanel messagePanel in this.Messages)
            {
                TextBlock nameBlock = (TextBlock)messagePanel.FindName("nameBlock");
                TextBlock messageBlock = (TextBlock)messagePanel.FindName("messageBlock");
                TextBlock timeStampBlock = (TextBlock)messagePanel.FindName("timeStampBlock");

                string name = nameBlock == null ? string.Empty : nameBlock.Text;
                string message = messageBlock == null ? string.Empty : messageBlock.Text;
                string dateTime = timeStampBlock == null ? string.Empty : timeStampBlock.Text;

                this.StringNames.Add(name);
                this.StringMessages.Add(message);
                this.stringDateTimes.Add(dateTime);
            }
        }

        public Guid Id;
        public string ChannelName;

        [JsonIgnore]
        public List<StackPanel> Messages { get; set; }

        public List<string> StringNames { get; set; } = new();
        public List<string> StringMessages { get; set; } = new();
        public List<string> stringDateTimes { get; set; } = new();

        [JsonIgnore]
        public TextBlock ChannelNameTextBlock;

        private Guid mission;
        public GenericMissionDialogResolver DialogResolver { get; set; }

        [JsonIgnore]
        public MissionTemplate Mission { get => Global.AllMissionsDict[mission]; set => mission = value.Id; }

        public Dictionary<string, System.Drawing.Color> SenderColorDict = new();
    }
}