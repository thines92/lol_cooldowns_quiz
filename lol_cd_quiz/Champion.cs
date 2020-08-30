using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using HtmlAgilityPack;

namespace lol_cd_quiz
{
    public class Champion
    {
        public Champion()
        {
        }

        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string title { get; set; }
        public List<Ability> Abilities { get; set; }

        public static List<string> GetChampionUrls(String url)
        {
            List<string> championUrls = new List<string>();
            HtmlNodeCollection champNodes = HtmlScraper.GetHtmlNodes(url, "//a[contains(@class, 'champ-list')]");

            //foreach (HtmlNode node in champNodes)
            //{
            //    var href = node.GetAttributeValue("href", string.Empty);
            //    var championPageUrl = "https://www.mobafire.com" + href;
            //    championUrls.Add(championPageUrl);
            //}

            for (var i = 0; i < 6; i++)
            {
                var href = champNodes[i].GetAttributeValue("href", string.Empty);
                var championPageUrl = "https://www.mobafire.com" + href;
                championUrls.Add(championPageUrl);
            }

            return championUrls;
        }

        public static List<Champion> GetChampions()
        {
            List<Champion> champions = new List<Champion>();
            List<String> championUrls = GetChampionUrls("https://www.mobafire.com/league-of-legends/champions");

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

            var nameAndTitle = GetChampionNameAndTitle(url);
            champion.Name = nameAndTitle[0];
            champion.title = nameAndTitle[1];
            champion.Abilities = new List<Ability>();
            var championAbilityNodes = HtmlScraper.GetHtmlNodes(url, "//a[contains(@class, 'champ-abilities__item ')]");

            foreach (HtmlNode node in championAbilityNodes)
            {
                var ability = new Ability(node);
                champion.Abilities.Add(ability);
            }

            return champion;
        }

        public static List<string> GetChampionNameAndTitle(String url)
        {
            var nodes = HtmlScraper.GetHtmlNodes(url, "//div[contains(@class, 'champ-splash__title')]/h2");
            List<string> nameArray = new List<string>();

            foreach (var node in nodes)
            {
                var nameList = node.ChildNodes.ToList();

                nameArray.Add(nameList[0].InnerText.Trim());
                nameArray.Add(nameList[1].InnerText.Trim());
            }

            return nameArray;
        }
    }
}
