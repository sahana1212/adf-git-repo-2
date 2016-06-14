using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Visual_Novel_Manager.Startup;
using System.Threading;
using System.Data.SQLite;
using System.IO.Compression;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Visual_Novel_Manager.JSON;
using SplashScreen = System.Windows.SplashScreen;

namespace Visual_Novel_Manager
{
    class App : Application
    {
        [STAThread()]
        static void Main()
        {
            Splasher.Splash = new Startup.SplashScreen();
            Splasher.ShowSplash();


            //libs checking disabled while finding a better way to move .dll files to a lib folder
            #region libs
            //MessageListener.Instance.RecieveCheckLibs("Checking For Libs...");
            //string[] dll = { "lib/Newtonsoft.Json.dll", "lib/Newtonsoft.Json.dll", "lib/System.Data.SqlServerCe.dll", "lib/System.Drawing.dll", "lib/System.Management.dll" };
            //foreach (var file in dll)
            //{

            //    if (!File.Exists(file))
            //    {
            //        MessageListener.Instance.RecieveCheckLibs("[ERROR] " + file + " not found");
            //        MessageListener.Instance.RecieveCheckWarning("[ERROR] " + file + " not found");

            //        MessageBox.Show(file + " was not found! Exiting...", "Critical File Misssing", MessageBoxButton.OK,
            //            MessageBoxImage.Error);
            //        Thread.Sleep(3000);
            //        Environment.Exit(0);
            //    }
            //}

            //string[] sqldll = { "lib/x86/sqlceca40.dll", "lib/x86/sqlcecompact40.dll", "lib/x86/sqlceer40EN.dll", "lib/x86/sqlceme40.dll", "lib/x86/sqlceqp40.dll", "lib/x86/sqlcese40.dll",
            //         "lib/amd64/sqlceca40.dll", "lib/amd64/sqlcecompact40.dll", "lib/amd64/sqlceer40EN.dll", "lib/amd64/sqlceme40.dll", "lib/amd64/sqlceqp40.dll", "lib/amd64/sqlcese40.dll" };
            //Console.WriteLine(sqldll);
            //foreach (var file in sqldll)
            //{
            //    if (!File.Exists(file))
            //    {
            //        MessageListener.Instance.RecieveCheckLibs("[ERROR] " + file + " not found");
            //        MessageListener.Instance.RecieveCheckWarning("[ERROR] " + file + " not found");

            //        MessageBox.Show(file + " was not found! Exiting...", "Critical File Misssing", MessageBoxButton.OK,
            //            MessageBoxImage.Error);
            //        Thread.Sleep(3000);
            //        Environment.Exit(0);
            //    }
            //}
            //MessageListener.Instance.RecieveCheckLibs("[SUCCESS] Alll dll files present");
            #endregion


            #region config.json
            MessageListener.Instance.RecieveCheckConfig("Checking config file");
            if (!File.Exists("config.json"))
            {
                MessageListener.Instance.RecieveCheckConfig("Config file not found. Creating config file");

                JObject jsonJObject = JObject.Parse(@"
                    {
	                    'global': 
                        {
		                    'NsfwEnabled': false
	                    },
	                    'unique': 
                        [{
		                    'VnId': 0,
		                    'VnSpoilerLevel': 0,
		                    'CharacterSpoilerLevel': 0
	                    }]
                    }");


                File.AppendAllText("config.json", jsonJObject.ToString());
                var oldJson = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
                var json = JsonConvert.DeserializeObject<ConfigRootObject>(oldJson);
                StaticClass.NsfwEnabled = json.global.NsfwEnabled;


                MessageListener.Instance.RecieveCheckConfig("Config.json created");
            }
            else
            {

                var oldJson = File.ReadAllText(StaticClass.CurrentDirectory + @"\config.json");
                var json = JsonConvert.DeserializeObject<ConfigRootObject>(oldJson);
                StaticClass.NsfwEnabled = json.global.NsfwEnabled;
                MessageListener.Instance.RecieveCheckConfig("[SUCCESS] Config.json found");
            }
            
            #endregion

            #region database
            MessageListener.Instance.RecieveCheckDb("Checking for Database...");
            if (!File.Exists("Database.db"))
            {
                MessageListener.Instance.RecieveCheckDb("[ERROR] Database.db not found. Creating Database...");
                CreateDatabase();
            }
            else
            {
                MessageListener.Instance.RecieveCheckDb("[SUCCESS] Database.db found");
            }
            #endregion


            #region directory
            MessageListener.Instance.RecieveCheckFileStructure("Checking Folder Structure...");
            if (!Directory.Exists("data"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                //Directory.CreateDirectory("character");
                //Directory.CreateDirectory("cover");
                //Directory.CreateDirectory("screenshots");
            }
            if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\cover"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\cover");
            }
            if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\character"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\character");
            }
            if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\screenshots"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\screenshots");
            }
            if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\vnlist"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\vnlist");
            }
            if (!Directory.Exists("lib"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\lib");
            }
            if (!Directory.Exists("res"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\res\nsfw");
            }
            else
            {
                MessageListener.Instance.RecieveCheckFileStructure("File stucture is fine");
                Console.WriteLine(StaticClass.CurrentDirectory);
            }
            #endregion


            #region tags and traits
            MessageListener.Instance.RecieveLoadTagsandTraits("Loading Tags and Traits....");

            if (!File.Exists("data/tags.json"))
            {
                MessageListener.Instance.RecieveLoadTagsandTraits("Tags not found. Downloading tags...");
                WebClient client = new WebClient();
                client.DownloadFile(new Uri("http://vndb.org/api/tags.json.gz"), StaticClass.CurrentDirectory + @"\data\" + "tags.json.gz");

                using (FileStream fInStream = new FileStream(StaticClass.CurrentDirectory + @"\data\" + "tags.json.gz", FileMode.Open, FileAccess.Read))
                {
                    using (GZipStream zipStream = new GZipStream(fInStream, CompressionMode.Decompress))
                    {
                        using (FileStream fOutStream =
                          new FileStream(StaticClass.CurrentDirectory + @"\data\" + "tags.json", FileMode.Create, FileAccess.Write))
                        {
                            byte[] tempBytes = new byte[4096];
                            int i;
                            while ((i = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                            {
                                fOutStream.Write(tempBytes, 0, i);
                            }
                        }
                    }
                }
                File.Delete(StaticClass.CurrentDirectory + @"\data\" + "tags.json.gz");


                MessageListener.Instance.RecieveLoadTagsandTraits("Deserializing Tags");
                if (StaticClass.PlainTags == null) //Read tag-dump and deserialize it (only once -> needs time!)
                    StaticClass.PlainTags = JsonConvert.DeserializeObject<List<WrittenTagsRootObject>>(File.ReadAllText(StaticClass.CurrentDirectory + @"\data\tags.json"));

            }


            if (!File.Exists("data/traits.json"))
            {
                MessageListener.Instance.RecieveLoadTagsandTraits("Traits not found. Downloading traits...");
                WebClient client = new WebClient();
                client.DownloadFile(new Uri("http://vndb.org/api/traits.json.gz"), StaticClass.CurrentDirectory + @"\data\" + "traits.json.gz");

                using (FileStream fInStream = new FileStream(StaticClass.CurrentDirectory + @"\data\" + "traits.json.gz", FileMode.Open, FileAccess.Read))
                {
                    using (GZipStream zipStream = new GZipStream(fInStream, CompressionMode.Decompress))
                    {
                        using (FileStream fOutStream =
                          new FileStream(StaticClass.CurrentDirectory + @"\data\" + "traits.json", FileMode.Create, FileAccess.Write))
                        {
                            byte[] tempBytes = new byte[4096];
                            int i;
                            while ((i = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                            {
                                fOutStream.Write(tempBytes, 0, i);
                            }
                        }
                    }
                }
                File.Delete(StaticClass.CurrentDirectory + @"\data\" + "traits.json.gz");


                MessageListener.Instance.RecieveLoadTagsandTraits("Deserializing Traits");
                if (StaticClass.PlainTraits == null) //Read tag-dump and deserialize it (only once -> needs time!)
                    StaticClass.PlainTraits = JsonConvert.DeserializeObject<List<WrittenTraitsRootObject>>(File.ReadAllText(StaticClass.CurrentDirectory + @"\data\traits.json"));
            }

            else
            {
                MessageListener.Instance.RecieveLoadTagsandTraits("Deserializing JSON");
                if (StaticClass.PlainTags == null) //Read tag-dump and deserialize it (only once -> needs time!)
                    StaticClass.PlainTags = JsonConvert.DeserializeObject<List<WrittenTagsRootObject>>(File.ReadAllText(StaticClass.CurrentDirectory + @"\data\tags.json"));

                if (StaticClass.PlainTraits == null) //Read tag-dump and deserialize it (only once -> needs time!)
                    StaticClass.PlainTraits = JsonConvert.DeserializeObject<List<WrittenTraitsRootObject>>(File.ReadAllText(StaticClass.CurrentDirectory + @"\data\traits.json"));

            }
            #endregion


            Splasher.CloseSplash();            
            new App();
        }


        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile("Database.db");

            using (SQLiteConnection dbConnection = new SQLiteConnection("Data Source=Database.db;Version=3;"))
            {
                dbConnection.Open();

                SQLiteCommand CategoriesTable = new SQLiteCommand("CREATE TABLE [Categories] ([RowId] INTEGER NOT NULL, [Category] nvarchar(100) NULL, CONSTRAINT[PK_Categories] PRIMARY KEY([RowId]));", dbConnection);

                SQLiteCommand CharacterAPITable = new SQLiteCommand("CREATE TABLE CharacterAPI(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL, CharacterId int NULL, Name text NULL, Original text NULL," +
                " Gender text NULL, BloodType text NULL, Birthday text NULL, Aliases text NULL, Description text NULL, Image text NULL, Bust int NULL, Waist int NULL, Hip int NULL, Height int NULL, Weight int NULL, vns int NULL);", dbConnection);

                SQLiteCommand CharacterTraitsTable = new SQLiteCommand("CREATE TABLE CharacterTraits(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL, CharacterId int NULL,Character text NULL," +
                " TraitId int NULL, TraitName text NULL, SpoilerLevel int NULL);", dbConnection);

                SQLiteCommand NovelCategoriesTable = new SQLiteCommand("CREATE TABLE NovelCategories(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL,Category text NULL);", dbConnection);

                SQLiteCommand NovelPathTable = new SQLiteCommand("CREATE TABLE NovelPath(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL,Novel text NULL,ExePath text NULL," +
                 " IconPath text NULL, LastPlayed text NULL, PlayTime text NULL);", dbConnection);

                SQLiteCommand ReleaseAPITable = new SQLiteCommand("CREATE TABLE ReleaseAPI(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL, ReleaseId int NULL, ProducerId int NULL," +
                " Title text NULL, Original text NULL, Released text NULL, Type text NULL,Patch text NULL, Freeware text NULL, Doujin text NULL, Languages text NULL, Website text NULL," +
                " Notes text NULL, MinAge int NULL, GTIN text NULL, Catalog text NULL, Platforms text NULL, Media text NULL, ProducerName text NULL," +
                " ProducerOriginal text NULL, ProducerDeveloper text NULL, ProducerPublisher text null, ProducerType text NULL);", dbConnection);

                SQLiteCommand VnAnimeTable = new SQLiteCommand("CREATE TABLE VnAnime(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL, AniDb int NULL, ANN innt null," +
                " NfoId int NULL, RomajiTitle text NULL, KanjiTitle text NULL, Year int NULL, Type text NULL);", dbConnection);

                SQLiteCommand VnAPITable = new SQLiteCommand("CREATE TABLE VnAPI(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL,Title text NULL, Original text NULL, Released text NULL," +
                " Languages text NULL,OrigLang text NULL, Platforms text NULL, Aliases text NULL, Length tinyint NULL, Description text NULL, Links text NULL, Image text NULL, ImageNsfw text NULL," +
                " Popularity numeric(5,2) NULL, Rating numeric(5,2) NULL, Screenshots text NULL);", dbConnection);

                SQLiteCommand VnTagsTable = new SQLiteCommand("CREATE TABLE VnTags(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL,TagId int NULL, TagName text NULL," +
                " Spoiler int NULL, Score numeric(5,2) NULL);", dbConnection);

                SQLiteCommand VnRelationsTable = new SQLiteCommand("CREATE TABLE VnRelations(RowId INTEGER CONSTRAINT PK PRIMARY KEY, OriginalVnId int NULL, RelatedVnId int NULL, Relation text NULL," +
                " Title text NULL, Original text NULL);", dbConnection);

                SQLiteCommand VnListTable = new SQLiteCommand("CREATE TABLE VnList(RowId INTEGER CONSTRAINT PK PRIMARY KEY, VnId int NULL, Title text NULL, NSFW text NULL, Vote text NULL," +
                " Status text NULL, Note text NULL);", dbConnection);

                SQLiteCommand AddCat = new SQLiteCommand("INSERT INTO Categories(Category) VALUES(@Category)", dbConnection);//add All into Categories
                AddCat.Parameters.AddWithValue("@Category", "All");

                try
                {
                    CategoriesTable.ExecuteNonQuery();
                    CharacterAPITable.ExecuteNonQuery();
                    CharacterTraitsTable.ExecuteNonQuery();
                    NovelCategoriesTable.ExecuteNonQuery();
                    NovelPathTable.ExecuteNonQuery();
                    ReleaseAPITable.ExecuteNonQuery();
                    VnAnimeTable.ExecuteNonQuery();
                    VnAPITable.ExecuteNonQuery();
                    VnTagsTable.ExecuteNonQuery();
                    VnRelationsTable.ExecuteNonQuery();
                    VnListTable.ExecuteNonQuery();

                    AddCat.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    throw;
                }
                dbConnection.Close();
            }            
            
        }

        public App()
        {
            StartupUri = new System.Uri("MainWindow.xaml", UriKind.Relative);
            Run();
        }
    }
}
