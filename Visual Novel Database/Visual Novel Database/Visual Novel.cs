using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using System.Net;

namespace Visual_Novel_Database
{
    //These class contains all information deemed necessary
    //for a Visual Novel
    public class Visual_Novel
    {
        public string englishName { get; set; } //VN Name
        public string originalName { get; set; } //Original VN name
        public string tags { get; set; } //All tags in string format (seperated with ',')
        public string description { get; set; } //VN description
        public string path { get; set; } //Path to VN executeable file
        public int id { get; set; } //VN id from vndb.org

        public int tag_hit { get; set; } //Amount of tags matching the tag search
        public int sort_index { get; set; } //Index accoring to search algorithm (determines placement in the ListBox)

        public List<string> Images { get; set; } //Contains https links to all images from vndb.org
        public List<string> Char_Images { get; set; } //Contains all links to character images from vndb.org
        public List<string> Category { get; set; } //Contains all categories the VN belongs to

        #region constructors

        public Visual_Novel()
        {
            Images = new List<string>();
            Char_Images = new List<string>();
            Category = new List<string>();
        }

        public Visual_Novel(string Name, string oName, string tag, string descripe, string pathing, int vn_id)
        {

            englishName = Name;
            originalName = oName;
            tags = tag;
            description = descripe;
            path = pathing;
            id = vn_id;
            Images = GetImages("https://vndb.org/v" + id); //Use the 'GetImages' function to get screenshots               
            Char_Images = GetImages("https://vndb.org/v" + id + "/chars#chars"); //and character images
            Category = new List<string>();
          
            Char_Images.RemoveAt(0); //Remove VN main image from character images

            saveInformation(); //and save the information in "VisualNovels.json"
        }

        #endregion constructors

        #region methods

        //Save all information in "VisualNovels.json
        private void saveInformation()
        {
            FileStream aFile = new FileStream("VisualNovels.json", FileMode.OpenOrCreate);
            aFile.Dispose();

            if (File.Exists("VisualNovels.json") == true)
            {
                bool add_all = true;
                List<Visual_Novel> existingVNS; 

                existingVNS = JsonConvert.DeserializeObject<List<Visual_Novel>>(File.ReadAllText(@"VisualNovels.json")); //Read existing .json file

                if (existingVNS == null)
                    existingVNS = new List<Visual_Novel>();

                foreach (string cat in this.Category) //Check if VN has category 'All'
                    if (cat.ToLower() == "all")
                        add_all = false;

                if (add_all == true) //if not add it to it
                    this.Category.Add("All");

                existingVNS.Add(this); //add new VN to existing VN's

                File.WriteAllText(@"VisualNovels.json", JsonConvert.SerializeObject(existingVNS)); //and save it again 
            }
        }

        //This function is from a user from stackoverflow...
        private List<string> GetImages(string website)
        {
            //Get all images from link

            WebClient y = new WebClient();
            List<string> images = new List<string>();
            string source = "";     
      
            source = y.DownloadString(website);

            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();

            document.LoadHtml(source);

            foreach (var link in document.DocumentNode
                            .Descendants("img")
                            .Select(x => x.Attributes["src"]))
            {

                images.Add(link.Value.ToString());
            }

            for (int i = 0; i < images.Count - 1; i++)
                images[i] = images[i].Replace("st", "sf");

            return images;
        }

        #endregion methods
    }
}
