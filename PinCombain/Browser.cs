using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PinCombain
{
    class Browser
    {
        private ChromeDriver driver { get; set; }

        public Browser(ChromeDriver driver)
        {
            this.driver = driver;
        }

        public bool MakeLogin(string email ,string password)
        {

            try
            {
                

                driver.Url = "https://www.pinterest.com/login/";


              

                driver.FindElementById("password").SendKeys(password);
                driver.FindElementById("email").SendKeys(email);

                driver.FindElementByCssSelector("div[data-test-id='registerFormSubmitButton']").Click();

                Thread.Sleep(new TimeSpan(0, 0, 25));
               
                var xs=   driver.Manage().Cookies.GetCookieNamed("_auth");


                if (xs.Value == "1")
                {
                    var cookies = driver.Manage().Cookies.AllCookies;
                    foreach (Cookie cookie in cookies)
                    {
                        //_auth=1
                        File.AppendAllText("cok.txt", cookie.ToString() + Environment.NewLine);
                        var row = cookie.ToString();
                        var x = 12;
                    }
                }
                else
                    throw new Exception();
             
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }


        }
    }
}
