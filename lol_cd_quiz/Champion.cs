using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;

namespace lol_cd_quiz
{
    public class Champion
    {
        public Champion()
        {
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Title")]
        public string Title { get; set; }
        [BsonElement("Abilities")]
        public List<Ability> Abilities { get; set; }

        public static List<Ability> GetAbilityInformation(string url)
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
