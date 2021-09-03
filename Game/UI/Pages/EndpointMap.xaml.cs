using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace Game.UI.Pages
{
    /// <summary>
    /// Interaction logic for EndpointMap.xaml
    /// </summary>
    public partial class EndpointMap : Game.UI.Pages.DisplayablePage
    {
        private Dictionary<Guid, Endpoint> DrawnEndpointsDict = new();
        private Dictionary<Guid, Button> ButtonDict = new();

        //private Dictionary<Guid, StackPanel> StackPanelDict = new();
        private Dictionary<Guid, TextBlock> TextBlockDict = new();

        private Dictionary<Guid, Point> PointDict = new();
        private Polyline Polyline = new();
        private bool isConnected = false;

        public EndpointMap()
        {
            this.Loaded += EndpointMap_Loaded;
            InitializeComponent();
            Map.Children.Add(Polyline);
            Polyline.Stroke = System.Windows.Media.Brushes.Red;
            Polyline.StrokeDashArray = new DoubleCollection(new double[] { 2, 3, 2 });
        }

        private void EndpointMap_Loaded(object sender, RoutedEventArgs e)
        {
            this.Map_Loaded(sender, e);
        }

        public void FilterOnEndpoint(string Ip)
        {
            foreach (Endpoint e in this.DrawnEndpointsDict.Values)
            {
                if (!e.IPAddress.Contains(Ip) && !(e.IPAddress == Ip))
                {
                    ButtonDict[e.Id].Background = Brushes.Yellow;
                    continue;
                }
                ButtonDict[e.Id].Background = Brushes.Violet;
            }
        }

        public void FilterOnEndpoint(Guid EndpointID)
        {
            foreach (Guid id in TextBlockDict.Keys)
            {
                if (id != EndpointID)
                {
                    ButtonDict[id].Background = Brushes.Yellow;
                    continue;
                }
                ButtonDict[id].Background = Brushes.Violet;
            }
        }

        public void FilterOnEndpoint(Endpoint e)
        {
            foreach (Guid id in TextBlockDict.Keys)
            {
                if (id != e.Id)
                {
                    ButtonDict[id].Background = Brushes.Yellow;
                    continue;
                }
                ButtonDict[id].Background = Brushes.Violet;
            }
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

            //
            //.WriteLine("Drawing:" + (Guid)(sender as Button).Tag);
            Guid id = (Guid)(sender as Button).Tag;

            if (!DrawnEndpointsDict.ContainsKey(id))
            {
                throw new Exception("Clicked endpoint is not rendered on the map? How was it clicked?");
            }
            if (DrawnEndpointsDict[id].IsLocalEndpoint)
            {
                return;
            }

            Global.Bounce.ToggleBounde(DrawnEndpointsDict[id]);
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
            Button last = this.ButtonDict[
                Global.Bounce.BounceList[
                    Global.Bounce.BounceList.Count - 1]
                    .Id];
            last.BorderThickness = new Thickness(2d);
            last.BorderBrush = Brushes.Red;
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
            if (Global.Bounce.BounceList.Count == 0)
            {
            }
            Button last = this.ButtonDict
                [
                    Global.Bounce.BounceList
                    [
                        Global.Bounce.BounceList.Count - 1
                    ].Id
                ];
            last.BorderThickness = new Thickness(0);
            last.BorderBrush = Brushes.Transparent;

            this.Polyline.StrokeDashArray = new DoubleCollection(new double[] { 2, 3, 2 });
            EnableEndpointButtons();
            DrawBouncePath();

            foreach (Button btn in this.ButtonDict.Values)
            {
                btn.Background = Brushes.Yellow;
            }
        }

        private void RadioButtonShow_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Guid id in this.TextBlockDict.Keys)
            {
                if (DrawnEndpointsDict[id].isHidden)
                {
                    continue;
                }
                TextBlockDict[id].Visibility = Visibility.Visible;
            }
        }

        private void RadioButtonHide_Checked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock txb in this.TextBlockDict.Values)
            {
                txb.Visibility = Visibility.Hidden;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            this.FilterOnEndpoint(textBox.Text);
            if (textBox.Text == string.Empty)
            {
                foreach (Button btn in this.ButtonDict.Values)
                {
                    btn.Background = Brushes.Yellow;
                }
            }
        }
    }
}