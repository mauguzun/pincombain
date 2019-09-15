using MongoDB.Driver;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PinCombain
{
    class Program
    {
        static PinterestMethods pinterestMethods = new PinterestMethods();
        static string DIR = "data";
        static string LOGINED = "for_grab_account_";
        static string currentPath;
        static string currentAccount = null;
        static ChromeDriver driver;


        static void Main(string[] args)
        {



            if (!Directory.Exists(DIR))
            {
                Directory.CreateDirectory(DIR);
            }




            string answer = null;

            if (answer == null)
            {
                Console.WriteLine("r/rename  p/post  b/makeboard  l/showAccountIn   u/user g/grab  c/check  do/doling"   );
                answer = Console.ReadLine();
            }
            if (answer.Contains("|"))
            {
                string[] param = answer.Split('|');
                answer = param[1];
                currentAccount = DIR + "/" + param[0].Trim() + ".xml";
            }
            switch (answer)
            {
                case "r":
                    Rename();
                    break;

                case "c":
                    CheckAccount();
                    break;

                case "l":
                    ShowAccountIn();
                    break;

                case "do":
                     DoLogin();
                    break;



                case "b":
                    MakeBoard();
                    break;

                case "g":
                    GrabOrMakePost();
                    break;

                case "p":
                    MakePost();
                    break;

                case "u":
                    GrabUser();
                    break;

                default:
                    Console.WriteLine("noting");
                    break;
            }

            Console.ReadKey();


        }

        private static void DoLogin()
        {
            string proxy = RandomProxy();
            do
            {
                try
                {
                    proxy = RandomProxy();
                    driver = GetDriver(true);
                 //   driver = GetDriver(true, proxy);
                    driver.Url = "https://www.pinterest.com/login/";
                }
                catch { }
              
     
                Console.WriteLine("ok? y|n");
                
            }
            while (!Console.ReadLine().Contains("y"));

            //save cookie
            var xs = driver.Manage().Cookies.GetCookieNamed("_auth");
            var cookies = driver.Manage().Cookies.AllCookies;

            List<DCookie> listDc = new List<DCookie>();
            foreach (OpenQA.Selenium.Cookie cookie in cookies)
            {
                //_auth=1
                var dCookie = new DCookie();
                dCookie.Domain = cookie.Domain;
                dCookie.Expiry = cookie.Expiry;
                dCookie.Name = cookie.Name;
                dCookie.Path = cookie.Path;
                dCookie.Value = cookie.Value;
                dCookie.Secure = cookie.Secure;

                listDc.Add(dCookie);
            }
            XmlSerializer ser = new XmlSerializer(typeof(List<DCookie>), new XmlRootAttribute("list"));

            using (FileStream fs = new FileStream(DIR + "/"+LOGINED + proxy.Replace(".","_").Replace(":","__") +".xml" , FileMode.Create))
            {
                ser.Serialize(fs, listDc);
            }
            GrabOrMakePost();

        }

        private static void MakePost()
        {
            string proxy = RandomProxy();
            driver = GetDriver(true, proxy);
            var res = MakeLogin("denisuminja.k.rasiv.i.jp.enis@gmail.com", "1637trance");
            if (res)
            {

              

              
                PostPin post = new PostPin();
                post.Driver = driver;
                post.Start();
               

            }

        }
        private static string RandomProxy()
        {
            string currentProxy;
            GetProxy.ProxyReader proxyReader = new GetProxy.ProxyReader();
            var proxyList = proxyReader.GetList();
            currentProxy = proxyList[new Random().Next(0, proxyList.Count())];

            try
            {

                proxyList.Remove(currentProxy);
                File.WriteAllLines(proxyReader.Path, proxyList.ToArray());

            }
            catch { };
            return currentProxy;
        }


        private static void ShowAccountIn()
        {
            //  -a -a
            var accounts = new ReadXmlAccounts().GetXml(DIR);
            foreach (string accountPath in accounts)
            {

                driver = GetDriver(false);
                pinterestMethods.Driver = driver;


                MakeLogin(accountPath);
                if (CheckLogin())
                {
                    Console.WriteLine(Path.GetFileNameWithoutExtension(accountPath));
                }
                else
                {
                    Console.WriteLine("***********" + Path.GetFileNameWithoutExtension(accountPath));
                    File.Delete(accountPath);
                }

                driver.Quit();


            }

        }
        static void GrabUser()
        {
            if (currentAccount == null)
            {
                var accounts = new ReadXmlAccounts().GetXml(DIR);
                currentAccount = accounts[0];
            }
            driver = GetDriver();

            pinterestMethods.Driver = driver;
            pinterestMethods.GrabName();
        }
        static void GrabOrMakePost(bool grabPin = true)
        {
          
             var accounts = new ReadXmlAccounts().GetXml(DIR);
           // string proxy = RandomProxy();


            foreach (var acc in accounts)
            {
                if (acc.Contains(LOGINED))
                {
                    currentAccount = acc;
                   // proxy = acc.Replace(DIR, "").Replace("\\", "").Replace(LOGINED, "").Replace("__", ":").Replace("_", ".").Replace("xml", ".");
                    break;

                }
            }
            driver = GetDriver(false);
           // driver = GetDriver(false, proxy);
            pinterestMethods.Driver = driver;

            MakeLogin(currentAccount);



            if (CheckLogin())
            {
                IStart gr;
                if (grabPin)
                    gr = new MakeGrabber();
                else
                    gr = new PostPin();

                gr.Driver = driver;
                while (true)
                {
                    gr.Start();
                }

            }
        }

        private static void MakeBoard()
        {
            List<string> proxies = new GetProxy.ProxyReader().GetList();
            foreach (string proxy in proxies)
            {
                Console.WriteLine(proxy);
                driver = GetDriver(true, proxy);
                currentAccount = currentAccount.Replace("data/", "");
                currentAccount = currentAccount.Replace(".xml", "");
                string[] emailPass = currentAccount.Replace("/data", "").Split(':');
                if (MakeLogin(emailPass[0], emailPass[1]) == true)
                {

                    Console.WriteLine("logined");

                    pinterestMethods.Driver = driver;
                    pinterestMethods.FillCard();
                    pinterestMethods.FillName();

                    if (pinterestMethods.CreateBoard())
                    {

                        File.AppendAllText(DIR + "/" + "supreproxy.txt", proxy + Environment.NewLine);
                        Console.WriteLine("done");
                        break;
                    }
                }
            }

        }

        private static bool MakeLogin(string email, string pass)
        {

            try
            {
                
                driver.FindElementById("password").SendKeys(pass);
                driver.FindElementById("email").SendKeys(email);


                driver.FindElementByCssSelector("div[data-test-id='registerFormSubmitButton']").Click();

                Thread.Sleep(new TimeSpan(0, 0, 25));

                var labels = driver.FindElementsByCssSelector("label[for='password']");
                // if 

                return CheckLogin();
            }
            catch
            {
                return false;
            }




        }

        private static void MakeLogin(string accountPath)
        {

         
            driver.Url = "http://pinterest.com";
            List<DCookie> dCookie;

           
            using (var reader = new StreamReader(accountPath))
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(List<DCookie>),
                    new XmlRootAttribute("list"));
                dCookie = (List<DCookie>)deserializer.Deserialize(reader);
            }

            foreach (var cookie in dCookie)
            {
                driver.Manage().Cookies.AddCookie(cookie.GetCookie());
            }

            driver.Url = "http://pinterest.com";
            Console.WriteLine("logined" + CheckLogin());
            Thread.Sleep(new TimeSpan(0, 0, 5));
        }

        static bool CheckLogin()
        {
            if (driver.Manage().Cookies.GetCookieNamed("_auth").Value.ToString() == "1")
            {
                return true;
            }
            else
            {

                return false;
            }
        }

        static public void CheckAccount()
        {


            var accounts = File.ReadAllLines(@"C:\my_work_files\pinterest\source_all_account_for_blaster.txt");

            var dr = GetDriver(false);


            foreach (string file in new string[] { "/__good_acc.txt", "/titles.txt" })
            {
                if (File.Exists(DIR + file))
                    File.Delete(DIR + file);
            }



            foreach (string line in accounts)
            {
                string[] values = line.Split(':');
                dr.Url = "https://pinterest.com/" + values[2];



                if (dr.Title.Trim() == "Pinterest")
                {

                    File.AppendAllText(DIR + "/__bad_url.txt", "https://pinterest.com/" + values[2] + Environment.NewLine);
                    File.AppendAllText(DIR + "/__bad_acc.txt", line + Environment.NewLine);
                    Console.WriteLine(line + " emmm... ");
                }
                else
                {
                    File.AppendAllText(DIR + "/titles.txt", dr.Title.Trim() + ":" + line + Environment.NewLine);

                    File.AppendAllText(DIR + "/__good_acc.txt", line + Environment.NewLine);
                    Console.WriteLine(line + " --- ");
                }
            }
            dr.Close();
        }




        static void Rename()
        {

            var accounts = new ReadXmlAccounts().GetXml(@"C:\StartNewMakeAccount\StartNewMakeAccount\bin\Debug\data\");


            foreach (string accountPath in accounts)
            {



                driver = GetDriver(true);
                pinterestMethods.Driver = driver;

                driver.Url = "http://pinterest.com";
                List<DCookie> dCookie;
                using (var reader = new StreamReader(accountPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(List<DCookie>),
                        new XmlRootAttribute("list"));
                    dCookie = (List<DCookie>)deserializer.Deserialize(reader);
                }

                foreach (var cookie in dCookie)
                {
                    driver.Manage().Cookies.AddCookie(cookie.GetCookie());
                }

                driver.Url = "http://pinterest.com";
                Console.WriteLine("logined");
                Thread.Sleep(new TimeSpan(0, 0, 5));
                if (CheckLogin())
                {
                    pinterestMethods.FillCard();
                    pinterestMethods.FillName();
                    pinterestMethods.CreateBoard();

                    File.Move(accountPath, DIR + @"\" + Path.GetFileName(accountPath));

                    File.AppendAllText(DIR + @"\" + "done.txt", Path.GetFileNameWithoutExtension(accountPath) + ":trance_333" + Environment.NewLine);

                }
                else
                {
                    File.Delete(accountPath);
                }
                driver.Quit();


            }



        }



        private static ChromeDriver GetDriver(bool visible = false, string proxy = null)
        {

            
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;


            ChromeOptions options = new ChromeOptions();

            if (!visible)
                options.AddArguments("headless");
           if (proxy != null)
             options.AddArgument($"--proxy-server={proxy}");

            options.AddArgument("--window-size=1920,4080");
            ChromeDriver driver = new ChromeDriver(driverService, options);
            return driver;
        }
    }
}
