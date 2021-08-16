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
        private Dictionary<Guid, Endpoint> DrawnEndpointsDict = new Dictionary<Guid, Endpoint>();
        private Dictionary<Guid, Button> GuidButtonDict = new Dictionary<Guid, Button>();
        private Dictionary<Guid, Point> PointDict = new Dictionary<Guid, Point>();
        private PointCollection points = new PointCollection();
        private Polyline Polyline = new Polyline();
        private bool isConnected = false;

        public EndpointMap()
        {
            InitializeComponent();
            map.Children.Add(Polyline);
            Polyline.Stroke = System.Windows.Media.Brushes.Red;
        }

        public void DisplayEndpoints()
        {
            foreach (Endpoint e in Global.AllEndpoints)
            {
                //Image img = new Image();
                //img.RenderSize = new Size(16, 16);
                //img.Source = new BitmapImage(new Uri(@"C:\Users\J.Vedder Desktop\source\repos\EventSystemTesting\UI\Icons\server.png"));

                StackPanel stp = new StackPanel();
                Button btn = new Button();
                btn.Click += EndpointClick;
                btn.Background = Brushes.Yellow;
                btn.BorderThickness = new Thickness(0);
                btn.BorderBrush = Brushes.Transparent;
                if (e.IsLocalEndpoint)
                {
                    btn.Background = Brushes.Red;
                }
                btn.Tag = e.Id;
                stp.Children.Add(btn);
                TextBlock txb = new TextBlock();
                txb.Text = e.IPAddress;
                stp.Children.Add(txb);
                int x = e.x;
                int y = e.y;
                (x, y) = GetRelativeCoordinates(e);
                Canvas.SetLeft(stp, x);
                Canvas.SetTop(stp, y);
                //Debug.WriteLine("ID: " + e.Id + " X:" + x + " Y:" + y);
                map.Children.Add(stp);
                this.DrawnEndpointsDict[e.Id] = e;
                this.GuidButtonDict[e.Id] = btn;
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

            Button from = GuidButtonDict[Global.StartEndPoint.Id];
            var parent = VisualTreeHelper.GetParent(from) as UIElement;
            PointDict[Global.StartEndPoint.Id] = new Point(Canvas.GetLeft(parent), Canvas.GetTop(parent));
            Polyline.Points.Add(PointDict[Global.StartEndPoint.Id]);

            foreach (Endpoint e in Global.Bounce.BounceList)
            {
                var button = GuidButtonDict[e.Id];
                var too = VisualTreeHelper.GetParent(button) as UIElement;

                PointDict[e.Id] = new Point(Canvas.GetLeft(too), Canvas.GetTop(too));
                Polyline.Points.Add(PointDict[e.Id]);
            }
        }

        private (int x, int y) GetRelativeCoordinates(Endpoint e)
        {
            double Boundx = map.ActualWidth;
            double Boundy = map.ActualHeight;
            double x = ((Boundx - 40) / (double)EndpointGenerator.XMax) * (double)e.x + 20d;
            double y = ((Boundy - 40) / (double)EndpointGenerator.YMax) * (double)e.y + 20d;
            return ((int)x, (int)y);
        }

        private int GetRelativeSize()
        {
            return (int)(Convert.ToDouble(EndpointGenerator.EndpointBaseSize) / 1080 * map.ActualWidth);
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            Global.App.OnMapReady();
        }

        private void map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //ScaleTransform scaleTransform1 = new ScaleTransform(0.9, 0.9);
            //map.RenderTransform = scaleTransform1;

            StackPanel b = new StackPanel();
            for (int i = 0; i < map.Children.Count; i++)
            {
                object o = map.Children[i];
                if (o.GetType() != b.GetType())
                {
                    continue;
                }
                b = (StackPanel)map.Children[i];
                Guid id = (Guid)(b.Children[0] as Button).Tag;

                (int x, int y) = GetRelativeCoordinates(DrawnEndpointsDict[id]);
                Canvas.SetLeft(b, x);
                Canvas.SetTop(b, y);
                //b.Height = GetRelativeSize();
                //b.Width = GetRelativeSize();

                if (PointDict.ContainsKey(id))
                {
                    Point p = PointDict[id];
                    p.X = x;
                    p.Y = y;
                }
            }
            DrawBouncePath();
        }

        public void DisplayConnection()
        {
            this.Polyline.StrokeDashArray = new DoubleCollection(new double[] { 2, 3, 2 });
            DisableEndpointButtons();
            Button last = this.GuidButtonDict[
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
            Button last = this.GuidButtonDict[
                Global.Bounce.BounceList[
                    Global.Bounce.BounceList.Count - 1]
                    .Id];
            last.BorderThickness = new Thickness(0);
            last.BorderBrush = Brushes.Transparent;

            this.Polyline.StrokeDashArray = new DoubleCollection();
            EnableEndpointButtons();
            DrawBouncePath();
        }
    }
}