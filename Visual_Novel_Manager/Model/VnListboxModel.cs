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


        public int VnSelectedIndex
        {
            get { return (int)GetValue(VnSelectedIndexProperty); }
            set { SetValue(VnSelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VnSelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VnSelectedIndexProperty =
            DependencyProperty.Register("VnSelectedIndex", typeof(int), typeof(VnListboxModel), new PropertyMetadata(0));


        public int CategorySelectedIndex
        {
            get { return (int)GetValue(CategorySelectedIndexProperty); }
            set { SetValue(CategorySelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategorySelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategorySelectedIndexProperty =
            DependencyProperty.Register("CategorySelectedIndex", typeof(int), typeof(VnListboxModel), new PropertyMetadata(-1));

        public string CategorySelectedItem
        {
            get { return (string)GetValue(CategorySelectedItemProperty); }
            set { SetValue(CategorySelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategorySelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategorySelectedItemProperty =
            DependencyProperty.Register("CategorySelectedItem", typeof(string), typeof(VnListboxModel), new PropertyMetadata(null));



    }
}
