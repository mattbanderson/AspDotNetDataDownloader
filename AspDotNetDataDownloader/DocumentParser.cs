using HtmlAgilityPack;
using System;

namespace AspDotNetDataDownloader
{
    public class DocumentParser
    {
        public static string GetElementValueById(string html, string id)
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
    }
}
