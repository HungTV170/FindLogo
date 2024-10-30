using _24_10.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _23_10.Website
{
    public class FindUrl 
    {
        public ILogger Logger; //defaul : console.writeline

        //public FindUrl()
        //{
        //    Logger = new ConsoleLog();
        //}
        public FindUrl(ILogger logger) 
        { 
            this.Logger = logger;
        }

        public string ReturnUrl(List<string> Urls, string searchString)
        {
            string firstChar = searchString.Split(' ')[0];
            Logger.WriteLine("First c:" + firstChar);
            foreach (var Url in Urls)
            {
                Uri uri = new Uri(Url);
                string domain = uri.Host;
                if (domain.ToLower().Contains(searchString.Trim().ToLower()))
                {
                    Logger.WriteLine($"Find url with {searchString}: {Url}");
                    return Url;
                }

                if (domain.ToLower().Contains(searchString.TrimStart().TrimEnd().Replace(" ", "-").ToLower()))
                {
                    Logger.WriteLine($"Find url with {searchString}: {Url}");
                    return Url;
                }

                if (domain.ToLower().Contains(firstChar.ToLower()))
                {
                    Logger.WriteLine($"Find url with {searchString}: {Url}");
                    return Url;
                }
            }
            Logger.WriteLine($"Search String is Not Match");
            return "";
        }
    }
}
