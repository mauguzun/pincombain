using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PinCombain
{
    public class PinterestMethods
    {
        public ChromeDriver Driver { get; set; }
        private string accountName = null;
        private Model model;
        public void SetAccountName(string accName)
        {
            this.accountName = accName;
        }
        public void FillName()
        {
            try
            {
                Driver.Url = "https://www.pinterest.com/settings#profile";

                //  Driver.FindElementById("location").SendKeys(new RandomValue().GetString("city_names.txt"));


                for (int i = 0; i < 50; i++)
                {
                    Driver.FindElementById("first_name").SendKeys(Keys.Backspace);
                    Driver.FindElementById("last_name").SendKeys(Keys.Backspace);
                }




                Driver.FindElementById("first_name").SendKeys(new RandomValue().GetString("names.txt"));
                Driver.FindElementById("last_name").SendKeys(new RandomValue().GetString("names.txt"));
                Driver.FindElementById("first_name").SendKeys(Keys.Space);

                this.accountName = Driver.FindElementById("username").GetAttribute("value");

                var buttons = Driver.FindElementsByTagName("button");
                foreach (var button in buttons)
                {
                    if (button.Text.Trim().ToUpper() == "DONE")
                    {
                        Actions actions = new Actions(Driver);
                        actions.MoveToElement(Driver.FindElementByCssSelector("div[data-test-id='settings-header']"));
                        actions.Perform();
                        button.Click(); Thread.Sleep(new TimeSpan(0, 0, 5)); return;
                    }
                }
            }

            catch (Exception ex)
            {
                var sdfs = ex.Message;
                return;
            }
        }

        public bool CreateBoard()
        {
            try
            {

                Driver.Url = $"https://www.pinterest.com/{this.accountName}/boards/";
                var buttons = Driver.FindElementsByCssSelector(".fixedHeader button");
                buttons[0].Click(); Thread.Sleep(new TimeSpan(0, 0, 5));

                Driver.FindElementByCssSelector("div[title='Create board']").Click(); Thread.Sleep(new TimeSpan(0, 0, 5));
                Driver.FindElementById("boardEditName").SendKeys(new RandomValue().GetString("names.txt"));
                buttons = Driver.FindElementsByTagName("button");
                foreach (var button in buttons)
                {
                    if (button.Text.Trim().ToUpper() == "CREATE")
                    {

                        button.Click();
                        Thread.Sleep(new TimeSpan(0, 0, 5));
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }

        }

        public void FillCard()
        {
            int result = 0;
            try
            {

                var cards = Driver.FindElementsByCssSelector(".NuxInterest");
                for (int i = 0; i < 100; i++)
                {

                    if (result > 5)
                    {
                        var button = Driver.FindElementsByCssSelector(".NuxPickerFooter button");
                        if (button.Count > 0)
                        {


                            button[0].Click();
                            Thread.Sleep(new TimeSpan(0, 0, 20));
                            return;

                        }

                    }

                    try
                    {
                        if (cards[i].Enabled && cards[i].Displayed)
                        {
                            //  OpenQA.Selenium.Interactions.Actions action = new OpenQA.Selenium.Interactions.Actions(Driver);
                            //  action.MoveToElement(cards[i]).DoubleClick().Build().Perform();
                            cards[i].Click();
                            result++;
                        }
                        else
                        {
                            Console.WriteLine("not possible");
                        }
                        //return;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        cards = Driver.FindElementsByCssSelector(".NuxInterest");
                        //   return;
                    }

                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        
        public void GrabName()
        {
            model = Model.GetInstance();
            try
            {
               
                while (true)
                {
                    Driver.Url = $"https://www.pinterest.com/search/users/?q={new RandomValue().GetString("city_names.txt")}&rs=filter";
                    var users = Driver.FindElementsByCssSelector("div[data-test-id='user-rep'] a");
                    foreach (var user in users)
                    {
                        var fullUrl = user.GetAttribute("href");

                        string[] paths = fullUrl.Split('/');
                        string name = paths[3];

                        if (model.FindUser(name) == null)
                        {
                            model.Add(new User() { Name = name });

                        }
                    }
                }
                    
                
  
            }
            catch
            {
               
            }

        }
    }
}
