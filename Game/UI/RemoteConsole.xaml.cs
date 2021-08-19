using Game.Core.Console;
using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class RemoteConsole : System.Windows.Controls.Page
    {
        private ConsoleContent ConsoleContent = new ConsoleContent();
        private RemoteCommandParser CommandParser;
        private int historyIndex = 0;

        public RemoteConsole()
        {
            InitializeComponent();
            DataContext = ConsoleContent;
            Loaded += ConsolePageLoaded;
            this.CommandParser = new RemoteCommandParser(ConsoleContent);
            ConsoleContent.AttachCommandParser(CommandParser);
            ConsoleContent.ConsoleOutput.Add("Remote Console [Version 11.0.19042.1110]");
            ConsoleContent.ConsoleOutput.Add("(c) TracON LLC. All Rights Reserved\n");
        }

        private void ConsolePageLoaded(object sender, RoutedEventArgs e)
        {
            InputBlock.KeyDown += InputBlock_KeyDown;
            InputBlock.Focus();
        }

        public void StartConsole(string user)
        {

        }

        private void InputBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (InputBlock.Text == "\n" || string.IsNullOrEmpty(InputBlock.Text) || string.IsNullOrWhiteSpace(InputBlock.Text))
                {
                    InputBlock.Text = string.Empty;
                    return;
                }
                ConsoleContent.ConsoleInput = InputBlock.Text;
                ConsoleContent.RunCommand();
                InputBlock.Focus();
                Scroller.ScrollToBottom();
                historyIndex = ConsoleContent.History.Count - 1;
            }
        }

        private void InputBlock_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                //Debug.WriteLine(e.Key);
                //Debug.WriteLine(historyIndex);

                if (ConsoleContent.History.Count == 0)
                {
                    return;
                }
                if (historyIndex < 0)
                {
                    historyIndex = ConsoleContent.History.Count - 1;
                }
                ConsoleContent.ConsoleInput = ConsoleContent.History[historyIndex];
                historyIndex = Math.Max(0, historyIndex - 1);
            }
            if (e.Key == Key.Down)
            {
                Debug.WriteLine(e.Key);
                Debug.WriteLine(historyIndex);

                if (ConsoleContent.History.Count == 0)
                {
                    return;
                }
                if (historyIndex > ConsoleContent.History.Count - 1)
                {
                    historyIndex = 0;
                }
                ConsoleContent.ConsoleInput = ConsoleContent.History[historyIndex];
                historyIndex = Math.Min(historyIndex + 1, ConsoleContent.History.Count - 1);
            }
            SetCaretToLast(sender as TextBox);
        }

        public void SetCaretToLast(TextBox t)
        {
            t.CaretIndex = t.Text.Length;
        }
    }

    public class ConsoleContent : INotifyPropertyChanged
    {
        private ICommandParser CMDP;
        private string consoleInput = string.Empty;
        private ObservableCollection<string> consoleOutput = new ObservableCollection<string>();
        public List<string> History = new List<string>();
        public string Prefix { set; get; }

        public void AttachCommandParser(ICommandParser CMDP)
        {
            this.CMDP = CMDP;
            CMDP.AttachConsole(this);
        }

        public string ConsolePrefix
        {
            get
            {
                return Prefix + ">";
            }
            set
            {
                Prefix = value;
                OnPropertyChanged("ConsolePrefix");
            }
        }

        public string ConsoleInput
        {
            get
            {
                return consoleInput;
            }
            set
            {
                consoleInput = value;
                OnPropertyChanged("ConsoleInput");
            }
        }

        public ObservableCollection<string> ConsoleOutput
        {
            get
            {
                return consoleOutput;
            }
            set
            {
                consoleOutput = value;
                OnPropertyChanged("ConsoleOutput");
            }
        }

        public void RunCommand()
        {
            ConsoleOutput.Add(ConsoleInput);

            //This is ugly but ey
            if (this.History.Contains(consoleInput))
            {
                this.History.Remove(consoleInput);
                this.History.Add(consoleInput);
            }
            this.History.Add(consoleInput);

            // do your stuff here.
            CMDP.ParseCommand(consoleInput, Prefix);
            //consoleOutput.Add("\n");

            ConsoleInput = String.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}