using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using _24_10.Log;
using Newtonsoft.Json.Linq;

namespace _23_10.Website
{
    public class Google : ISource
    {

        private readonly HttpClient client = new HttpClient();
        //private readonly string _APIKEY = "AIzaSyC7rvbu9bxxNT3DGvaEbiDEkboYcgof4xM";
        //private readonly string _CX = "f7305f809d68744c8";
        private readonly string _APIKEY = "AIzaSyC3u3RdC_xzX16Yo8i7rDzoe8ZEI2RZ7Sk";
        private readonly string _CX = "26c249b92cf7e4577";
        private ILogger logger;
        public Google(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<Data> GetData(string name, string id)
        {
            string url = $"https://www.googleapis.com/customsearch/v1?key={_APIKEY}&cx={_CX}&q={name}";
            try
            {
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode) {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseString);
                    return new Data()
                    {
                        Id = id,
                        Name = name,
                        Json = json.ToString(),
                        Logo = "",
                    };
                }
                else
                {
                    var errorResponseString = await response.Content.ReadAsStringAsync();
                    logger.WriteLine($"----Response is not OK: {errorResponseString}");
                    throw new Exception("Not Found");
                }
            }
            catch (Exception ex) { 
                logger.WriteLine($"----Unkown Error in GG Service: {ex.Message}");
                throw new Exception("Not Found");
            }
        }

    }
}
