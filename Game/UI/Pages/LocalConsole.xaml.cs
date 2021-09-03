using Game.Core.Console;
using Game.UI.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for Console.xaml
    /// </summary>
    public partial class LocalConsole : DisplayablePage
    {
        private ConsoleContent ConsoleContent = new ConsoleContent();
        private LocalCommandParser CommandParser;
        private int historyIndex = 0;

        public LocalConsole()
        {
            InitializeComponent();
            DataContext = ConsoleContent;
            Loaded += ConsolePageLoaded;
            this.CommandParser = new LocalCommandParser(ConsoleContent);
            ConsoleContent.AttachCommandParser(CommandParser);
            ConsoleContent.ConsoleOutput.Add("M-Soft Apature [Version 10.3]");
            ConsoleContent.ConsoleOutput.Add("(c) M-Soft LLC. All Rights Reserved\n");
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

        public void AddLine(string Line)
        {
            ConsoleContent.ConsoleOutput.Add(Line);
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
                //Debug.WriteLine(e.Key);
                //Debug.WriteLine(historyIndex);

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
}