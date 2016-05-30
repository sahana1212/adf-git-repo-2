using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VisualNovelManager.Model
{
    public class ReleasesModel: DependencyObject
    {
        #region collection
        public string ReleaseTitle
        {
            get { return (string)GetValue(ReleaseTitleProperty); }
            set { SetValue(ReleaseTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReleaseTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReleaseTitleProperty =
            DependencyProperty.Register("ReleaseTitle", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));

        
        #endregion



        #region static properties


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));





        public string Original
        {
            get { return (string)GetValue(OriginalProperty); }
            set { SetValue(OriginalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Original.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OriginalProperty =
            DependencyProperty.Register("Original", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));





        public string Released
        {
            get { return (string)GetValue(ReleasedProperty); }
            set { SetValue(ReleasedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Released.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReleasedProperty =
            DependencyProperty.Register("Released", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));






        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string Patch
        {
            get { return (string)GetValue(PatchProperty); }
            set { SetValue(PatchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Patch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PatchProperty =
            DependencyProperty.Register("Patch", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string Freeware
        {
            get { return (string)GetValue(FreewareProperty); }
            set { SetValue(FreewareProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Freeware.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FreewareProperty =
            DependencyProperty.Register("Freeware", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string Doujin
        {
            get { return (string)GetValue(DoujinProperty); }
            set { SetValue(DoujinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Doujin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DoujinProperty =
            DependencyProperty.Register("Doujin", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));




        public string Languages
        {
            get { return (string)GetValue(LanguagesProperty); }
            set { SetValue(LanguagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Languages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LanguagesProperty =
            DependencyProperty.Register("Languages", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));




        public string Notes
        {
            get { return (string)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Notes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotesProperty =
            DependencyProperty.Register("Notes", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));




        public string Website
        {
            get { return (string)GetValue(WebsiteProperty); }
            set { SetValue(WebsiteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Website.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WebsiteProperty =
            DependencyProperty.Register("Website", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string MinAge
        {
            get { return (string)GetValue(MinAgeProperty); }
            set { SetValue(MinAgeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinAge.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinAgeProperty =
            DependencyProperty.Register("MinAge", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));




        public string Gtin
        {
            get { return (string)GetValue(GtinProperty); }
            set { SetValue(GtinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Gtin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GtinProperty =
            DependencyProperty.Register("Gtin", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string Catalog
        {
            get { return (string)GetValue(CatalogProperty); }
            set { SetValue(CatalogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Catalog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CatalogProperty =
            DependencyProperty.Register("Catalog", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string Platforms
        {
            get { return (string)GetValue(PlatformsProperty); }
            set { SetValue(PlatformsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Platforms.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlatformsProperty =
            DependencyProperty.Register("Platforms", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string Media
        {
            get { return (string)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Media.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MediaProperty =
            DependencyProperty.Register("Media", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));





        public BitmapSource VnImage
        {
            get { return (BitmapSource)GetValue(VnImageProperty); }
            set { SetValue(VnImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VnImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VnImageProperty =
            DependencyProperty.Register("VnImage", typeof(BitmapSource), typeof(ReleasesModel), new PropertyMetadata(null));

        


        //public string VnImage
        //{
        //    get { return (string)GetValue(VnImageProperty); }
        //    set { SetValue(VnImageProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for VnImage.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty VnImageProperty =
        //    DependencyProperty.Register("VnImage", typeof(string), typeof(ReleasesModel), new PropertyMetadata(null));

        









        public string ProducerName
        {
            get { return (string)GetValue(ProducerNameProperty); }
            set { SetValue(ProducerNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProducerName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProducerNameProperty =
            DependencyProperty.Register("ProducerName", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string ProducerOriginal
        {
            get { return (string)GetValue(ProducerOriginalProperty); }
            set { SetValue(ProducerOriginalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProducerOriginal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProducerOriginalProperty =
            DependencyProperty.Register("ProducerOriginal", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string ProducerDeveloper
        {
            get { return (string)GetValue(ProducerDeveloperProperty); }
            set { SetValue(ProducerDeveloperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProducerDeveloper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProducerDeveloperProperty =
            DependencyProperty.Register("ProducerDeveloper", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));



        public string ProducerPublisher
        {
            get { return (string)GetValue(ProducerPublisherProperty); }
            set { SetValue(ProducerPublisherProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProducerPublisher.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProducerPublisherProperty =
            DependencyProperty.Register("ProducerPublisher", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));





        public string ProducerType
        {
            get { return (string)GetValue(ProducerTypeProperty); }
            set { SetValue(ProducerTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProducerType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProducerTypeProperty =
            DependencyProperty.Register("ProducerType", typeof(string), typeof(ReleasesModel), new PropertyMetadata(""));

        

        

        
        
        
        
        
        

        

        
        #endregion

    }
}
