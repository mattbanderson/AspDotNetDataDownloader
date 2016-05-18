using HtmlAgilityPack;
using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace AspDotNetDataDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            var downloader = new DataDownloader(
                ConfigurationManager.AppSettings["downloadLinkElementId"],
                ConfigurationManager.AppSettings["expectedContentType"],
                new RestClient(ConfigurationManager.AppSettings["uri"])
                );
            
            var html = downloader.ExecuteGet();
            var viewState = DocumentParser.GetElementValueById(html, "__VIEWSTATE");
            var viewStateGenerator = DocumentParser.GetElementValueById(html, "__VIEWSTATEGENERATOR");
            var eventValidation = DocumentParser.GetElementValueById(html, "__EVENTVALIDATION");
            
            var response = downloader.ExecutePost(viewState, viewStateGenerator, eventValidation);
            if (response == null || string.IsNullOrWhiteSpace(response.Content))
            {
                Console.WriteLine("Response was empty or an error occurred.");
            }
            else if (!string.IsNullOrWhiteSpace(downloader.ExpectedContentType) && 
                     !response.ContentType.ToLower().Contains(downloader.ExpectedContentType))
            {
                Console.WriteLine(string.Format("Unexpected response. Expected content-type to contain '{0}' but received '{1}'.", downloader.ExpectedContentType, response.ContentType));
            }
            else
            {
                var outputFile = ConfigurationManager.AppSettings["outputFile"];
                Console.WriteLine(string.Format("Writing output to file {0}...", outputFile));
                File.WriteAllText(outputFile, response.Content);
                Console.WriteLine(string.Format("Output file {0} created.", outputFile));
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        
      
    }
}
