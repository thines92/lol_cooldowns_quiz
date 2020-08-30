using System;
using System.Collections.Generic;
using HtmlAgilityPack;
namespace lol_cd_quiz
{
    public class ChampionList
    {
        public ChampionList()
        {
            Champions = GetChampions();
        }

        public List<Champion> Champions;

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

        public static List<Champion> GetChampions()
        {
            List<Champion> champions = new List<Champion>();
            List<string> championUrls = GetChampionUrls("https://www.mobafire.com/league-of-legends/champions");

            foreach (String url in championUrls)
            {
                Champion champion = new Champion();

                champion.Name = Champion.GetChampionNameAndTitle(url)[0];
                champion.Title = Champion.GetChampionNameAndTitle(url)[1];
                champion.Abilities = Champion.GetAbilityInformation(url);

                champions.Add(champion);
            }

            return champions;
        }
    }
}
