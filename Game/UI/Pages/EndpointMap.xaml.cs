using Game.Core.Endpoints;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Game.Model;

namespace Game.UI.Pages
{
    /// <summary>
    /// Interaction logic for EndpointMap.xaml
    /// </summary>
    public partial class EndpointMap : Game.UI.Pages.DisplayablePage, INotifyPropertyChanged
    {
        private Dictionary<Guid, Endpoint> DrawnEndpointsDict = new();
        private Dictionary<Guid, Button> ButtonDict = new();
        private HashSet<Endpoint> FilteredOutEndpoints = new();
        private bool HideText { get; set; } = false;

        //private Dictionary<Guid, StackPanel> StackPanelDict = new();
        private Dictionary<Guid, TextBlock> TextBlockDict = new();

        private Dictionary<Guid, Point> PointDict = new();
        private Polyline Polyline = new();
        private bool isConnected = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private int _traceTimer = int.MaxValue;
        public int TraceTimer
        {
            get { return _traceTimer; }
            set 
            {
                _traceTimer = value;
                OnPropertyChanged("TraceTimer");
            }
        }
        public EndpointMap()
        {
            this.DataContext = this;
            this.Loaded += EndpointMap_Loaded;
            InitializeComponent();
            Map.Children.Add(Polyline);
            Polyline.Stroke = System.Windows.Media.Brushes.Red;
            Polyline.StrokeDashArray = new DoubleCollection(new double[] { 2, 3, 2 });
            this.HasClose = false;
        }
        private void EndpointMap_Loaded(object sender, RoutedEventArgs e)
        {
            this.Map_Loaded(sender, e);
            //EnableTraceTimer();
            Global.BounceNetwork.OnAddOrRemove += BounceNetwork_OnAddOrRemove;
        }
        private void BounceNetwork_OnAddOrRemove(object sender, Model.NotifyingListAddOrRemoveEventArgs<Endpoint> e)
        {
            switch (e.Action)
            {
                case NotifyingListAddOrRemove.ADD:
                    ColorEndpoint(e.Item, Brushes.Purple);
                    e.Item.isHidden = false;
                    RedrawEndpoints();
                    break;
                case NotifyingListAddOrRemove.REMOVE:
                    ColorEndpoint(e.Item, Brushes.Yellow);
                    RedrawEndpoints();
                    break;
                default:
                    break;
            }

        }
        private void ColorEndpoint(Endpoint endpoint, Brush brush)
        {
            if (!ButtonDict.TryGetValue(endpoint.Id, out Button endpointButton))
            {
                return;
            }
            Global.MainWindow.Dispatcher.Invoke(() =>
            {
                lock (endpointButton)
                {
                    endpointButton.Background = brush;
                }
            });
        }

        public void FilterOnEndpoint(string filterString)
        {
            this.FilteredOutEndpoints.Clear();
            foreach (Endpoint e in Global.AllEndpoints)
            {
                if (e.IPAddress.ToLower().Contains(filterString.ToLower()) || (e.IPAddress.ToLower() == filterString.ToLower()))
                {
                    continue;
                }
                if(e.Name.ToLower().Contains(filterString.ToLower()) || (e.Name.ToLower() == filterString.ToLower()))
                {
                    continue;
                }
                this.FilteredOutEndpoints.Add(e);
            }
            this.RedrawEndpoints();
        }

        internal void UpdateTraceTimer(int TimeLeft)
        {
            this.TraceTimer = TimeLeft;
        }

        public void RedrawEndpoints()
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (Endpoint e in Global.AllEndpoints)
                {
                    if (FilteredOutEndpoints.Contains(e))
                    {
                        ButtonDict[e.Id].Visibility = Visibility.Hidden;
                        TextBlockDict[e.Id].Visibility = Visibility.Hidden;
                        continue;
                    }
                    if (e.isHidden)
                    {
                        ButtonDict[e.Id].Visibility = Visibility.Hidden;
                        TextBlockDict[e.Id].Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        ButtonDict[e.Id].Visibility = Visibility.Visible;
                        TextBlockDict[e.Id].Visibility = this.HideText ? Visibility.Hidden : Visibility.Visible;
                    }
                    if (Global.BounceNetwork.Contains(e))
                    {
                        ButtonDict[e.Id].Background = Brushes.Purple;
                    }
                    else if (e.IsLocalEndpoint)
                    {
                        ButtonDict[e.Id].Background = Brushes.Red;
                    }
                    else
                    {
                        ButtonDict[e.Id].Background = Brushes.Yellow;
                    }
                }
            });            
        }

        public void DisplayEndpoints()
        {
            foreach (Endpoint e in Global.AllEndpoints)
            {
                if (this.DrawnEndpointsDict.ContainsValue(e))
                {
                    //Go home you are allready drawn
                    continue;
                }

                //Variables
                Button btn = new Button();
                TextBlock txb = new TextBlock();

                //Dict
                ButtonDict[e.Id] = btn;
                TextBlockDict[e.Id] = txb;
                DrawnEndpointsDict[e.Id] = e;

                //Settings
                btn.Click += EndpointClick;
                btn.Background = Brushes.Yellow;
                btn.BorderThickness = new Thickness(0);
                btn.BorderBrush = Brushes.Transparent;
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.VerticalAlignment = VerticalAlignment.Top;
                if (e.IsLocalEndpoint)
                {
                    btn.Background = Brushes.Red;
                }
                btn.Tag = e.Id;
                btn.Width = 3;
                btn.Height = 3;
                txb.HorizontalAlignment = HorizontalAlignment.Left;
                txb.VerticalAlignment = VerticalAlignment.Top;
                txb.Text = e.Name;

                //Location
                int x = e.x / 10;
                int y = e.y / 10;
                Canvas.SetLeft(btn, x);
                Canvas.SetTop(btn, y);
                Map.Children.Add(btn);
                Canvas.SetLeft(txb, x);
                Canvas.SetTop(txb, y - 6);
                Canvas.SetZIndex(txb, int.MinValue);
                Map.Children.Add(txb);
                if (e.isHidden)
                {
                    btn.Visibility = Visibility.Hidden;
                    txb.Visibility = Visibility.Hidden;
                }
            }
        }

        internal void PingEndpoint(Endpoint currentEndpoint)
        {
            Guid id = currentEndpoint.Id;
            ColorAnimation animation = new ColorAnimation(Colors.Red, new Duration(TimeSpan.FromSeconds(1)));
            animation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Button btn = ButtonDict[id];
            btn.Dispatcher.Invoke(() => { btn.Background = Brushes.Red; });
            /*BeginAnimation(SolidColorBrush.ColorProperty, animation)*/
        }

        private void EndpointClick(object sender, RoutedEventArgs e)
        {
            //No map editing when we are in a connection.
            if (isConnected)
            {
                return;
            }

            Guid id = (Guid)(sender as Button).Tag;

            if (!DrawnEndpointsDict.ContainsKey(id))
            {
                throw new Exception("Clicked endpoint is not rendered on the map? How was it clicked?");
            }

            Endpoint endpoint = DrawnEndpointsDict[id];

            if (endpoint.IsLocalEndpoint)
            {
                return;
            }

            if (Global.Bounce.BounceList.Contains(endpoint))
            {
                Global.Bounce.RemoveBounce(endpoint);
                DrawBouncePath();
                return;
            }

            if (!UTILS.CanAddBounce(endpoint))
            {
                Global.Bounce.RemoveBounce(endpoint);
                DrawBouncePath();
                return;
            }

            Global.Bounce.AddBounce(endpoint);
            DrawBouncePath();
        }

        public void DrawBouncePath()
        {
            this.Polyline.Points.Clear();
            this.PointDict.Clear();
            if (Global.LocalEndpoint == null)
            {
                return;
            }

            var from = ButtonDict[Global.LocalEndpoint.Id];
            PointDict[Global.LocalEndpoint.Id] = new Point(Canvas.GetLeft(from) + 1.5, Canvas.GetTop(from) + 1.5);
            Polyline.Points.Add(PointDict[Global.LocalEndpoint.Id]);

            foreach (Endpoint e in Global.Bounce.BounceList)
            {
                var too = ButtonDict[e.Id];

                PointDict[e.Id] = new Point(Canvas.GetLeft(too) + 1.5, Canvas.GetTop(too) + 1.5);
                Polyline.Points.Add(PointDict[e.Id]);
            }
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            Global.App.OnMapReady();
        }

        public void DisplayConnection()
        {
            this.Polyline.StrokeDashArray = new DoubleCollection();
            DisableEndpointButtons();
            Button last = this.ButtonDict[Global.Bounce.BounceList[Global.Bounce.BounceList.Count - 1].Id];
            //last.BorderThickness = new Thickness(2d);
            last.Background = Brushes.Orange;
        }

        private void DisableEndpointButtons()
        {
            this.isConnected = true;
        }

        private void EnableEndpointButtons()
        {
            this.isConnected = false;
        }

        public void UnmakeConnection()
        {
            this.Polyline.StrokeDashArray = new DoubleCollection(new double[] { 2, 3, 2 });
            EnableEndpointButtons();
            DrawBouncePath();
            RedrawEndpoints();
        }

        private void RadioButtonShow_Checked(object sender, RoutedEventArgs e)
        {
            this.HideText = false;
            this.RedrawEndpoints();
        }

        private void RadioButtonHide_Checked(object sender, RoutedEventArgs e)
        {
            this.HideText = true;
            this.RedrawEndpoints();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            this.FilterOnEndpoint(textBox.Text);
        }

        public void EnableTraceTimer()
        {
            this.TraceTimerControl.Visibility = Visibility.Visible;
        }

        public void DisableTraceTimer()
        {
            this.TraceTimerControl.Visibility = Visibility.Collapsed;
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public override void Open()
        {
            throw new NotImplementedException();
        }
    }
}