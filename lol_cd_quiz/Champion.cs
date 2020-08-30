using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using HtmlAgilityPack;

namespace lol_cd_quiz
{
    public class Champion
    {
        public Champion(string url)
        {
            Name = GetChampionNameAndTitle(url)[0];
            Title = GetChampionNameAndTitle(url)[1];
            Abilities = GetAbilityInformation(url);
        }

        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public List<Ability> Abilities { get; set; }

        

        //public static Champion GetInformation(string url)
        //{
        //    Champion champion = new Champion();

        //    var nameAndTitle = GetChampionNameAndTitle(url);
        //    champion.Name = nameAndTitle[0];
        //    champion.Title = nameAndTitle[1];
        //    champion.Abilities = new List<Ability>();
        //    var championAbilityNodes = HtmlScraper.GetHtmlNodes(url, "//a[contains(@class, 'champ-abilities__item ')]");

        //    foreach (HtmlNode node in championAbilityNodes)
        //    {
        //        var ability = new Ability(node);
        //        champion.Abilities.Add(ability);
        //    }

        //    return champion;
        //}

        private static List<Ability> GetAbilityInformation(string url)
        {
            List<Ability> abilities = new List<Ability>();
            var championAbilityNodes = HtmlScraper.GetHtmlNodes(url, "//a[contains(@class, 'champ-abilities__item ')]");

            foreach (HtmlNode node in championAbilityNodes)
            {
                var ability = new Ability(node);
                abilities.Add(ability);
            }

            return abilities;
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
