using System.Windows;
using System.Windows.Media.Imaging;

namespace Visual_Novel_Manager.Model
{
    public class VnInfoModel : DependencyObject
    {
        /// <summary>
        /// remember to set property meta to appropriate value for property
        /// </summary>

        #region Non Collection Properties
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string Original
        {
            get { return (string)GetValue(OriginalProperty); }
            set { SetValue(OriginalProperty, value); }
        }
        public static readonly DependencyProperty OriginalProperty =
            DependencyProperty.Register("Original", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));

        public BitmapSource VnImage
        {
            get { return (BitmapSource)GetValue(VnImageProperty); }
            set { SetValue(VnImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VnImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VnImageProperty =
            DependencyProperty.Register("VnImage", typeof(BitmapSource), typeof(VnInfoModel), new PropertyMetadata(null));

        


        public string Aliases
        {
            get { return (string)GetValue(AliasesProperty); }
            set { SetValue(AliasesProperty, value); }
        }
        public static readonly DependencyProperty AliasesProperty =
            DependencyProperty.Register("Aliases", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string Released
        {
            get { return (string)GetValue(ReleasedProperty); }
            set { SetValue(ReleasedProperty, value); }
        }
        public static readonly DependencyProperty ReleasedProperty =
            DependencyProperty.Register("Released", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string Length
        {
            get { return (string)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string Developers
        {
            get { return (string)GetValue(DevelopersProperty); }
            set { SetValue(DevelopersProperty, value); }
        }
        public static readonly DependencyProperty DevelopersProperty =
            DependencyProperty.Register("Developers", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string Publishers
        {
            get { return (string)GetValue(PublishersProperty); }
            set { SetValue(PublishersProperty, value); }
        }
        public static readonly DependencyProperty PublishersProperty =
            DependencyProperty.Register("Publishers", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string Rating
        {
            get { return (string)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }
        public static readonly DependencyProperty RatingProperty =
            DependencyProperty.Register("Rating", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string Popularity
        {
            get { return (string)GetValue(PopularityProperty); }
            set { SetValue(PopularityProperty, value); }
        }
        public static readonly DependencyProperty PopularityProperty =
            DependencyProperty.Register("Popularity", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string VnDescription
        {
            get { return (string)GetValue(VnDescriptionProperty); }
            set { SetValue(VnDescriptionProperty, value); }
        }
        public static readonly DependencyProperty VnDescriptionProperty =
            DependencyProperty.Register("VnDescription", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));




        public string PlayTime
        {
            get { return (string)GetValue(PlayTimeProperty); }
            set { SetValue(PlayTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayTimeProperty =
            DependencyProperty.Register("PlayTime", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));



        public string LastPlayed
        {
            get { return (string)GetValue(LastPlayedProperty); }
            set { SetValue(LastPlayedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastPlayed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastPlayedProperty =
            DependencyProperty.Register("LastPlayed", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));

        

        #endregion


        #region Relations Collection


        public string RelTitle
        {
            get { return (string)GetValue(RelTitleProperty); }
            set { SetValue(RelTitleProperty, value); }
        }
        public static readonly DependencyProperty RelTitleProperty =
            DependencyProperty.Register("RelTitle", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string RelOriginal
        {
            get { return (string)GetValue(RelOriginalProperty); }
            set { SetValue(RelOriginalProperty, value); }
        }
        public static readonly DependencyProperty RelOriginalProperty =
            DependencyProperty.Register("RelOriginal", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        public string RelRelations
        {
            get { return (string)GetValue(RelRelationsProperty); }
            set { SetValue(RelRelationsProperty, value); }
        }
        public static readonly DependencyProperty RelRelationsProperty =
            DependencyProperty.Register("RelRelations", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        #endregion


        #region Tag Collection


        public string TagName
        {
            get { return (string)GetValue(TagNameProperty); }
            set { SetValue(TagNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TagName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TagNameProperty =
            DependencyProperty.Register("TagName", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));




        public string TagDescription
        {
            get { return (string)GetValue(TagDescriptionProperty); }
            set { SetValue(TagDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TagDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TagDescriptionProperty =
            DependencyProperty.Register("TagDescription", typeof(string), typeof(VnInfoModel), new PropertyMetadata(""));


        #endregion
    }
}
