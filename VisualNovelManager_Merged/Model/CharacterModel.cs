using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisualNovelManager.Model
{
    public class CharacterModel : DependencyObject
    {

        #region static properties
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));





        public string Original
        {
            get { return (string)GetValue(OriginalProperty); }
            set { SetValue(OriginalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Original.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalProperty =
            DependencyProperty.Register("Original", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Aliases
        {
            get { return (string)GetValue(AliasesProperty); }
            set { SetValue(AliasesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Aliases.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AliasesProperty =
            DependencyProperty.Register("Aliases", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));





        public string Gender
        {
            get { return (string)GetValue(GenderProperty); }
            set { SetValue(GenderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Gender.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GenderProperty =
            DependencyProperty.Register("Gender", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string BloodType
        {
            get { return (string)GetValue(BloodTypeProperty); }
            set { SetValue(BloodTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BloodType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BloodTypeProperty =
            DependencyProperty.Register("BloodType", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Birthday
        {
            get { return (string)GetValue(BirthdayProperty); }
            set { SetValue(BirthdayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Birthday.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BirthdayProperty =
            DependencyProperty.Register("Birthday", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Bust
        {
            get { return (string)GetValue(BustProperty); }
            set { SetValue(BustProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Bust.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BustProperty =
            DependencyProperty.Register("Bust", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Waist
        {
            get { return (string)GetValue(WaistProperty); }
            set { SetValue(WaistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Waist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaistProperty =
            DependencyProperty.Register("Waist", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Hips
        {
            get { return (string)GetValue(HipsProperty); }
            set { SetValue(HipsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hips.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HipsProperty =
            DependencyProperty.Register("Hips", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Height
        {
            get { return (string)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Weight
        {
            get { return (string)GetValue(WeightProperty); }
            set { SetValue(WeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Weight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register("Weight", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));




        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));






        public BitmapSource CharImage
        {
            get { return (BitmapSource)GetValue(CharImageProperty); }
            set { SetValue(CharImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CharImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharImageProperty =
            DependencyProperty.Register("CharImage", typeof(BitmapSource), typeof(CharacterModel), new PropertyMetadata(null));

        
        #endregion




        #region Traits
        public string TraitName
        {
            get { return (string)GetValue(TraitNameProperty); }
            set { SetValue(TraitNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraitName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraitNameProperty =
            DependencyProperty.Register("TraitName", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));



        public string TraitDescription
        {
            get { return (string)GetValue(TraitDescriptionProperty); }
            set { SetValue(TraitDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraitDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraitDescriptionProperty =
            DependencyProperty.Register("TraitDescription", typeof(string), typeof(CharacterModel), new PropertyMetadata(""));

        #endregion



    }
}
