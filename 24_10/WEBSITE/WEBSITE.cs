using _23_10.Website;
using _24_10.Log;
using _24_10.WEBSITE;
using Newtonsoft.Json.Linq;
using Svg;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _23_10.Website
{
    // get web, chose web, 
    public class WEBSITE
    {
        public ISource source;
        public ISQLQuery sQLQuery;
        public ILogger Logger;
        public FindUrl findUrl;
        public FindImg findImg;
        public WEBSITE(ISource source, ISQLQuery sQLQuery,ILogger Logger, FindUrl findUrl,FindImg findImg)
        {
            this.source = source;
            this.sQLQuery = sQLQuery;
            this.Logger = Logger;
            this.findUrl = findUrl;
            this.findImg = findImg;
        }

        public Task<Data> GetData(string name,string id) {
            Logger.WriteLine($"Get Data ({id}, {name}) ...");
            return source.GetData(name, id);

        }
        public bool CheckData(string Id)
        {
            Logger.WriteLine($"Check Id ({Id}) ...");
            return sQLQuery.findById(Id);
        }
        public void SaveData(string id,string name,string json,string logo)
        {
            Logger.WriteLine($"3) WEB,Save Data ({id},{name} ,\"Json\", {logo}) ...");
            try
            {
                sQLQuery.Insert(new Data
                {
                    Id = id,
                    Name = name,
                    Json = json,
                    Logo = logo

                });
            }
            catch (Exception e) 
            {
                Logger.WriteLine($"Exception in WEBSITE,SaveData {e.Message}");
            }

        }
        public string GetUrl(Data data,string searchString) 
        {
            Logger.WriteLine($"Get Url ({searchString}) ...");
            List<string> Urls = new List<string>();
            JObject json = JObject.Parse(data.Json);
            foreach (var item in json["items"]) 
            {
                string link = item["link"].ToString();
                Logger.WriteLine($"Link: {link}");
                Urls.Add(link);
            }

            return findUrl.ReturnUrl(Urls,searchString);

        }
        public string FindImg(string html,string url)
        {
            Logger.WriteLine($"1) WEB,Find Img:.......{url}");
            return findImg.Find(html,url);
        }
        private void ConvertSvgToJpg(string filePath)
        {
            var svgDocument = SvgDocument.Open(filePath);
            using (var bitmap = svgDocument.Draw())
            {
                bitmap?.Save(filePath.Replace(".svg",".jpg"));
            }
            File.Delete(filePath);
        }
        public bool dowloadImg(string url, string name)
        {

            Logger.WriteLine($"2) WEB,download Img:........{url}");
            try
            {
                string dest = string.Empty; 
                if (url.EndsWith(".svg"))
                {
                    dest = $"D:\\Logo\\{name}.svg";
                }
                else
                {
                    dest = $"D:\\Logo\\{name}.jpg";
                }

                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(url, dest);

                    Logger.WriteLine($"Dowload File: {name} in {url} ,Success");

                    if (url.EndsWith(".svg"))
                    {
                        ConvertSvgToJpg(dest);
                    }

                    return true;
                }




            }
            catch (Exception ex)
            {
                Logger.WriteLine("Error in Dowload :" + ex.Message);
                return false;
            }
        }

    }
}
