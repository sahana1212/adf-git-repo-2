using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VisualNovelManager.JSON;
using VisualNovelManager.Startup;
using SplashScreen = System.Windows.SplashScreen;


namespace VisualNovelManager
{
    class App : Application
    {
        [STAThread()]
        static void Main()
        {
            Splasher.Splash = new Startup.SplashScreen();
            Splasher.ShowSplash();
            ////startup code goes under here/////



            #region libs
            MessageListener.Instance.RecieveCheckLibs("Checking For Libs...");
            string[] dll = { "lib/Newtonsoft.Json.dll", "lib/Newtonsoft.Json.dll", "lib/System.Data.SqlServerCe.dll", "lib/System.Drawing.dll", "lib/System.Management.dll" };
            foreach (var file in dll)
            {

                if (!File.Exists(file))
                {
                    MessageListener.Instance.RecieveCheckLibs("[ERROR] " + file + " not found");
                    MessageListener.Instance.RecieveCheckWarning("[ERROR] " + file + " not found");

                    MessageBox.Show(file + " was not found! Exiting...", "Critical File Misssing", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
            }

            string[] sqldll = { "lib/x86/sqlceca40.dll", "lib/x86/sqlcecompact40.dll", "lib/x86/sqlceer40EN.dll", "lib/x86/sqlceme40.dll", "lib/x86/sqlceqp40.dll", "lib/x86/sqlcese40.dll",
                     "lib/amd64/sqlceca40.dll", "lib/amd64/sqlcecompact40.dll", "lib/amd64/sqlceer40EN.dll", "lib/amd64/sqlceme40.dll", "lib/amd64/sqlceqp40.dll", "lib/amd64/sqlcese40.dll" };
            Console.WriteLine(sqldll);
            foreach (var file in sqldll)
            {
                if (!File.Exists(file))
                {
                    MessageListener.Instance.RecieveCheckLibs("[ERROR] " + file + " not found");
                    MessageListener.Instance.RecieveCheckWarning("[ERROR] " + file + " not found");

                    MessageBox.Show(file + " was not found! Exiting...", "Critical File Misssing", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
            }
            MessageListener.Instance.RecieveCheckLibs("[SUCCESS] Alll dll files present");
            #endregion



            #region config.json
            MessageListener.Instance.RecieveCheckConfig("Checking config file");
            if (!File.Exists("config.json"))
            {
                MessageListener.Instance.RecieveCheckConfig("Config file not found. Creating config file");

                JObject jsonJObject= JObject.Parse(@"
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






            MessageListener.Instance.RecieveCheckDb("Checking for Database...");
            if (!File.Exists("Database.sdf"))
            {
                MessageListener.Instance.RecieveCheckDb("[ERROR] db.sdf not found. Creating Database...");
                CreateDatabase();
            }
            else
            {
                MessageListener.Instance.RecieveCheckDb("[SUCCESS] db.sdf found");
            }





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
             if(!Directory.Exists(StaticClass.CurrentDirectory+ @"\data\screenshots"))
            {
                MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\screenshots");
            }
             if (!Directory.Exists(StaticClass.CurrentDirectory + @"\data\vnlist"))
             {
                 MessageListener.Instance.RecieveCheckFileStructure("Creating Directory");
                 Directory.CreateDirectory(StaticClass.CurrentDirectory + @"\data\vnlist");
             }
            if(!Directory.Exists("lib"))
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



             ////startup code goes above here//////
            Splasher.CloseSplash();
            new App();
        }

        /// <summary>
        /// put my methods below here:
        /// </summary>

        private static void CreateDatabase()
        {

            string connectionString = "DataSource=\"Database.sdf\"";
            SqlCeEngine en = new SqlCeEngine(connectionString);
            en.CreateDatabase();
            using (SqlCeConnection conn = new SqlCeConnection(connectionString))
            {


                SqlCeCommand CategoriesTable = new SqlCeCommand("CREATE TABLE Categories (RowId int IDENTITY NOT NULL, Category nvarchar(50) NULL, PRIMARY KEY (RowId));", conn);

                SqlCeCommand CharacterAPITable = new SqlCeCommand("CREATE TABLE CharacterAPI (RowId int IDENTITY NOT NULL, VnId int NULL, CharacterId int NULL, Name nvarchar(500) NULL," +
                " Original nvarchar(500) NULL, Gender nvarchar(100) NULL, BloodType nvarchar(100) NULL, Birthday nvarchar(100) NULL, Aliases nvarchar(2000) NULL, Description nvarchar(4000) NULL, Image nvarchar(100) NULL," +
                "Bust int NULL, Waist int NULL, Hip int NULL, Height int null, Weight int null, vns int null, PRIMARY KEY(RowId) );", conn);

                SqlCeCommand CharacterTraitsTable = new SqlCeCommand("CREATE TABLE CharacterTraits(RowId int IDENTITY NOT NULL, VnId int NULL, CharacterId int NULL, Character nvarchar(300), TraitId int NULL, " +
                "TraitName nvarchar(500) NULL, SpoilerLevel int NULL, PRIMARY KEY(RowId));", conn);               

                SqlCeCommand NovelCategoriesTable = new SqlCeCommand("CREATE TABLE [NovelCategories] ([RowId] int IDENTITY NOT NULL, [VnId] int NULL, [Category] nvarchar(50) NULL, PRIMARY KEY(RowId))", conn);

                SqlCeCommand NovelPathTable = new SqlCeCommand("CREATE TABLE [NovelPath] ([RowId] int IDENTITY NOT NULL, [VnId] int NULL, [Novel] nvarchar(250) NOT NULL, [ExePath] nvarchar(270) NULL," +
                " [IconPath] nvarchar(270) NULL, [LastPlayed] nvarchar(300) NULL, [PlayTime] nvarchar(500) NULL, PRIMARY KEY(RowId))", conn);

                SqlCeCommand ReleaseAPITable = new SqlCeCommand("CREATE TABLE [ReleaseAPI] ([RowId] int IDENTITY NOT NULL, [VnId] int NULL, [ReleaseId] int NULL, [ProducerId] int NULL, [Title] nvarchar(500) NULL, [Original] nvarchar(1000) NULL," +
                " [Released] nvarchar(300) NULL, [Type] nvarchar(100) NULL, [Patch] nvarchar(100) NULL, [Freeware] nvarchar(100) NULL, [Doujin] nvarchar(100) NULL, [Languages] nvarchar(300) NULL," +
                " [Website] nvarchar(1500) NULL, [Notes] nvarchar(3000) NULL, [MinAge] int NULL, [GTIN] nvarchar(100) NULL, [Catalog] nvarchar(100) NULL, [Platforms] nvarchar(500) NULL, [Media] nvarchar(1000) NULL, [ProducerName] nvarchar(200) NULL, " +
                "[ProducerOriginal] nvarchar(200) NULL, [ProducerDeveloper] nvarchar(100) NULL, [ProducerPublisher] nvarchar(100) NULL, [ProducerType] nvarchar(100) NULL,  PRIMARY KEY(RowId))", conn);

                SqlCeCommand VnAnimeTable = new SqlCeCommand("CREATE TABLE [VnAnime] ([RowId] int IDENTITY NOT NULL, [VnId] int NULL, [AniDb Id] int NULL, [ANN Id] int NULL, [Nfo Id] int NULL," +
                " [Romaji Title] nvarchar(500) NULL, [Kanji Title] nvarchar(500) NULL, [Year] int NULL, [Type] nvarchar(1000) NULL, PRIMARY KEY(RowId))", conn);

                SqlCeCommand VnAPITable = new SqlCeCommand("CREATE TABLE [VnAPI] ([RowId] int IDENTITY NOT NULL, [VnId] int NULL, [title] nvarchar(250) NULL, [original] nvarchar(250) NULL," +
                " [released] nvarchar(300) NULL, [languages] nvarchar(400) NULL, [orig_lang] nvarchar(100) NULL, [platforms] nvarchar(400) NULL, [aliases] nvarchar(1000) NULL, [length] tinyint NULL," +
                " [description] nvarchar(4000) NULL, [links] nvarchar(2000) NULL, [image] nvarchar(500) NULL, [image_nsfw] nvarchar(100) NULL, [popularity] numeric(5,2) NULL," +
                " [rating] numeric(5,2) NULL, [screenshots] nvarchar(2000) NULL,  PRIMARY KEY(RowId))", conn);

                SqlCeCommand VnTagsTable = new SqlCeCommand("CREATE TABLE [VnTags] ([RowId] int IDENTITY (1,1) NOT NULL, [VnId] int NULL, [TagId] int NULL, [TagName] nvarchar(400) NULL, " +
                "[Spoiler] int NULL, [Score] numeric(5,2) NULL, PRIMARY KEY(RowId))", conn);

                SqlCeCommand VnRelationsTable = new SqlCeCommand("CREATE TABLE [VnRelations] ([RowId] int IDENTITY NOT NULL, [OriginalVnId] int NULL, [RelatedVnId] int NULL, [Relation] nvarchar(300) NULL," +
                " [Title] nvarchar(500) NULL, [Original] nvarchar(500) NULL, PRIMARY KEY(RowId))", conn);

                SqlCeCommand VnListTable= new SqlCeCommand("CREATE TABLE [VnList] ([RowId] int IDENTITY (1,1) NOT NULL, [VnId] int NULL, [Title] nvarchar(500) NULL, [NSFW] nvarchar(100) NULL, [Vote] nvarchar(100) NULL," +
                " [Status] nvarchar(50) NULL, [Note] nvarchar(1000) NULL, PRIMARY KEY(RowId))", conn);


                SqlCeCommand AddCat = new SqlCeCommand("INSERT INTO Categories(Category) VALUES(@Category)", conn);//add All into Categories
                AddCat.Parameters.AddWithValue("@Category", "All");

                try
                {
                    conn.Open();

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
                }
                finally
                {
                    conn.Close();
                }
            }




        }

























        /// <summary>
        /// put my methods above here:
        /// </summary>


        public App()
        {
            StartupUri = new System.Uri("MainWindow.xaml", UriKind.Relative);

            Run(); 
        }
    }
}
