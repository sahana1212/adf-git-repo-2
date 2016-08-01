using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Visual_Novel_Manager.Model
{
    public class AddVnModel: DependencyObject
    {
        public string VnId
        {
            get { return (string)GetValue(VnIdProperty); }
            set { SetValue(VnIdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VnId.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VnIdProperty =
            DependencyProperty.Register("VnId", typeof(string), typeof(AddVnModel), new PropertyMetadata(null));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(AddVnModel), new PropertyMetadata(null));


        public string ExePath
        {
            get { return (string)GetValue(ExePathProperty); }
            set { SetValue(ExePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExePathProperty =
            DependencyProperty.Register("ExePath", typeof(string), typeof(AddVnModel), new PropertyMetadata(null));



        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(AddVnModel), new PropertyMetadata(null));




        public double ProgressBar
        {
            get { return (double)GetValue(ProgressBarProperty); }
            set { SetValue(ProgressBarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressBar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressBarProperty =
            DependencyProperty.Register("ProgressBar", typeof(double), typeof(AddVnModel), new PropertyMetadata(0.0));


        public string ProgressPercent
        {
            get { return (string)GetValue(ProgressPercentProperty); }
            set { SetValue(ProgressPercentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressPercent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressPercentProperty =
            DependencyProperty.Register("ProgressPercent", typeof(string), typeof(AddVnModel), new PropertyMetadata("0%"));






    }
}
