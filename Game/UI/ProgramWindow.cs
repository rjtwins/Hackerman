using System.Windows;
using System.Windows.Controls;

namespace Game.UI
{
    public class ProgramWindow : ContentControl
    {
        public bool maxed = false;

        public double WindowedWidth = 0;
        public double WindowedHeight = 0;
        public double WindowedLeft = 0;
        public double WindowedTop = 0;

        #region DependencyProperties

        public Image icon
        {
            get { return (Image)GetValue(iconProperty); }
            set { SetValue(iconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty iconProperty = DependencyProperty.Register("icon", typeof(Image), typeof(ProgramWindow), null);

        public TextBlock title
        {
            get { return (TextBlock)GetValue(titleProperty); }
            set { SetValue(titleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty titleProperty = DependencyProperty.Register("title", typeof(TextBlock), typeof(ProgramWindow), null);

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty maxButtonProperty = DependencyProperty.Register("maxButton", typeof(Button), typeof(ProgramWindow), null);

        public object mainContent
        {
            get { return GetValue(mainContentProperty); }
            set { SetValue(mainContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty mainContentProperty = DependencyProperty.Register("mainContent", typeof(object), typeof(ProgramWindow), null);

        #endregion DependencyProperties
    }

    public class ProgramWindowNoButtons : ProgramWindow
    {
    }
}