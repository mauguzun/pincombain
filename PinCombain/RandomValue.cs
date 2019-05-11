using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinCombain
{
     public class RandomValue
    {
        public string GetString(string path)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);

                return lines[new Random().Next(0, lines.Length)];
            }
            catch
            {
                return null;
            }




        }
    }
}
