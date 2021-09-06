using Game.UI.Pages;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Game.UI
{
    public class ProgramWindow : ContentControl, INotifyPropertyChanged
    {
        public bool HasControlButtons = true;
        public bool HasClose = true;
        public bool HasMax = true;
        public bool HasMin = true;
        public bool Maxed = false;
        public double WindowedWidth = 0;
        public double WindowedHeight = 0;
        public double WindowedLeft = 0;
        public double WindowedTop = 0;

        private int isActive = 0;

        public int IsActive
        {
            get { return isActive; }
            set 
            { 
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public ContentControl TaskBarButton;
        public ToggleButton TaskBarToggleButton;

        public Button MaxButton { get; internal set; }
        public Button MinButton { get; internal set; }
        public Button CloseButton { get; internal set; }
        public Thumb MoveThumb { get; internal set; }

        #region DependencyProperties

        public Image icon
        {
            get { return (Image)GetValue(iconProperty); }
            set { SetValue(iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty iconProperty = DependencyProperty.Register("icon", typeof(Image), typeof(ProgramWindow), null);

        public TextBlock title
        {
            get { return (TextBlock)GetValue(titleProperty); }
            set { SetValue(titleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty titleProperty = DependencyProperty.Register("title", typeof(TextBlock), typeof(ProgramWindow), null);

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty maxButtonProperty = DependencyProperty.Register("maxButton", typeof(Button), typeof(ProgramWindow), null);

        public object mainContent
        {
            get { return GetValue(mainContentProperty); }
            set { SetValue(mainContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mainContentProperty = DependencyProperty.Register("mainContent", typeof(object), typeof(ProgramWindow), null);

        #endregion DependencyProperties

        public ProgramWindow(DisplayablePage pageToDisplay) : base()
        {
            this.DataContext = this;
            this.PreviewMouseDown += ProgramWindow_PreviewMouseDown;
            this.IsVisibleChanged += ProgramWindow_IsVisibleChanged;
            Global.MainWindow.MainCanvas.SizeChanged += MainCanvas_SizeChanged;
            this.Loaded += ProgramWindow_Loaded;

            Global.MainWindow.MainCanvas.Children.Add(this);
            Canvas.SetLeft(this, 100);
            Canvas.SetTop(this, 100);

            if (pageToDisplay == null)
            {
                return;
            }
            pageToDisplay.Loaded += PageToDisplay_Loaded;
            this.Style = (Style)FindResource("ProgramWindowActiveStyle");
            this.title = new TextBlock();

            this.title.Text = pageToDisplay.Title;
            this.icon = new Image();
            this.icon.Source = pageToDisplay.Icon;
            this.icon.Width = 16;
            this.icon.Height = 16;

            this.HasMin = pageToDisplay.HasMin;
            this.HasMax = pageToDisplay.HasMax;
            this.HasClose = pageToDisplay.HasClose;

            //Register self with main window.
            Global.MainWindow.ContentControlElements.Add(this);

            Frame frame = new Frame();
            this.mainContent = frame;
            frame.Margin = new Thickness(1, 4, 1, 1);
            frame.Navigate(pageToDisplay);
            ProgramWindow_PreviewMouseDown(this, null);
            SetupTaskBarButton();
        }

        private void MoveThumb_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ToggleMaxOrWindofy();
        }

        private void ToggleMaxOrWindofy()
        {
            if (this.Maxed)
            {
                this.Windowfy();
            }
            else
            {
                this.Maximize();
            }
        }

        private void PageToDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = this.ActualHeight;
            this.Width = this.ActualWidth;
            this.MinHeight = this.ActualHeight;
            this.MinWidth = this.ActualWidth;

            this.MoveThumb.MouseDoubleClick += MoveThumb_MouseDoubleClick;
        }

        private void WindofyIfMaxed()
        {
            if (this.Maxed)
            {
                this.Windowfy();
            }
        }

        private void ProgramWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MainCanvas_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (this.Maxed)
            {
                this.Width = Global.MainWindow.MainCanvas.ActualWidth;
                this.Height = Global.MainWindow.MainCanvas.ActualHeight;
                Canvas.SetLeft(this, 0);
                Canvas.SetTop(this, 0);
            }
        }

        private void ProgramWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.TaskBarButton == null)
            {
                return;
            }
            if (this.Visibility == Visibility.Visible)
            {
                this.TaskBarButton.Style = (Style)FindResource("TaskBarButtonBoxInverted");
                this.TaskBarToggleButton.IsChecked = true;
            }
            else
            {
                this.TaskBarButton.Style = (Style)FindResource("TaskBarButtonBox");
                this.TaskBarToggleButton.IsChecked = false;
            }
        }

        private void SetupTaskBarButton()
        {
            this.TaskBarButton = new ContentControl();
            TaskBarButton.Style = (Style)FindResource("TaskBarButtonBoxInverted");
            TaskBarButton.VerticalAlignment = VerticalAlignment.Center;
            TaskBarButton.HorizontalAlignment = HorizontalAlignment.Left;
            TaskBarButton.Margin = new Thickness(0, 2, 1, 2);
            TaskBarButton.Padding = new Thickness(0, 2, 0, 2);
            TaskBarButton.Height = 27;

            TaskBarToggleButton = new ToggleButton();
            TaskBarToggleButton.VerticalAlignment = VerticalAlignment.Stretch;
            TaskBarToggleButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            TaskBarToggleButton.BorderBrush = Brushes.Transparent;
            TaskBarToggleButton.Foreground = Brushes.Black;
            TaskBarToggleButton.Background = Brushes.Transparent;
            TaskBarToggleButton.Click += ToggleButton_Click;
            TaskBarToggleButton.Margin = new Thickness(0);
            //TaskBarToggleButton.Style = (Style)FindResource("NoSelectionBoxButton");

            StackPanel stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            stackPanel.Orientation = Orientation.Horizontal;

            Image image = new Image();
            image.Source = this.icon.Source;
            image.Width = 20;
            image.Height = 20;
            image.VerticalAlignment = VerticalAlignment.Center;
            image.HorizontalAlignment = HorizontalAlignment.Center;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = this.title.Text;

            stackPanel.Children.Add(image);
            stackPanel.Children.Add(textBlock);

            TaskBarToggleButton.Content = stackPanel;
            TaskBarButton.Content = TaskBarToggleButton;

            Global.MainWindow.AddToTaskBar(TaskBarButton);

            this.TaskBarToggleButton.IsChecked = true;
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.TaskBarToggleButton.IsChecked == true)
            {
                this.Visibility = Visibility.Visible;
                Global.MainWindow.SetOntop(this);
                this.SetActive();
            }
            else
            {
                this.Visibility = Visibility.Hidden;
            }
        }

        private void ProgramWindow_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e != null)
            {
                e.Handled = false;
            }
            SetActive();
        }

        public void SetActive()
        {
            Global.MainWindow.SetOntop(this);
            this.Focus();
            this.IsActive = 1;
            foreach (ProgramWindow p in Global.MainWindow.ContentControlElements)
            {
                if (p == this)
                {
                    continue;
                }
                p.IsActive = 0;
            }
            try
            {
                if (this.Maxed)
                {
                    (MaxButton.Content as TextBlock).Text = "◈";
                }
                else
                {
                    (MaxButton.Content as TextBlock).Text = "◇";
                }
                (MinButton.Content as TextBlock).Text = "_";
                (CloseButton.Content as TextBlock).Text = "X";
            }
            catch (Exception) { }
        }

        public void SetInactive()
        {
            this.IsActive = 1;
            try
            {
                if (this.Maxed)
                {
                    (MaxButton.Content as TextBlock).Text = "◈";
                }
                else
                {
                    (MaxButton.Content as TextBlock).Text = "◇";
                }
                (MinButton.Content as TextBlock).Text = "_";
                (CloseButton.Content as TextBlock).Text = "X";
            }
            catch (Exception) { }
        }

        internal void OnMaxButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleMaxOrWindofy();
        }

        internal void OnMinButtonClick(object sender, RoutedEventArgs e)
        {
            this.Minimize();
        }

        internal void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        public void Maximize()
        {
            if (!this.Maxed)
            {
                this.WindowedHeight = this.Height;
                this.WindowedWidth = this.Width;
                this.WindowedLeft = Canvas.GetLeft(this);
                this.WindowedTop = Canvas.GetTop(this);
                this.Width = Global.MainWindow.MainCanvas.ActualWidth;
                this.Height = Global.MainWindow.MainCanvas.ActualHeight;

                (MaxButton.Content as TextBlock).Text = "◈";

                Canvas.SetLeft(this, 0);
                Canvas.SetTop(this, 0);
                this.Maxed = true;
            }
            this.SetActive();
        }

        public void Minimize()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void Windowfy()
        {
            if (this.Maxed)
            {
                this.Width = this.WindowedWidth;
                this.Height = this.WindowedHeight;
                Canvas.SetLeft(this, this.WindowedLeft);
                Canvas.SetTop(this, this.WindowedTop);

                (MaxButton.Content as TextBlock).Text = "◇";

                this.Maxed = false;
            }
            this.SetActive();
        }

        public void Close()
        {
            Global.MainWindow.MainCanvas.Children.Remove(this);
            Global.MainWindow.TaskBar.Children.Remove(this.TaskBarButton);
            Global.MainWindow.ContentControlElements.Remove(this);
            this.Visibility = Visibility.Collapsed;
            this.IsEnabled = false;
            if(OnClosed != null)
            {
                OnClosed(this, new WindowClosedEventArgs(string.Empty));
            }
        }

        public void Open()
        {
            //To something for programs that need to open
        }


        public delegate void WindowClosedEventHandler(object sender, WindowClosedEventArgs e);

        public event WindowClosedEventHandler OnClosed;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class WindowClosedEventArgs : EventArgs
    {
        public string Status { get; private set; }

        public WindowClosedEventArgs(string status)
        {
            Status = status;
        }
    }

public class ProgramWindowNoButtons : ProgramWindow
    {
        public ProgramWindowNoButtons(DisplayablePage pageToDisplay) : base(pageToDisplay)
        {
        }
    }
}