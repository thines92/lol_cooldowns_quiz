using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
namespace lol_cd_quiz
{
    public class HtmlScraper
    {
        public HtmlScraper()
        {
            
        }

        public static HtmlDocument WebpageHtml = GetHtmlDocumentAsync("https://www.mobafire.com/league-of-legends/champions").Result;

        public static async Task<HtmlDocument> GetHtmlDocumentAsync(string url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            HtmlDocument WebpageHtml = new HtmlDocument();
            WebpageHtml.LoadHtml(html);

            return WebpageHtml;
        }

        public static HtmlNodeCollection GetHtmlNodes(String url, String searchValue)
        {
            WebpageHtml = GetHtmlDocumentAsync(url).Result;


            var result = WebpageHtml
                .DocumentNode
                .SelectNodes(searchValue);

            return result;
        }

        public static HtmlNodeCollection GetHtmlNodes(String searchValue)
        {
            var result = WebpageHtml
                .DocumentNode
                .SelectNodes(searchValue);

            return result;
        }

        public static HtmlNode GetHtmlNode(String url, String searchValue)
        {
            return WebpageHtml
                .DocumentNode
                .SelectSingleNode(searchValue);
        }

        public static HtmlNodeCollection GetChildNodes(string searchValue)
        {
            return WebpageHtml
                .DocumentNode
                .SelectSingleNode(searchValue)
                .ChildNodes;
        }

        public static HtmlNodeCollection SearchNodeForValue(HtmlNode node, Filter filter)
        {
            HtmlNodeCollection result = null;

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes[filter.Attribute] != null && element.Attributes[filter.Attribute].Value == filter.XPath)
                {
                    result = element.ChildNodes;

                }

            }

            return result;
        }
    }
}
