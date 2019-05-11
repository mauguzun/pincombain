using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinCombain
{

    public class Domain
    {
        public string Url { get; set; }

        public Domain(string domain)
        {
            this.Url = domain;
            if (!domain.Contains("http://"))
                Url = $"http://{domain}";
            if (!domain.Contains("/put/index/"))
                Url = $"{Url}/put/index/";


        }
    }

}
