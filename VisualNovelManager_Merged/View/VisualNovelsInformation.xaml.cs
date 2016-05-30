using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace VisualNovelManager.View
{
    /// <summary>
    /// Interaction logic for Visual_Novels_Information.xaml
    /// </summary>
    public partial class VisualNovelsInformation : UserControl
    {

        public VisualNovelsInformation()
        {
            InitializeComponent();
            DataContext = StaticClass.VnInfoViewModelStatic;//REQUIRED for databinding   
            tagdesc = this.tagdescription;
            vndesc = this.rtbDescriptionvn;
        }

        public static RichTextBox tagdesc;
        public static RichTextBox vndesc;
        readonly Stopwatch stopwatch = new Stopwatch();
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            
            Process proc = null;
            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();

                SqlCeCommand cmd = new SqlCeCommand("SELECT * FROM NovelPath WHERE RowID=" + VisualNovelsListbox.VnListBoxSelectedIndex, con);
                SqlCeDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string exepath = (string)reader["ExePath"];
                    string dirpath = System.IO.Path.GetDirectoryName(exepath);
                    Directory.SetCurrentDirectory(dirpath);
                    proc = Process.Start(exepath);                    
                    stopwatch.Start();
                    break;
                }


                con.Close();
            }
            Directory.SetCurrentDirectory(StaticClass.CurrentDirectory);
            


            var children = proc.GetChildProcesses();
            proc.EnableRaisingEvents = true;
            proc.Exited += delegate(object o, EventArgs args)
            {
                if (children.Count < 1)
                {
                    //main exe stopped
                    stopwatch.Stop();
                    MessageBox.Show(stopwatch.Elapsed.Seconds + "\n" + stopwatch.Elapsed.Minutes+ "\n"+ stopwatch.Elapsed.TotalMinutes+"\n"+ stopwatch.Elapsed.Days);

                    
                    




                    stopwatch.Reset();
                }
            };

            for (int i = 0; i < children.Count; i++)
            {
                children[i].EnableRaisingEvents = true;
                children[i].Exited += OnExited;
            }




        }

        private void OnExited(object sender, EventArgs eventArgs)
        {
            //stopwatch.Stop();
            //child exe stopped
            

            stopwatch.Stop();
            //string timePlayed = stopwatch.Elapsed.Days + "," + stopwatch.Elapsed.Hours + "," + stopwatch.Elapsed.Minutes +"," + stopwatch.Elapsed.Seconds;

            using (SqlCeConnection con = new SqlCeConnection(@"Data Source=|DataDirectory|\Database.sdf"))
            {
                con.Open();

                var getlasttimecmd = new SqlCeCommand("SELECT * FROM NovelPath WHERE VnId=@VnId", con);
                getlasttimecmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid.ToString());
                var reader = getlasttimecmd.ExecuteReader();

                string lastPlaytime = "";
                while (reader.Read())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal("PlayTime")))
                    {
                        lastPlaytime = (string)reader["PlayTime"];
                    }
                    else
                    {
                        lastPlaytime = "0,0,0,0";
                    }
                }

                var tmpSplitPlayTime = lastPlaytime.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                List<int> timecount= new List<int>();
                for (int i = 0; i < tmpSplitPlayTime.Count(); i++)
                {
                    timecount.Add(new int());
                    timecount[i] = Convert.ToInt32(tmpSplitPlayTime[i]);
                }

                TimeSpan timeSpan = new TimeSpan(timecount[0], timecount[1], timecount[2], timecount[3]);
                TimeSpan currentplaytime= new TimeSpan(stopwatch.Elapsed.Days, stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds);
                timeSpan= timeSpan.Add(currentplaytime);






                SqlCeCommand cmd = new SqlCeCommand("UPDATE NovelPath SET PlayTime=@PlayTime, LastPlayed=@LastPlayed WHERE VnId=@VnId", con);
                cmd.Parameters.AddWithValue("@PlayTime", (timeSpan.Days+","+timeSpan.Hours+","+timeSpan.Minutes+","+timeSpan.Seconds));
                cmd.Parameters.AddWithValue("@LastPlayed", DateTime.Now.ToString("M/d/yyyy"));
                cmd.Parameters.AddWithValue("@VnId", StaticClass.Vnid.ToString());
                cmd.ExecuteNonQuery();

                con.Close();
            }




            stopwatch.Reset();
        }




    }



    

   
}

public static class ProcessExtensions
{
    public static List<Process> GetChildProcesses(this Process process)
    {
        List<Process> children = new List<Process>();
        ManagementObjectSearcher mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", process.Id));

        foreach (ManagementObject mo in mos.Get())
        {
            children.Add(Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])));
        }

        return children;
    }
}
