using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Visual_Novel_Manager.Model
{
    public class SettingsModel: DependencyObject
    {
        public int VnSpoilerIndex
        {
            get { return (int)GetValue(VnSpoilerIndexProperty); }
            set { SetValue(VnSpoilerIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VnSpoilerIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VnSpoilerIndexProperty =
            DependencyProperty.Register("VnSpoilerIndex", typeof(int), typeof(SettingsModel), new PropertyMetadata(0));


        public int CharacterSpoilerIndex
        {
            get { return (int)GetValue(CharacterSpoilerIndexProperty); }
            set { SetValue(CharacterSpoilerIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharacterSpoilerIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterSpoilerIndexProperty =
            DependencyProperty.Register("CharacterSpoilerIndex", typeof(int), typeof(SettingsModel), new PropertyMetadata(0));

        public int NsfwEnabledIndex
        {
            get { return (int)GetValue(NsfwEnabledIndexProperty); }
            set { SetValue(NsfwEnabledIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NsfwEnabledIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NsfwEnabledIndexProperty =
            DependencyProperty.Register("NsfwEnabledIndex", typeof(int), typeof(SettingsModel), new PropertyMetadata(0));





    }
}
