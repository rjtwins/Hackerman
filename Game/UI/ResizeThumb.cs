using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Game.UI
{
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ProgramWindow programWindow = (ProgramWindow)this.DataContext;

            if (e != null)
            {
                e.Handled = true;
            }
            if (programWindow == null)
            {
                return;
            }
            if (programWindow.Maxed)
            {
                return;
            }
            double deltaVertical, deltaHorizontal;

            switch (VerticalAlignment)
            {
                case VerticalAlignment.Bottom:
                    deltaVertical = Math.Min(-e.VerticalChange, programWindow.ActualHeight - programWindow.MinHeight);
                    programWindow.Height -= deltaVertical;
                    break;

                case VerticalAlignment.Top:
                    deltaVertical = Math.Min(e.VerticalChange, programWindow.ActualHeight - programWindow.MinHeight);
                    Canvas.SetTop(programWindow, Canvas.GetTop(programWindow) + deltaVertical);
                    programWindow.Height -= deltaVertical;
                    break;

                default:
                    break;
            }

            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                    deltaHorizontal = Math.Min(e.HorizontalChange, programWindow.ActualWidth - programWindow.MinWidth);
                    Canvas.SetLeft(programWindow, Canvas.GetLeft(programWindow) + deltaHorizontal);
                    programWindow.Width -= deltaHorizontal;
                    break;

                case HorizontalAlignment.Right:
                    deltaHorizontal = Math.Min(-e.HorizontalChange, programWindow.ActualWidth - programWindow.MinWidth);
                    programWindow.Width -= deltaHorizontal;
                    break;

                default:
                    break;
            }
        }
    }
}