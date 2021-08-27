using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Game.UI
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
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
                return;
            }
            double left = Canvas.GetLeft(programWindow);
            double top = Canvas.GetTop(programWindow);

            Canvas.SetLeft(programWindow, left + e.HorizontalChange);
            Canvas.SetTop(programWindow, top + e.VerticalChange);
        }
    }
}