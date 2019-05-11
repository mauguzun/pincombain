using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PinCombain
{
    public class MakeGrabber:IStart
    {
       
        private List<Domain> _domains;

        private string _url = "https://www.pinterest.com/";
        //private string _url = "https://www.pinterest.com/categories/popular/";

        private string _domainFile = "domains.txt";
        private string _resultFile = "result.txt";


      
        public RemoteWebDriver Driver { get ; set ; }

        public MakeGrabber()
        {
            _domains = new List<Domain>();
            _LoadDomain();
        }

        private void _LoadDomain()
        {
            if (System.IO.File.Exists(this._domainFile))
            {
                string[] domains = System.IO.File.ReadAllLines(this._domainFile);
                foreach (string domain in domains)
                {
                    _domains.Add(new Domain(domain));
                }
                Console.WriteLine( $"loaded {_domains.Count} domains" + Environment.NewLine);

            }
        }


        public void Start()
        {

            if (_domains.Count == 0)
            {
                Console.WriteLine("please load domain");
                return;
            }


            while (true)
            {
                DoGrab();
            }



        }

        private void DoGrab()
        {
            List<string> result = new List<string>();
            try
            {


                var nodes = Driver.FindElementsByCssSelector(".pinWrapper img");
                foreach (var node in nodes)
                {
                    string img = node.GetAttribute("src");
                    img = img.Replace("236x", "564x");
                    string text = node.GetAttribute("alt");
                    string request = $"?c={this.Base64Encode(text)}*{this.Base64Encode(img.Replace("https://", ""))}";

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
                File.AppendAllLines(this._resultFile, result);
                Console.WriteLine($"saved {result.Count()}" + Environment.NewLine);
                Driver.Url = this._url;
                PostResult();

            }
        }

        private void PostResult()
        {
            try
            {
                int newP = 0;
                int existP = 0;
                string[] lines = File.ReadAllLines(this._resultFile);
                File.Delete(this._resultFile);
                Random rand = new Random();

                int count = 0;
                foreach (var line in lines)
                {
                    var random = this._domains[rand.Next(0, _domains.Count)];

                    var url = (random.Url.Contains("put/index/")) ? random.Url + line :
                        random.Url + "put/index/" + line;




                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var x = reader.ReadToEnd();



                        if (x.Contains("1"))
                            newP++;
                        else
                            existP++;

                        Thread.Sleep(100);
                        Console.WriteLine($"{lines.Length} / {count} **" + Environment.NewLine);
                        count++;
                    }
                }
                Console.WriteLine($"{existP} exist  , new  ** {newP} **" + Environment.NewLine);
                Thread.Sleep(100);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"doestn`t have any result for post " + ex.Message);
                Thread.Sleep(100);
            }
            finally
            {

            }

        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }


}

