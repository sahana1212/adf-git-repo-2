using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Visual_Novel_Manager.ConnectionClient;
using Visual_Novel_Manager.JSON;

namespace Visual_Novel_Manager
{
    class AddToDatabase
    {
        private static string _vndbId;
        private static string _vnExePath;
        private static string _vnName;
        private static string _vnIcon;
        /// <summary>
        /// /
        /// </summary>        

        int _characterCount;



        readonly List<string[]> _tagList = new List<string[]>();

        private List<BasicItem> _basicItem;
        private List<DetailsItem> _detailsItem;
        private List<VNRelationItem> _vnrelationItem;
        private List<ReleaseItem> _vnreleaseItem;
        private List<StatsItem> _statsItem;
        private List<CharacterItem> _characterItem;
        private List<TagsItem> _tagsItem;
        private List<CharacterItem> _characterListArray;
        private string screenlist = null;






        public static void CreateAddVnVariables(string id, string exepath, string name, string icon)
        {
            _vndbId = id;
            _vnExePath = exepath;
            _vnName = name;
            _vnIcon = icon;
        }






        #region fetch data from API

        internal async void fetch_vn_basic()
        {

            #region backup
            Connection conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));

            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }

            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get vn basic (id = " + _vndbId + " )"));
                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {

                    BasicRootObject basicInformation = JsonConvert.DeserializeObject<BasicRootObject>(conn.JsonResponse); //deserialize it
                    _basicItem = basicInformation.items;
                    /////
                    await conn.Close();
                    break;
                }
            }
            #endregion


        }

        internal async void fetch_vn_details()
        {
            Connection conn = new Connection();

            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));
            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }

            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get vn details (id = " + _vndbId + " )"));
                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    DetailsRootObject detailedInformation = JsonConvert.DeserializeObject<DetailsRootObject>(conn.JsonResponse); //deserialize it
                    _detailsItem = detailedInformation.items;
                    /////
                    await conn.Close();
                    break;
                }
            }

        }

        internal async void fetch_vn_relations()
        {
            Connection conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));

            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }

            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get vn relations (id = " + _vndbId + " )"));
                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    VnRelationRootObject vnrelationInformation = JsonConvert.DeserializeObject<VnRelationRootObject>(conn.JsonResponse); //deserialize it
                    _vnrelationItem = vnrelationInformation.items;
                    await conn.Close();
                    break;
                }
            }
        }

        internal async void fetch_vn_stats()
        {
            Connection conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));

            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }

            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get vn stats (id = " + _vndbId + " )"));
                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    StatsRootObject statsInformation = JsonConvert.DeserializeObject<StatsRootObject>(conn.JsonResponse); //deserialize it
                    _statsItem = statsInformation.items;
                    await conn.Close();
                    break;
                }
            }

        }

        internal async void fetch_vn_screens()
        {
            Connection conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));

            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }

            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get vn screens (id = " + _vndbId + " )"));
                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    ScreenRootObject screenInformation = JsonConvert.DeserializeObject<ScreenRootObject>(conn.JsonResponse); //deserialize it
                    List<Screen> screenitems = screenInformation.items[0].screens;
                    screenlist = null;
                    for (int i = 0; i < screenitems.Count; i++)
                    {
                        screenlist += "[" + screenitems[i].image + "," + screenitems[i].nsfw + "]";
                    }
                    await conn.Close();
                    break;
                }
            }
        }

        internal async void fetch_releases()
        {
            Connection conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));


            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }

            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get release basic,details,vn,producers (vn = " + _vndbId + " )"));
                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    VnReleaseRootObject vnreleaseInformation = JsonConvert.DeserializeObject<VnReleaseRootObject>(conn.JsonResponse);
                    _vnreleaseItem = vnreleaseInformation.items;
                    await conn.Close();
                    break;
                }
            }
        }

        internal async void fetch_characters()
        {
            Connection conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));
            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }
            int page = 1;
            var mergelist = _characterItem;
            while (true)
            {

                responseCode = Convert.ToInt32(await conn.Query("get character basic,details,meas,traits,vns (vn=" + _vndbId + ")" + "{\"results\":25,\"page\":" + page + "}"));


                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    CharacterRootObject characterInformation = JsonConvert.DeserializeObject<CharacterRootObject>(conn.JsonResponse);
                    _characterItem = characterInformation.items;//for MLA, does 25,25, 7

                    //////////////////////////////////////////////////
                    TraitsRootObject traitsInformation = JsonConvert.DeserializeObject<TraitsRootObject>(conn.JsonResponse); //deserialize them
                    //ManipulateTraits(traitsInformation);

                    if (page == 1)
                    {
                        //mergelist = ViewModel.character_item.ToList();
                        var characterItemListLarge = characterInformation.items;
                        mergelist = characterItemListLarge;
                        mergelist = mergelist.ToList();
                    }

                    else if (page > 1)
                    {
                        //mergelist = ViewModel.character_item.Union(ViewModel.character_item_TMP).ToList();
                        mergelist = mergelist.Concat(_characterItem).ToList();
                        //mergelist = ViewModel.character_item_TMP.Concat(ViewModel.character_item).ToList();
                        Thread.Sleep(0);
                    }


                    if (characterInformation.more == false)//break the loop
                    {
                        _characterCount = 1;
                        break;
                    }
                    page++;
                }






            }
            _characterCount = mergelist.Count;
            _characterListArray = mergelist;
            /////
            await conn.Close();
        }

        internal async void fetch_tags()
        {

            List<int> TagIdList = new List<int>();
            var conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));

            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }
            /////


            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get vn tags (id = " + _vndbId + " )"));
                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    TagsRootObject tagsInformation = JsonConvert.DeserializeObject<TagsRootObject>(conn.JsonResponse); //deserialize them
                    _tagsItem = tagsInformation.items;

                    for (int i = 0; i < _tagsItem[0].tags.Count; i++) //add all tags for the vn to a list array
                    {
                        TagIdList.Add(Convert.ToInt32(_tagsItem[0].tags[i][0]));
                    }



                    int count = 0;
                    foreach (int tmp in TagIdList) //Change our 'int'-tags into matching 'string'-tags
                    {
                        foreach (WrittenTagsRootObject tmp2 in StaticClass.PlainTags)
                        {
                            if ((Convert.ToInt32(tmp) == tmp2.id))
                            {
                                _tagList.Add(new[] { TagIdList[count].ToString(), tmp2.name, _tagsItem[0].tags[count][2].ToString() });
                                count++;
                            }
                        }
                    }
                    await conn.Close();
                    break;
                }
            }
        }

        internal async void fetch_traits()
        {
            List<TraitsItem> traitsItem = null;
            Connection conn = new Connection();
            await conn.Open();
            int responseCode = Convert.ToInt32(await conn.Login(null, null));
            if (responseCode == -1)
            {
                MessageBox.Show("Error while logging in. Response: " + conn.JsonResponse, "Login Error", MessageBoxButton.OK);
            }
            int page = 1;
            while (true)
            {
                responseCode = Convert.ToInt32(await conn.Query("get character basic,traits (vn=" + _vndbId + ")" + "{\"results\":25,\"page\":" + page + "}"));


                if (responseCode == -1)
                {
                    var error = JsonConvert.DeserializeObject<ErrorRootObject>(conn.JsonResponse);
                    if (error.id == "throttled")
                    {
                        Thread.Sleep(Convert.ToInt32(error.minwait) + 4000);
                    }
                    else
                    {
                        MessageBox.Show("Error while requesting information. Response: " + conn.JsonResponse, "Query Error", MessageBoxButton.OK);
                    }
                }
                if (responseCode == 0)
                {
                    CharacterRootObject characterInformation = JsonConvert.DeserializeObject<CharacterRootObject>(conn.JsonResponse);
                    _characterItem = characterInformation.items;//for MLA, does 25,25, 7
                    //////////////////////////////////////////////////
                    TraitsRootObject traitsInformation = JsonConvert.DeserializeObject<TraitsRootObject>(conn.JsonResponse); //deserialize them
                    //ManipulateTraits(traitsInformation);



                    if (page == 1)
                    {
                        ManipulateTraits(traitsInformation, characterInformation);
                    }
                    else if (page > 1)
                    {
                        ManipulateTraits(traitsInformation, characterInformation);
                    }


                    if (traitsInformation.more == false)//break the loop
                    {
                        ManipulateTraits(traitsInformation, characterInformation);
                        break;
                    }
                    page++;
                    await conn.Close();
                    break;
                }
            }
        }
        #endregion











        #region add data to sql database



        internal void AddNovelData()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO NovelPath(Novel, ExePath, VnId, IconPath, LastPlayed, PlayTime) VALUES(@Novel, @ExePath, @VnId, @IconPath, @LastPlayed, @PlayTime)", con);

                    if (_vnName == "" | _vnName == null)
                    {
                        cmd.Parameters.AddWithValue("@Novel", CheckForDbNull(_basicItem[0].title));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Novel", CheckForDbNull(_vnName));
                    }

                    cmd.Parameters.AddWithValue("@ExePath", CheckForDbNull(_vnExePath));
                    cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(_basicItem[0].id));
                    cmd.Parameters.AddWithValue("@IconPath", CheckForDbNull(_vnIcon));
                    cmd.Parameters.AddWithValue("@LastPlayed", "");
                    cmd.Parameters.AddWithValue("@PlayTime", "0,0,0,0");

                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
        }

        internal void AddNovelCategory()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO NovelCategories(VnId, Category) VALUES(@VnId, @Category)",con);
                    cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(_basicItem[0].id));
                    cmd.Parameters.AddWithValue("@Category", "All");
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }           
        }

        internal void AddVnApi()
        {

            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO VnAPI(VnId, title, original, released, languages, OrigLang, platforms, aliases, length, description, links, image, ImageNsfw, popularity, rating, screenshots) " +
                        "VALUES(@VnId, @title, @original, @released, @languages, @OrigLang, @platforms, @aliases, @length, @description, @links, @image, @ImageNsfw, @popularity, @rating, @screenshots)", con);

                    cmd.Parameters.AddWithValue("@VnId", CheckForDbNull(_basicItem[0].id));
                    cmd.Parameters.AddWithValue("@title", CheckForDbNull(_basicItem[0].title));
                    cmd.Parameters.AddWithValue("@original", CheckForDbNull(_basicItem[0].original));
                    cmd.Parameters.AddWithValue("@released", CheckForDbNull(_basicItem[0].released));
                    var vnlang = String.Join(",", _basicItem[0].languages); //use this to join an array into comma delimited string
                    cmd.Parameters.AddWithValue("@languages", CheckForDbNull(vnlang));
                    var vnOriginalLanguage = String.Join(",", _basicItem[0].orig_lang);
                    cmd.Parameters.AddWithValue("@OrigLang", CheckForDbNull(vnOriginalLanguage));
                    var vnPlatforms = String.Join(",", _basicItem[0].platforms);
                    cmd.Parameters.AddWithValue("@platforms", CheckForDbNull(vnPlatforms));
                    cmd.Parameters.AddWithValue("@aliases", CheckForDbNull(_detailsItem[0].aliases));
                    cmd.Parameters.AddWithValue("@length", CheckForDbNull(_detailsItem[0].length));
                    cmd.Parameters.AddWithValue("@description", CheckForDbNull(_detailsItem[0].description));
                    cmd.Parameters.AddWithValue("@links", CheckForDbNull("[" + _detailsItem[0].links.encubed + "," + _detailsItem[0].links.renai + "," + _detailsItem[0].links.wikipedia + "]"));
                    cmd.Parameters.AddWithValue("@image", CheckForDbNull(_detailsItem[0].image));
                    cmd.Parameters.AddWithValue("@ImageNsfw", CheckForDbNull(_detailsItem[0].image_nsfw));
                    cmd.Parameters.AddWithValue("@popularity", CheckForDbNull(_statsItem[0].popularity));
                    cmd.Parameters.AddWithValue("@rating", CheckForDbNull(_statsItem[0].rating));
                    cmd.Parameters.AddWithValue("@screenshots", CheckForDbNull(screenlist));
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }      
        }


        internal void AddVnRelations()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand("INSERT INTO VnRelations(OriginalVnId, RelatedVnId, Relation, Title, Original) VALUES(@OriginalVnId, @RelatedVnId, @Relation, @Title, @Original )",con);
                    foreach (var relation in _vnrelationItem[0].relations)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@OriginalVnId", _basicItem[0].id);
                        cmd.Parameters.AddWithValue("@RelatedVnId", relation.id);
                        cmd.Parameters.AddWithValue("@Relation", relation.relation ?? "");
                        cmd.Parameters.AddWithValue("@Title", relation.title ?? "");
                        cmd.Parameters.AddWithValue("@Original", relation.original ?? "");//null coalescing operators to deal with possible null values


                        //relationsvar += relation.id + "," + relation.original + "," + relation.relation + "," + relation.title + ")";
                        //vnrelations = relationsvar.ToString();
                        cmd.ExecuteNonQuery();
                        //cmd.Parameters.Clear();
                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            
        }


        internal void AddVnTags()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    for (int i = 0; i < _tagList.Count; i++)
                    {
                        SQLiteCommand cmd = new SQLiteCommand("INSERT INTO VnTags(VnId, TagId, TagName, Spoiler) VALUES(@VnId, @TagId, @TagName, @Spoiler)", con);  
                                             
                        cmd.Parameters.AddWithValue("@VnId", _basicItem[0].id);
                        cmd.Parameters.AddWithValue("@TagId", _tagList[i][0]);
                        cmd.Parameters.AddWithValue("@TagName", _tagList[i][1]);
                        cmd.Parameters.AddWithValue("@Spoiler", _tagList[i][2]);

                        cmd.ExecuteNonQuery();
                        //tag id list, for each index has these:
                        //index [0] is the tag id
                        //index [1] is the tag name
                        //index [2] is the tag spoiler
                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }


            
        }

        /// <summary>
        /// If I use it, put AddVnAnime() here
        /// </summary>

        internal void AddReleaseApi()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    for (int j = 0; j < _vnreleaseItem.Count; j++)
                    {
                        SQLiteCommand cmd = new SQLiteCommand("INSERT INTO ReleaseAPI(VnId, ReleaseId, ProducerId, Title, Original, Released, Type, Patch, Freeware, Doujin, Languages, Website, Notes, MinAge, GTIN," +
                        " Catalog, Platforms, Media, ProducerName, ProducerOriginal, ProducerDeveloper, ProducerPublisher, ProducerType) VALUES(@VnId, @ReleaseId, @ProducerId, @Title, @Original, @Released, @Type, @Patch, @Freeware," +
                        " @Doujin, @Languages, @Website, @Notes, @MinAge, @GTIN, @Catalog, @Platforms, @Media, @ProducerName, @ProducerOriginal, @ProducerDeveloper, @ProducerPublisher, @ProducerType)", con);
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@VnId", _basicItem[0].id);
                        cmd.Parameters.AddWithValue("@ReleaseId", _vnreleaseItem[j].id);
                        cmd.Parameters.AddWithValue("@Title", CheckForDbNull(_vnreleaseItem[j].title));
                        cmd.Parameters.AddWithValue("@Original", CheckForDbNull(_vnreleaseItem[j].original));
                        cmd.Parameters.AddWithValue("@Released", CheckForDbNull(_vnreleaseItem[j].released));
                        cmd.Parameters.AddWithValue("@Type", CheckForDbNull(_vnreleaseItem[j].type));
                        cmd.Parameters.AddWithValue("@Patch", CheckForDbNull(_vnreleaseItem[j].patch));
                        cmd.Parameters.AddWithValue("@Freeware", CheckForDbNull(_vnreleaseItem[j].freeware));
                        cmd.Parameters.AddWithValue("@Doujin", CheckForDbNull(_vnreleaseItem[j].doujin));
                        string releaseLanguage = String.Join(",", _vnreleaseItem[j].languages);
                        cmd.Parameters.AddWithValue("@Languages", CheckForDbNull(releaseLanguage));
                        cmd.Parameters.AddWithValue("@Website", CheckForDbNull(_vnreleaseItem[j].website));
                        cmd.Parameters.AddWithValue("@Notes", CheckForDbNull(_vnreleaseItem[j].notes));
                        cmd.Parameters.AddWithValue("@MinAge", CheckForDbNull(_vnreleaseItem[j].minage));
                        cmd.Parameters.AddWithValue("@GTIN", CheckForDbNull(_vnreleaseItem[j].gtin));
                        cmd.Parameters.AddWithValue("@Catalog", CheckForDbNull(_vnreleaseItem[j].catalog));

                        if (_vnreleaseItem[j].platforms == null)
                        {
                            cmd.Parameters.AddWithValue("@Platforms", CheckForDbNull(null));
                        }
                        else
                        {
                            string relplatform = String.Join(",", _vnreleaseItem[j].platforms);
                            cmd.Parameters.AddWithValue("Platforms", CheckForDbNull(relplatform));
                        }

                        if (_vnreleaseItem[j].media.Count < 1)
                        {
                            cmd.Parameters.AddWithValue("@Media", CheckForDbNull(null));
                        }
                        else
                        {
                            ReleaseMedia releaseMedia = JsonConvert.DeserializeObject<ReleaseMedia>(_vnreleaseItem[j].media[0].ToString());
                            var media = releaseMedia.medium + "," + releaseMedia.qty;


                            string relmedia = String.Join(",", media);
                            cmd.Parameters.AddWithValue("@Media", CheckForDbNull(relmedia));
                        }

                        if (!(_vnreleaseItem[j].producers.Count < 1))
                        {
                            cmd.Parameters.AddWithValue("@ProducerId", CheckForDbNull(_vnreleaseItem[j].producers[0].id));//check for null values
                            cmd.Parameters.AddWithValue("@ProducerDeveloper", CheckForDbNull(_vnreleaseItem[j].producers[0].developer));
                            cmd.Parameters.AddWithValue("@ProducerPublisher", CheckForDbNull(_vnreleaseItem[j].producers[0].publisher));
                            cmd.Parameters.AddWithValue("@ProducerName", CheckForDbNull(_vnreleaseItem[j].producers[0].name));
                            cmd.Parameters.AddWithValue("@ProducerOriginal", CheckForDbNull(_vnreleaseItem[j].producers[0].original));
                            cmd.Parameters.AddWithValue("@ProducerType", CheckForDbNull(_vnreleaseItem[j].producers[0].type));
                            cmd.ExecuteNonQuery();
                        }


                    }


                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }           
        }




        internal void AddCharacterApi()
        {
            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    string bdmonth = null;
                    string bdday = null;
                    for (int j = 0; j < _characterCount; j++)
                    {
                        int vnsArr = 0;
                        if (_characterListArray[j].birthday[1] != null)
                        {
                            bdmonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_characterListArray[j].birthday[1].Value).ToString();
                        }
                        else if (_characterListArray[j].birthday[1] == null)
                        {
                            bdmonth = "";
                        }
                        if (_characterListArray[j].birthday[0] != null)
                        {
                            bdday = _characterListArray[j].birthday[0].Value.ToString();
                        }
                        else if (_characterListArray[j].birthday[0] == null)
                        {
                            bdday = "";
                        }
                        //CheckForDbNull(character_list_arr[j].birthday[0].Value)
                        var charbd = bdmonth + " " + bdday;

                        foreach (var vns in _characterListArray[j].vns)
                        {
                            if (Convert.ToInt32(vns[0]) == _basicItem[0].id)
                            {
                                vnsArr = Convert.ToInt32(vns[2]);
                            }
                            //this should not have an else, there should be only one spoiler per character
                        }

                        SQLiteCommand cmd = new SQLiteCommand("INSERT INTO CharacterAPI(VnId, CharacterId, Name, Original, Gender, BloodType, Birthday, Aliases, Description, Image, Bust, Waist, Hip, Height, Weight, vns)" +
                        " VALUES(@VnId, @CharacterId, @Name, @Original, @Gender, @BloodType, @Birthday, @Aliases, @Description, @Image, @Bust, @Waist, @Hip, @Height, @Weight, @vns)", con);
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@VnId", _basicItem[0].id);
                        cmd.Parameters.AddWithValue("@CharacterId", CheckForDbNull(_characterListArray[j].id));
                        cmd.Parameters.AddWithValue("@Name", CheckForDbNull(_characterListArray[j].name));
                        cmd.Parameters.AddWithValue("@Original", CheckForDbNull(_characterListArray[j].original));
                        cmd.Parameters.AddWithValue("@Gender", CheckForDbNull(_characterListArray[j].gender));
                        cmd.Parameters.AddWithValue("@BloodType", CheckForDbNull(_characterListArray[j].bloodt));
                        cmd.Parameters.AddWithValue("@Birthday", CheckForDbNull(charbd));
                        cmd.Parameters.AddWithValue("@Aliases", CheckForDbNull(_characterListArray[j].aliases));
                        cmd.Parameters.AddWithValue("@Description", CheckForDbNull(_characterListArray[j].description));
                        cmd.Parameters.AddWithValue("@Image", CheckForDbNull(_characterListArray[j].image));
                        cmd.Parameters.AddWithValue("@Bust", CheckForDbNull(_characterListArray[j].bust));
                        cmd.Parameters.AddWithValue("@Waist", CheckForDbNull(_characterListArray[j].waist));
                        cmd.Parameters.AddWithValue("@Hip", CheckForDbNull(_characterListArray[j].hip));
                        cmd.Parameters.AddWithValue("@Height", CheckForDbNull(_characterListArray[j].height));
                        cmd.Parameters.AddWithValue("@Weight", CheckForDbNull(_characterListArray[j].weight));
                        cmd.Parameters.AddWithValue("@vns", CheckForDbNull(vnsArr));

                        cmd.ExecuteNonQuery();
                    }



                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }           
        }

        internal void AddCharacterTraits()
        {

            using (SQLiteConnection con = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                con.Open();
                try
                {
                    for (int j = 0; j < TraitList.Count; j++)
                    {
                        for (int h = 0; h < TraitList[j].Count; h++)
                        {
                            SQLiteCommand cmd = new SQLiteCommand("INSERT INTO CharacterTraits(VnId, CharacterId, Character, TraitId, TraitName, SpoilerLevel) " +
                                "VALUES(@VnId, @CharacterId, @Character, @TraitId, @TraitName, @SpoilerLevel)", con);
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@VnId", _basicItem[0].id);
                            cmd.Parameters.AddWithValue("@CharacterId", CheckForDbNull(TraitList[j][h][0]));
                            cmd.Parameters.AddWithValue(@"Character", CheckForDbNull(TraitList[j][h][1]));
                            cmd.Parameters.AddWithValue(@"TraitId", CheckForDbNull(TraitList[j][h][2]));
                            cmd.Parameters.AddWithValue(@"TraitName", CheckForDbNull(TraitList[j][h][3]));
                            cmd.Parameters.AddWithValue(@"SpoilerLevel", CheckForDbNull(TraitList[j][h][4]));
                            cmd.ExecuteNonQuery();
                        }
                        //in TraitList[#], [0]is character id, [1] is trait id, [2] is trait name, [3] is spoiler level
                    }
                    con.Close();
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex2)
                {
                    Console.WriteLine(ex2.Message);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }           
        }



        #endregion







        static List<List<string[]>> TraitList = new List<List<string[]>>();//[0]is character id, [1] is trait id, [2] is trait name, [3] is spoiler level




        private static void ManipulateTraits(TraitsRootObject traitsInformation, CharacterRootObject characterInformation)
        {
            //List<WrittenTraitsRootObject> plainTraits = null; //Contains all plain_tags, meaning the loaded tagdump.json
            List<TraitsItem> traitsItem = null;
            List<List<int>> TraitIdList = new List<List<int>>();


            List<List<string[]>> tempList = new List<List<string[]>>();

            traitsItem = traitsInformation.items;

            for (int i = 0; i < traitsItem.Count; ++i)
            {
                var innerList = new List<int>();
                TraitIdList.Add(innerList);


            }

            for (int j = 0; j < traitsItem.Count; j++)
            {
                for (int i = 0; i < traitsItem[j].traits.Count; i++) //Store them in textbox
                {
                    TraitIdList[j].Add(Convert.ToInt32(traitsItem[j].traits[i][0]));
                }

            }

            for (int i = 0; i < traitsItem.Count; ++i)
            {
                var inlst = new List<string[]>();

                //YYY.Add(inlst);
                //YYY[i].Add(new string[]{});

                tempList.Add(inlst);
            }





            int h = 0;
            int traitIdCounter = 0;
            int g = 0;
            for (int i = 0; i < traitsItem.Count; i++)
            {
                foreach (var tmp in TraitIdList[h])
                {
                    foreach (WrittenTraitsRootObject tmp2 in StaticClass.PlainTraits)
                    {
                        if (Convert.ToInt32(tmp) == tmp2.id)
                        {
                            if (!(g >= traitsItem.Count))
                            {
                                tempList[h].Add(new string[] { traitsItem[g].id.ToString(), characterInformation.items[g].name, tmp2.id.ToString(), tmp2.name, traitsItem[g].traits[traitIdCounter][1].ToString() });
                                //traitIdCounter++;
                            }

                        }
                        ////ANY var++ VARIABLE HERE WILL END UP AS THE SIZE OF THE TAG DUMP!!!!
                    }

                    //traitIdCounter = 0;
                    //g++;
                    traitIdCounter++;
                }

                h++;
                //g = 0;
                g++;
                traitIdCounter = 0;
            }

            TraitList.AddRange(tempList);
            tempList.Clear();

        }





        public object CheckForDbNull(object value)       //this forces entries that are null to work in the database
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            return value;
        }




    }
}
