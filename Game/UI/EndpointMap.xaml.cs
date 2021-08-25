using Game.Core.Endpoints;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;

namespace Game.UI
{
    /// <summary>
    /// Interaction logic for EndpointMap.xaml
    /// </summary>
    public partial class EndpointMap : System.Windows.Controls.Page
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
                if(id != EndpointID)
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
                //Variables
                StackPanel stp = new StackPanel();
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
                txb.Text = e.IPAddress;

                //Location
                int x = e.x/10;
                int y = e.y/10;
                Canvas.SetLeft(btn, x);
                Canvas.SetTop(btn, y);
                Map.Children.Add(btn);
                Canvas.SetLeft(txb, x);
                Canvas.SetTop(txb, y-6);
                Map.Children.Add(txb);
                if (e.isHidden)
                {
                    btn.Visibility = Visibility.Hidden;
                }
            }
        }

        private void EndpointClick(object sender, RoutedEventArgs e)
        {
            //No map editing when we are in a connection.
            if (isConnected)
            {
                return;
            }

            Debug.WriteLine("Drawing:" + (Guid)(sender as Button).Tag);
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
            if (Global.StartEndPoint == null)
            {
                return;
            }

            var from = ButtonDict[Global.StartEndPoint.Id];
            PointDict[Global.StartEndPoint.Id] = new Point(Canvas.GetLeft(from) +1.5, Canvas.GetTop(from) + 1.5);
            Polyline.Points.Add(PointDict[Global.StartEndPoint.Id]);

            foreach (Endpoint e in Global.Bounce.BounceList)
            {
                var too = ButtonDict[e.Id];

                PointDict[e.Id] = new Point(Canvas.GetLeft(too) +1.5, Canvas.GetTop(too) +1.5);
                Polyline.Points.Add(PointDict[e.Id]);
            }
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            Global.App.OnMapReady();
        }

        public void DisplayConnection()
        {
            this.Polyline.StrokeDashArray = new DoubleCollection(new double[] { 2, 3, 2 });
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

            //foreach (Button btn in this.GuidButtonDict.Values)
            //{
            //    btn.IsEnabled = false;
            //}
        }

        private void EnableEndpointButtons()
        {
            this.isConnected = false;
            //foreach (Button btn in this.GuidButtonDict.Values)
            //{
            //    btn.IsEnabled = true;
            //}
        }

        public void UnmakeConnection()
        {
            Button last = this.ButtonDict[
                Global.Bounce.BounceList[
                    Global.Bounce.BounceList.Count - 1]
                    .Id];
            last.BorderThickness = new Thickness(0);
            last.BorderBrush = Brushes.Transparent;

            this.Polyline.StrokeDashArray = new DoubleCollection();
            EnableEndpointButtons();
            DrawBouncePath();
        }

        private void RadioButtonShow_Checked(object sender, RoutedEventArgs e)
        {
            foreach(TextBlock txb in this.TextBlockDict.Values)
            {
                txb.Visibility = Visibility.Visible;
            }
        }

        private void RadioButtonHide_Checked(object sender, RoutedEventArgs e)
        {
            foreach (TextBlock txb in this.TextBlockDict.Values)
            {
                txb.Visibility = Visibility.Collapsed;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            this.FilterOnEndpoint(textBox.Text);
            if(textBox.Text == string.Empty)
            {
                foreach(Button btn in this.ButtonDict.Values)
                {
                    btn.Background = Brushes.Yellow;
                }
            }
        }
    }
}