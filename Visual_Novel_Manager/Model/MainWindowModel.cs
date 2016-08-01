using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Visual_Novel_Manager.Model
{
    public class MainWindowModel: DependencyObject
    {
        public string CategoryText
        {
            get { return (string)GetValue(CategoryTextProperty); }
            set { SetValue(CategoryTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CategoryText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CategoryTextProperty =
            DependencyProperty.Register("CategoryText", typeof(string), typeof(MainWindowModel), new PropertyMetadata(null));


    }
}
