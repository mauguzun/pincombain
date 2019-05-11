using GetProxy;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using StartNewMakeAccount.Models.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StartNewMakeAccount
{
    class Program
    {
        static bool show = false;

        static string currentProxy;
        

 
        static void Main(string[] args)
        {


            //var randome = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(@"C:\test\my.json"));
            //File.WriteAllLines("gmailAcc.txt", randome);
            
            RandomProxy();


            //  WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(70));
            int i = 0;
            Console.WriteLine("Show ?");
            if (Console.ReadLine() != "y")
                show = true;

            while (true)
            {





                if (i > 5)
                {
                    RandomProxy();
                    i = 0;
                }

                ChromeOptions option = new ChromeOptions();
                option.AddArgument($"--proxy-server={currentProxy}");  //
                option.AddArgument("no-sandbox");

                Console.Title = currentProxy;

                if (show)
                    option.AddArgument("--window-position=-32000,-32000");

                ChromeDriver driver = new ChromeDriver(option);

                // driver.Manage().Timeouts().PageLoad = new TimeSpan(0, 0, 0);
                driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 30);

                var emailProvider = new GenerateEmail();
                Steps ac = new Steps(driver, emailProvider);

                int actions = 0;
                if (ac.MakeLogin())
                {
                    while (ac.Settings() != true && actions < 7)
                    {
                        try
                        {
                            ac.CheckPage();
                        }
                        catch { }
                        finally
                        {
                            actions++;

                        }
                        Console.WriteLine(actions + "action number ");

                    }
                    driver.Quit();
                    i++;
                }
                else
                {

                    RandomProxy();
                    driver.Quit();
                }
            }







        }

        private static void RandomProxy()
        {
            GetProxy.ProxyReader proxyReader = new GetProxy.ProxyReader();
            var   proxyList = proxyReader.GetList();
            currentProxy = proxyList[new Random().Next(0, proxyList.Count())];

            try
            {
               
                proxyList.Remove(currentProxy);
                File.WriteAllLines(proxyReader.Path, proxyList.ToArray());
              
            }
            catch { }
        }

    }
}
