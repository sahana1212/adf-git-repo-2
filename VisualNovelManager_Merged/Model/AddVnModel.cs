using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualNovelManager.Model
{
    public class AddVnModel: DependencyObject
    {


        public bool IsAddVnProgressVisible
        {
            get { return (bool)GetValue(IsAddVnProgressVisibleProperty); }
            set { SetValue(IsAddVnProgressVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsAddVnProgressVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsAddVnProgressVisibleProperty =
            DependencyProperty.Register("IsAddVnProgressVisible", typeof(bool), typeof(AddVnModel), new PropertyMetadata(false));

        
    }
}
