using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinCombain
{
    public class ReadXmlAccounts
    {
        public List<string> GetXml(string path)
        {
            List<string> result = new List<string>();
            string[] accountPaths = Directory.GetFiles(path);
            foreach (string accountPath in accountPaths)
            {


                if (Path.GetExtension(accountPath) == ".xml")
                {
                    result.Add(accountPath);
                }


            }
            return result;
        }

    }
}
