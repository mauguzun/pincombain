using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace PinCombain
{
   public class ExtraGrab
    {

        private string _dbName = "my.sqlite";

        private int catNumber = 0;
        private List<string> categories;

        private SQLiteConnection m_dbConn;
        private SQLiteCommand m_sqlCmd;

        public ExtraGrab()
        {
            this.categories = new List<string>();

            categories.Add("http://repinned.net/pins/food-drink/");
            categories.Add("http://repinned.net/pins/");
            categories.Add("http://repinned.net/pins/health-fitness/");
            Connection();

        }

        private void Connection()
        {
            if (!File.Exists(this._dbName))
            {
                SQLiteConnection.CreateFile(this._dbName);
            }
            try
            { 
                m_dbConn = new SQLiteConnection("Data Source=" + this._dbName + ";Version=3;");
                m_dbConn.Open();
                m_sqlCmd = new SQLiteCommand();

                var x = m_dbConn.State;
                m_sqlCmd.Connection = m_dbConn;

                m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS content (id INTEGER PRIMARY KEY AUTOINCREMENT,title TEXT, link TEXT, text TEXT,done intenger default 0 )";
                m_sqlCmd.ExecuteNonQuery();

                Console.WriteLine("Connected");
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public RemoteWebDriver Driver { get; set; }

        public void Start()
        {
            for (int i = 0; i < 2; i++)
            {
                this.catNumber = i;
                this.Job();

            }

        }
        private void Insert(string link,string title , string text)
        {
            try
            {

                SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO content (link, title, text ) VALUES (@link,@title,@text)", m_dbConn);
                insertSQL.Parameters.Add("@link", System.Data.DbType.String).Value = link;
                insertSQL.Parameters.Add("@title", System.Data.DbType.String).Value = title;
                insertSQL.Parameters.Add("@text", System.Data.DbType.String).Value = text;


                insertSQL.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine ("Error: " + ex.Message);
            }
        }
        private void Job()
        {
            MakeGrabber mk = new MakeGrabber();


            for (int i = 1; i < 2000; i++)
            {


                Driver.Url = this.categories[this.catNumber] + i;
                List<string> result = new List<string>();
                try
                {

                    Console.WriteLine(this.categories[this.catNumber] + ", " + i.ToString());


                    var nodes = Driver.FindElementsByCssSelector(".image img:not(.mirror) ");
                    foreach (var node in nodes)
                    {
                        string img = node.GetAttribute("src");
                        img = img.Replace("236x", "564x");
                        string text = node.GetAttribute("alt");
                        string title = node.GetAttribute("title");

                        this.Insert(img,title, text);

                        string request = $"?c={mk.Base64Encode(text)}*{mk.Base64Encode(img.Replace("https://", ""))}";

                        if (!result.Contains(request))
                            result.Add(request);
                        else
                            Console.WriteLine($" node  already exist" + Environment.NewLine);



                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"added {ex.Message}" + Environment.NewLine);
                }
                finally
                {
                    File.AppendAllLines(mk.resultFile, result);
                    Console.WriteLine($"saved {result.Count()}" + Environment.NewLine);

                    mk.PostResult();
                    i++;
                }
            }
        }
    }
}
