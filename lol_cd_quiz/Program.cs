using System;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;

namespace lol_cd_quiz
{
    class Program
    {
        static void Main(string[] args)
        {
            var championUrls = GetChampionUrls("https://www.mobafire.com/league-of-legends/champions");

            var champions = GetChampions(championUrls);

            Console.ReadLine();
        }

        private static async Task<string> GetHtmlAsync(String url)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            return html;
        }

        private static HtmlNodeCollection GetHtmlNodes(String url, String searchValue)
        {
            var html = GetHtmlAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            return htmlDocument
                .DocumentNode
                .SelectNodes(searchValue);
        }

        private static HtmlNode GetHtmlNode(String url, String searchValue)
        {
            var html = GetHtmlAsync(url).Result;
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var htmlNode = htmlDocument
                .DocumentNode
                .SelectSingleNode(searchValue);

            return htmlNode;
        }

        private static List<string> GetChampionUrls(String url)
        {
            List<string> championUrls = new List<string>();
            HtmlNodeCollection champNodes = GetHtmlNodes(url, "//a[contains(@class, 'champ-list')]");

            foreach (HtmlNode node in champNodes)
            {
                var href = node.GetAttributeValue("href", string.Empty);
                var championPageUrl = "https://www.mobafire.com" + href;
                championUrls.Add(championPageUrl);
            }

            return championUrls;
        }

        private static List<Champion> GetChampions(List<string> championUrls)
        {
            List<Champion> champions = new List<Champion>();

            foreach (String url in championUrls)
            {
                Champion champion = GetChampionInformation(url);

                champions.Add(champion);
            }

            return champions;
        }

        private static Champion GetChampionInformation(String url)
        {
            Champion champion = new Champion();

            champion.Name = GetHtmlNode(url, "//div[contains(@class, 'champ-splash__title')]/h2").InnerText;
            champion.Cooldowns = new List<string>();
            var championCooldownNodes = GetHtmlNodes(url, "//div[contains(@class, 'champ-abilities__item__cooldown')]");

            foreach (HtmlNode node in championCooldownNodes)
            {
                var championCooldown = GetChampionCooldown(node);
                champion.Cooldowns.Add(championCooldown);
            }

            return champion;
        }

        private static String GetChampionCooldown(HtmlNode node)
        {
            var nodeText = node.InnerText;
            var cooldowns = new string((from i in nodeText
                                       where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                       select i
                                       ).ToArray()).Replace(Environment.NewLine, "");
            var cooldownArray = cooldowns.Split(" ");
            //var cooldown = nodeText.FirstOrDefault(c => char.IsDigit(c)).ToString();
            var cooldown = cooldownArray[0];

            return cooldown;
        }

        public class Champion
        {
            public string Name { get; set; }
            public List<String> Cooldowns { get; set; }
        }
    }
}
