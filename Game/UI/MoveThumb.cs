using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Game.UI
{
    public class MoveThumb : Thumb
    {
        private double MX = 0;
        private double MY = 0;
        private bool Windofied = false;

        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
            PreviewMouseDown += MoveThumb_PreviewMouseDown;
        }

        private void MoveThumb_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.MX = e.GetPosition(Global.MainWindow.MainCanvas).X;
            this.MY = e.GetPosition(Global.MainWindow.MainCanvas).Y;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ProgramWindow programWindow = (ProgramWindow)this.DataContext;

            if (programWindow == null)
            {
                return;
            }
            if (programWindow.Maxed)
            {
                Windofied = true;
                Debug.WriteLine("DragDelta maxed = true");
                programWindow.Width = programWindow.WindowedWidth;
                programWindow.Height = programWindow.WindowedHeight;
                Canvas.SetLeft(programWindow, MX);
                Canvas.SetTop(programWindow, MY);
                e.Handled = true;
                Debug.WriteLine(Canvas.GetLeft(programWindow));
                Debug.WriteLine(Canvas.GetTop(programWindow));

                programWindow.Maxed = false;

                //AutomationPeer button1AP = UIElementAutomationPeer.CreatePeerForElement(this);
                //(button1AP.GetPattern(PatternInterface.Invoke) as IInvokeProvider).Invoke();

                this.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                return;
            }

            if (Windofied)
            {
                Windofied = false;
                return;
            }

            double verticalChange = e.VerticalChange;
            double horizontalChange = e.HorizontalChange;

            //double maxChange = 20;

            //if (e.VerticalChange > maxChange)
            //{
            //    verticalChange = maxChange;
            //}
            //if (e.VerticalChange < -1* maxChange)
            //{
            //    verticalChange = -1*maxChange;
            //}
            //if (e.HorizontalChange > maxChange)
            //{
            //    horizontalChange = maxChange;
            //}
            //if (e.HorizontalChange < -1*maxChange)
            //{
            //    horizontalChange = -1*maxChange;
            //}

            Debug.WriteLine(Canvas.GetLeft(programWindow));
            Debug.WriteLine(Canvas.GetTop(programWindow));

            double left = Math.Min(Math.Max(Canvas.GetLeft(programWindow) + horizontalChange, 0), Global.MainWindow.MainCanvas.ActualWidth - programWindow.ActualWidth);
            double top = Math.Min(Math.Max(Canvas.GetTop(programWindow) + verticalChange, 0), Global.MainWindow.MainCanvas.ActualHeight);

            Canvas.SetLeft(programWindow, left);
            Canvas.SetTop(programWindow, top);
        }
    }
}