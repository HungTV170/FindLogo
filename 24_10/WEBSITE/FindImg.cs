using _24_10.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _24_10.WEBSITE
{
    public class FindImg
    {
        public ILogger Logger; //defaul : console.writeline

        public DetailLogger DLogger; 

        //public FindUrl()
        //{
        //    Logger = new ConsoleLog();
        //}
        public FindImg(ILogger logger, DetailLogger detail)
        {
            this.Logger = logger;
            this.DLogger = detail;
        }

    private string formatUrl(string img,string web)
        {
            if (!img.StartsWith("http"))
            {
                Uri uri = new Uri(web);
                string domain = uri.Host;
                //img = "http://" + domain + "/" + img;
                if (img.StartsWith("/"))
                {
                    img = "https://" + domain + img;
                }
                else
                {
                    img = "https://" + domain + "/" + img;
                }

            }
            return img;
        }

    public string Find(string html, string url)
        {
            var imgPattern = @"<img[^>]+src=""([^""]+)""";
            Console.WriteLine(html);

            var imgMatches = Regex.Matches(html, imgPattern, RegexOptions.IgnoreCase);

            foreach (Match match in imgMatches)
            {

                bool containsLogo = match.Groups[1].Value.IndexOf("logo", StringComparison.OrdinalIgnoreCase) >= 0;
                //MyLog(match.Groups[1].Value);
                if (containsLogo)
                {
                    Logger.WriteLine("FindImg rule1 'logo': " + match.Groups[1].Value);
                    return formatUrl(match.Groups[1].Value,url);
                }
            }

            imgPattern = $@"<a[^>]+href=""(\/|\/index|\/de\/|\/en\/|{url})""[^>]*?>.*?<img[^>]+src=""([^""]+)""";


            imgMatches = Regex.Matches(html, imgPattern, RegexOptions.IgnoreCase);
            Logger.WriteLine("FindImg rule2 <a>");
            foreach (Match match in imgMatches)
            {
                Console.WriteLine(match.Value);
            }
            foreach (Match match in imgMatches)
            {

                Logger.WriteLine("result: " + match.Groups[2].Value);
                return formatUrl(match.Groups[2].Value,url);
            }
            Logger.WriteLine("No Img Result:");
            DLogger.WriteLine(url);
            DLogger.WriteLine(html);

            return "";

        }
    }
}
