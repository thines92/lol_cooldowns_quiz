using System;
using System.Collections.Generic;
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
        public List<Ability> Abilities { get; set; }

        public static List<string> GetChampionUrls(String url)
        {
            List<string> championUrls = new List<string>();
            HtmlNodeCollection champNodes = HtmlScraper.GetHtmlNodes(url, "//a[contains(@class, 'champ-list')]");

            foreach (HtmlNode node in champNodes)
            {
                var href = node.GetAttributeValue("href", string.Empty);
                var championPageUrl = "https://www.mobafire.com" + href;
                championUrls.Add(championPageUrl);
            }

            //for (var i = 0; i < 6; i++)
            //{
            //    var href = champNodes[i].GetAttributeValue("href", string.Empty);
            //    var championPageUrl = "https://www.mobafire.com" + href;
            //    championUrls.Add(championPageUrl);
            //}

            return championUrls;
        }

        private static Champion GetChampionInformation(String url)
        {
            Champion champion = new Champion();


            champion.Name = HtmlScraper.GetHtmlNode(url, "//div[contains(@class, 'champ-splash__title')]/h2").InnerText;
            champion.Abilities = new List<Ability>();
            var championAbilityNodes = HtmlScraper.GetHtmlNodes(url, "//a[contains(@class, 'champ-abilities__item ')]");

            foreach (HtmlNode node in championAbilityNodes)
            {
                var ability = new Ability(node);
                champion.Abilities.Add(ability);
            }

            return champion;
        }

        public static List<Champion> GetChampions(List<string> championUrls)
        {
            List<Champion> champions = new List<Champion>();

            foreach (String url in championUrls)
            {
                Champion champion = GetChampionInformation(url);

                champions.Add(champion);
            }

            return champions;
        }
    }
}
