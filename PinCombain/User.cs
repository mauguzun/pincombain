using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinCombain
{
    public  class User
    {

        public string Name { get; set; }
        public bool Used { get; set; } = false;

        public DateTime ? LastUsed { get; set; }
    }

}
