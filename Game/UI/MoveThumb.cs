using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Game.UI
{
    public class MoveThumb : Thumb
    {
        double MX = 0;
        double MY = 0;
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
            if (programWindow.maxed)
            {
                Debug.WriteLine("DragDelta maxed = true");
                programWindow.Width = programWindow.WindowedWidth;
                programWindow.Height = programWindow.WindowedHeight;
                Canvas.SetLeft(programWindow, MX);
                Canvas.SetTop(programWindow, MY);
                programWindow.MaxButton.ToggleButtonText();
                programWindow.maxed = false;
                return;
            }

            double left = Math.Min(Math.Max(Canvas.GetLeft(programWindow) + e.HorizontalChange, 0), Global.MainWindow.MainCanvas.ActualWidth - programWindow.ActualWidth);
            double top = Math.Min(Math.Max(Canvas.GetTop(programWindow) + e.VerticalChange, 0), Global.MainWindow.MainCanvas.ActualHeight);

            Debug.WriteLine(left);
            Debug.WriteLine(top);

            Canvas.SetLeft(programWindow, left);
            Canvas.SetTop(programWindow, top);
        }
    }
}