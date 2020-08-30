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

        public static async Task<string> GetHtmlAsync(String url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            return html;
        }

        public static HtmlNodeCollection GetHtmlNodes(String url, String searchValue)
        {
            var html = GetHtmlAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            return htmlDocument
                .DocumentNode
                .SelectNodes(searchValue);
        }

        public static HtmlNode GetHtmlNode(String url, String searchValue)
        {
            var html = GetHtmlAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var htmlNode = htmlDocument
                .DocumentNode
                .SelectSingleNode(searchValue);

            return htmlNode;
        }
    }
}
