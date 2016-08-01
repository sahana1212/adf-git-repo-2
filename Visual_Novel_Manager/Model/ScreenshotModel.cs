using System.Windows;
using System.Windows.Media.Imaging;

namespace Visual_Novel_Manager.Model
{
    public class ScreenshotModel: DependencyObject
    {



        //public string Screenshot
        //{
        //    get { return (string)GetValue(ScreenshotProperty); }
        //    set { SetValue(ScreenshotProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Screenshot.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty ScreenshotProperty =
        //    DependencyProperty.Register("Screenshot", typeof(string), typeof(ScreenshotModel), new PropertyMetadata(null));



        public BitmapSource Screenshot
        {
            get { return (BitmapSource)GetValue(ScreenshotProperty); }
            set { SetValue(ScreenshotProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Screenshot.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScreenshotProperty =
            DependencyProperty.Register("Screenshot", typeof(BitmapSource), typeof(ScreenshotModel), new PropertyMetadata(null));

        



       

        

        
    }
}
