using RestSharp;
using System;
using System.Configuration;
using System.Net;

namespace AspDotNetDataDownloader
{
    public class DataDownloader
    {
        public string DownloadLinkElementId { get; set; }
        public string ExpectedContentType { get; set; }
        public IRestClient Client { get; set; }
        
        public DataDownloader(string downloadLinkElementId, string expectedContentType, IRestClient client)
        {
            this.DownloadLinkElementId = downloadLinkElementId;
            this.ExpectedContentType = expectedContentType;
            this.Client = client;
            this.Client.CookieContainer = new CookieContainer();
        }

        public string ExecuteGet()
        {
            var request = new RestRequest(Method.GET);
            Console.WriteLine("Requesting HTML...");
            var response = this.Client.Execute(request);
            Console.WriteLine("HTML response complete.");
            return response.Content;
        }

        public IRestResponse ExecutePost(string viewstate, string viewstateGenerator, string eventValidation)
        {
            var request = new RestRequest(Method.POST);

            AddParams(this.DownloadLinkElementId, viewstate, viewstateGenerator, eventValidation, request);
            AddHeaders(request);

            Console.WriteLine("Beginning file download request...");
            var response = this.Client.Execute(request);
            Console.WriteLine("Response received.");

            return response;
        }

        private static void AddHeaders(RestRequest request)
        {
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        }

        private static void AddParams(string downloadLinkElementId, string viewstate, string viewstateGenerator, string eventValidation, RestRequest request)
        {
            if (!string.IsNullOrWhiteSpace(downloadLinkElementId)) request.AddParameter("__EVENTTARGET", downloadLinkElementId);
            if (!string.IsNullOrWhiteSpace(viewstate)) request.AddParameter("__VIEWSTATE", viewstate);
            if (!string.IsNullOrWhiteSpace(viewstateGenerator)) request.AddParameter("__VIEWSTATEGENERATOR", viewstateGenerator);
            if (!string.IsNullOrWhiteSpace(eventValidation)) request.AddParameter("__EVENTVALIDATION", eventValidation);
        }
    }
}
