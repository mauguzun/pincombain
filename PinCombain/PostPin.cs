using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PinCombain
{
    public class PostPin : IStart
    {

        public string Url { get; set; } = "http://fairfield.nl.eu.org/get";
        public RemoteWebDriver Driver { get; set; }

        public PostPin()
        {

        }
        public void Start()
        {
            while(true)
            MakePost();

        }

        private void MakePost()
        {
            Driver.Url = Url;
            //
            Thread.Sleep(new TimeSpan(0, 0, 7));
            var boards = Driver.FindElementsByCssSelector("div[data-test-id='boardWithoutSection']");
            if (boards.Count == 0)
            {
                Console.WriteLine("not boards");
                return;
            }
            foreach (var board in boards)
            {
                try
                {
                    board.Click();
                    Thread.Sleep(new TimeSpan(0, 0, 7));
                    Console.WriteLine("pinned" + DateTime.Now.ToShortTimeString());
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }
        }
    }
}
