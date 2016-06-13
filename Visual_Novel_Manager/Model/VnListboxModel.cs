using System.Windows;
using System.Windows.Media.Imaging;

namespace Visual_Novel_Manager.Model
{
    public class VnListboxModel :DependencyObject
    {

        

        public string ItemName
        {
            get { return (string)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemNameProperty =
            DependencyProperty.Register("ItemName", typeof(string), typeof(VnListboxModel), new PropertyMetadata(""));



        public BitmapSource ItemIcon
        {
            get { return (BitmapSource)GetValue(ItemIconProperty); }
            set { SetValue(ItemIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemIconProperty =
            DependencyProperty.Register("ItemIcon", typeof(BitmapSource), typeof(VnListboxModel));

        

    }
}
