using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24_10.Log
{
    public class TxtLog : ILogger
    {
        private static readonly string dicPath = $"D:\\Logo\\Log\\{DateTime.Now.ToString("MM-dd HH-mm-ss")}_log.txt";
        public void WriteLine(string message)
        {
            using(
                StreamWriter sw  = new StreamWriter(dicPath,true))
            {
                sw.WriteLine(message);
            }
        }
    }
}
