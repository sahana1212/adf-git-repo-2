using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VisualNovelManager.Model
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
