using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Game.UI
{
    class TaskBarButton : ToggleButton
    {

        Rectangle left1;
        Rectangle left2;
        Rectangle left3;

        Rectangle top1;
        Rectangle top2;
        Rectangle top3;

        Rectangle right1;
        Rectangle right2;
        Rectangle right3;

        Rectangle bottom1;
        Rectangle bottom2;
        Rectangle bottom3;

        public TaskBarButton()
        {
            Loaded += TaskBarButton_Loaded;
            this.Checked += TaskBarButton_Checked;
            this.Unchecked += TaskBarButton_Unchecked;
        }

        private void TaskBarButton_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            left1.Fill = Brushes.Silver;
            left2.Fill = Brushes.Gray;
            left3.Fill = Brushes.Black;

            top1.Fill = Brushes.Silver;
            top2.Fill = Brushes.Gray;
            top3.Fill = Brushes.Black;

            right1.Fill = Brushes.LightGray;
            right2.Fill = Brushes.White;
            right3.Fill = Brushes.Silver;

            bottom1.Fill = Brushes.LightGray;
            bottom2.Fill = Brushes.White;
            bottom3.Fill = Brushes.Silver;
        }

        private void TaskBarButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }
            left1.Fill = Brushes.LightGray;
            left2.Fill = Brushes.White;
            left3.Fill = Brushes.Silver;

            top1.Fill = Brushes.LightGray;
            top2.Fill = Brushes.White;
            top3.Fill = Brushes.Silver;

            right1.Fill = Brushes.Silver;
            right2.Fill = Brushes.Gray;
            right3.Fill = Brushes.Black;

            bottom1.Fill = Brushes.Silver;
            bottom2.Fill = Brushes.Gray;
            bottom3.Fill = Brushes.Black;
        }
        private void TaskBarButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.Template.FindName("Image", this) as Image).Source = new BitmapImage(new Uri(this.Uid, UriKind.Relative));

            left1 = this.Template.FindName("r4", this) as Rectangle;
            left2 = this.Template.FindName("r5", this) as Rectangle;
            left3 = this.Template.FindName("r6", this) as Rectangle;

            top1 = this.Template.FindName("r1", this) as Rectangle;
            top2 = this.Template.FindName("r2", this) as Rectangle;
            top3 = this.Template.FindName("r3", this) as Rectangle;

            right1 = this.Template.FindName("r10", this) as Rectangle;
            right2 = this.Template.FindName("r11", this) as Rectangle;
            right3 = this.Template.FindName("r12", this) as Rectangle;

            bottom1 = this.Template.FindName("r9", this) as Rectangle;
            bottom2 = this.Template.FindName("r8", this) as Rectangle;
            bottom3 = this.Template.FindName("r7", this) as Rectangle;
        }
    }
}
