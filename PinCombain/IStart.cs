using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinCombain
{
    interface IStart
    {
        void Start();

        RemoteWebDriver Driver { get; set; }
    }
}
