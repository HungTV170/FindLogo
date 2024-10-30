using _23_10.Website;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Xml.Linq;
using _24_10.Log;
using Autofac;
using CsvHelper.Configuration.Attributes;
using _24_10.WEBSITE;
using System.Net.Http;
using System.Runtime.Serialization.Formatters;
using Autofac.Features.ResolveAnything;
namespace _23_10
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false });
        private static IContainer _container;
        private static string csvPath = "D:\\companyWebsiteOutput.csv";
        static void Main(string[] args)
        {

            var builder = new ContainerBuilder();
            builder.RegisterType<TxtLog>().As<ILogger>();
            builder.RegisterType<DetailLogger>();
            //builder.Register<Func<string,ILogger,ISource>>(
            //    c =>
            //    {
            //        return (type, logger) =>
            //        {
            //            switch (type)
            //            {
            //                case "google":return new Google(logger);
            //                case "sql": return new SQL(logger);
            //                default: throw new NotImplementedException();
            //            }
            //        };
            //    }
            //);

            builder.RegisterType<SQL>().As<ISource>();
            builder.RegisterType<SQLQuery>().As<ISQLQuery>();
            builder.RegisterType<FindImg>();
            builder.RegisterType<FindUrl>();
            builder.RegisterType<WEBSITE>();

            _container = builder.Build();

            var companies = ReadCsvFile(csvPath);

            int count = 0;
            foreach (var company in companies) 
            { 
                
                if(count < 1000000)
                {
                    if(company.Logo == "")
                    {
                        //if (CheckId(company.Id))
                        //{
                        //    Data data = GetData(company.Name, company.Id);
                        //    if (data == null)
                        //    {
                        //        continue;
                        //    }
                        //    var url = GetUrl(data, company.Name);

                        //    string logoDownload = String.Empty;
                        //    if (url != "") 
                        //    {
                        //        logoDownload = FindImg(url);
                        //    }
                        //    if(logoDownload != "")
                        //    {
                        //        DownLoad(logoDownload, company.Name);
                        //    }

                        //    using (var scope = _container.BeginLifetimeScope())
                        //    {
                        //        var logger = scope.Resolve<ILogger>();
                        //        logger.WriteLine($"=========================================={count}");
                        //    }

                        //}
                        //count++;

                        //if (!CheckId(company.Id))
                        //{
                        //    //Data data = GetData(company.Name, company.Id);
                        //    //GetUrl(data, company.Name);
                        //    //SaveData(data);
                        //    //using (var scope = _container.BeginLifetimeScope())
                        //    //{
                        //    //    var logger = scope.Resolve<ILogger>();
                        //    //    logger.WriteLine($"=========================================={count}");
                        //    //}
                        //    //count++;
                        //}

                        if (company.Website != "")
                        {
                            string logoDownload = String.Empty;
                            logoDownload = FindImg(company.Website);

                            if (logoDownload != "")
                            {
                                if (DownLoad(logoDownload, company.Name))
                                {
                                    SaveData(new Data()
                                    {
                                        Id = "t2" + company.Id,
                                        Logo = logoDownload,
                                        Name = company.Name,
                                        Json = ""
                                    });
                                };
                            }

                            using (var scope = _container.BeginLifetimeScope())
                            {
                                var logger = scope.Resolve<ILogger>();
                                logger.WriteLine($"=========================================={count}");
                            }
                        }
                        count++;

                    }
                }
            }
            Console.WriteLine("Done!!");
            Console.ReadKey();
        }
        public static bool DownLoad(string url,string name)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var web = scope.Resolve<WEBSITE>();
                return web.dowloadImg(url, name+"Logo");
            }
        }
        public static string FindImg(string url)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                //var sourceFactory = scope.Resolve<Func<string, ILogger, ISource>>();
                var logger = scope.Resolve<ILogger>();
                //var source = sourceFactory("google", logger);
                //var web = scope.Resolve<WEBSITE>(new NamedParameter("source", source));
                var web = scope.Resolve<WEBSITE>();
                try
                {
                    HttpResponseMessage response = null;

                    response = client.GetAsync(url).Result;
                    while ((int)response.StatusCode >= 300 && (int)response.StatusCode < 400)
                    {
                        url = response.Headers.Location.ToString();
                        response = client.GetAsync(url).Result;

                    }

                    string html = string.Empty;
                    html = client.GetStringAsync(url).Result;



                    return web.FindImg(html, url);
                }
                catch (Exception ex) 
                {
                    logger.WriteLine($"Something went wrong :{url}");
                    logger.WriteLine($"Exception in Program,FindImg {ex.InnerException?.Message}");
                    return "";
                }


            }
        }
        public static void SaveData(Data data)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var web = scope.Resolve<WEBSITE>();
                web.SaveData(data.Id,data.Name,data.Json,data.Logo);
            }
        }
        public static string GetUrl(Data data, string searchString)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var web = scope.Resolve<WEBSITE>();
                return web.GetUrl(data, searchString);
            }
        }
        public static Data GetData(string name,string id)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var web = scope.Resolve<WEBSITE>();
                try
                {
                    return web.GetData(name, id).Result;
                }
                catch (Exception ex)
                {
                    return null;
                }

            }
        }
        public static bool CheckId(string id) 
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var web = scope.Resolve<WEBSITE>();
                return web.CheckData(id);
            }

        }
        public static IList<Company> ReadCsvFile(string csvPath)
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var logger = scope.Resolve<ILogger>();
                logger.WriteLine("Read File");
                using(var reader = new StreamReader(csvPath))
                {
                    using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<Company>();
    
                        return records.ToList();
                    }
                }

            }
        }



    }
}
