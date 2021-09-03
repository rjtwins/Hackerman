using System.Windows;
using System.Windows.Controls;

namespace Game.UI
{
    internal class MinButton : Button
    {
        public MinButton()
        {
            this.Loaded += MinButton_Loaded;
        }

        private void MinButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ProgramWindow programWindow = (ProgramWindow)DataContext;

            if (!programWindow.HasControlButtons)
            {
                (this.Parent as UIElement).Visibility = Visibility.Collapsed;
            }

            this.Content = new TextBlock();
            programWindow.MinButton = this;
            this.Click += programWindow.OnMinButtonClick;
            (this.Content as TextBlock).Text = "_";
        }
    }

    internal class MaxButton : Button
    {
        public MaxButton()
        {
            this.Loaded += MaxButton_Loaded;
        }

        private void MaxButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ProgramWindow programWindow = (ProgramWindow)DataContext;

            if (!programWindow.HasControlButtons)
            {
                (this.Parent as UIElement).Visibility = Visibility.Collapsed;
            }

            this.Content = new TextBlock();
            programWindow.MaxButton = this;
            this.Click += programWindow.OnMaxButtonClick;
            (this.Content as TextBlock).Text = "◇";
        }
    }

    internal class CloseButton : Button
    {
        public CloseButton()
        {
            this.Loaded += CloseButton_Loaded;
        }

        private void CloseButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ProgramWindow programWindow = (ProgramWindow)DataContext;

            if (!programWindow.HasControlButtons)
            {
                (this.Parent as UIElement).Visibility = Visibility.Collapsed;
            }

            if (!programWindow.HasClose)
            {
                (this.Parent as UIElement).Visibility = Visibility.Collapsed;
            }
            this.Content = new TextBlock();
            programWindow.CloseButton = this;
            this.Click += programWindow.OnCloseButtonClick;
            (this.Content as TextBlock).Text = "X";
        }
    }
}