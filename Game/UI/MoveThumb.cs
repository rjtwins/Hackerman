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
            Control designerItem = this.DataContext as Control;

            if (designerItem == null)
            {
                return;
            }
            if ((bool)designerItem.Resources["Maxed"])
            {
                return;
            }
            double left = Canvas.GetLeft(designerItem);
            double top = Canvas.GetTop(designerItem);

            Canvas.SetLeft(designerItem, left + e.HorizontalChange);
            Canvas.SetTop(designerItem, top + e.VerticalChange);
        }
    }
}