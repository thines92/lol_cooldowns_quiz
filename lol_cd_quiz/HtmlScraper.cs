using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public static HtmlNodeCollection GetNodeByClass(HtmlNode node, string className)
        {
            HtmlNodeCollection result = null;

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == className)
                {
                    result = element.ChildNodes;
                }
            }

            return result;
        }

        public static List<string> GetListFromNode(HtmlNode node, string className)
        {
            List<string> list = GetNodeByClass(node, className)
                .Where(x => x.Name.Contains("text"))
                .FirstOrDefault()
                .InnerText.Replace("\n", "").Replace(" / ", " ")
                .Split(" ")
                .ToList();

            return list;
        }

        public static string GetStringFromNode(HtmlNode node, string className)
        {
            string value = GetNodeByClass(node, className)?
                   .Where(x => x.Name.Contains("text"))
                   .FirstOrDefault()
                   .InnerText.Replace("\n", "");

            return value;
        }

        public static string GetStringArrayFromNode(HtmlNode node, string className)
        {
            string value = HtmlScraper.GetNodeByClass(node, className)?
                .Where(x => x.Name.Contains("text"))
                .ToArray()
                .Aggregate("", (x, y) => x += y.InnerText);

            return value;
        }
    }
}
