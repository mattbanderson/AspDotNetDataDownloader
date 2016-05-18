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
            var uri = ConfigurationManager.AppSettings["uri"];
            var downloadLinkElementId = ConfigurationManager.AppSettings["downloadLinkElementId"];
            var expectedContentType = ConfigurationManager.AppSettings["expectedContentType"];
            var client = new RestClient(uri);
            client.CookieContainer = new CookieContainer();

            var html = GetHtml(client);
            var viewState = GetElementValueById(html, "__VIEWSTATE");
            var viewStateGenerator = GetElementValueById(html, "__VIEWSTATEGENERATOR");
            var eventValidation = GetElementValueById(html, "__EVENTVALIDATION");
            
            var response = ExecutePost(client, downloadLinkElementId, viewState, viewStateGenerator, eventValidation);
            if (response == null || string.IsNullOrWhiteSpace(response.Content))
            {
                Console.WriteLine("Response was empty or an error occurred.");
            }
            else if (!string.IsNullOrWhiteSpace(expectedContentType) && !response.ContentType.ToLower().Contains(expectedContentType))
            {
                Console.WriteLine(string.Format("Unexpected response. Expected content-type to contain '{0}' but received '{1}'.", expectedContentType, response.ContentType));
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

        private static string GetHtml(IRestClient client)
        {
            var request = new RestRequest(Method.GET);
            Console.WriteLine("Requesting HTML...");
            var response = client.Execute(request);            
            Console.WriteLine("HTML response complete.");
            return response.Content;
        }

        private static string GetElementValueById(string html, string id)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var viewState = doc.GetElementbyId(id);
            if (viewState == null || viewState.Attributes == null)
            {
                Console.WriteLine("ViewState not found.");
            } 
            return viewState.Attributes["value"].Value ?? string.Empty;
        }

        private static IRestResponse ExecutePost(IRestClient client, string downloadLinkElementId, string viewstate, string viewstateGenerator, string eventValidation)
        {
            var request = new RestRequest(Method.POST);
            request.AddParameter("__EVENTTARGET", downloadLinkElementId);
            request.AddParameter("__VIEWSTATE", viewstate);
            request.AddParameter("__VIEWSTATEGENERATOR", viewstateGenerator);
            request.AddParameter("__EVENTVALIDATION", eventValidation);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
           
            Console.WriteLine("Beginning file download request...");
            var response = client.Execute(request);
            Console.WriteLine("Response received.");

            return response;
        }
    }
}
