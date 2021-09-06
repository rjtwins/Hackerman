using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Game.UI
{
    public class MoveThumb : Thumb
    {
        //private double MX = 0;
        //private double MY = 0;

        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
            //PreviewMouseDown += MoveThumb_PreviewMouseDown;
            Loaded += MoveThumb_Loaded;
        }

        private void MoveThumb_Loaded(object sender, RoutedEventArgs e)
        {
            ProgramWindow programWindow = DataContext as ProgramWindow;
            programWindow.MoveThumb = this;
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
                return;
            }

            double verticalChange = e.VerticalChange;
            double horizontalChange = e.HorizontalChange;

            Debug.WriteLine(Canvas.GetLeft(programWindow));
            Debug.WriteLine(Canvas.GetTop(programWindow));

            double left = Math.Min(Math.Max(Canvas.GetLeft(programWindow) + horizontalChange, 0), Global.MainWindow.MainCanvas.ActualWidth - programWindow.ActualWidth);
            double top = Math.Min(Math.Max(Canvas.GetTop(programWindow) + verticalChange, 0), Global.MainWindow.MainCanvas.ActualHeight);

            Canvas.SetLeft(programWindow, left);
            Canvas.SetTop(programWindow, top);
        }
    }
}