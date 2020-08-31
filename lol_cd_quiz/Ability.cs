using System;
using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace lol_cd_quiz
{
    public class Ability
    {
        public Ability(HtmlNode node)
        {
            IconSrc = GetIconSrc(node);
            Name = GetName(node);
            Key = GetKey(node);
            Cooldowns = GetCooldowns(node);
            Description = GetDescription(node);
            Cost = GetCost(node);
            Range = GetRange(node);
        }

        public string IconSrc { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public List<string> Cooldowns { get; set; }
        public string Description { get; set; }
        public List<string> Cost { get; set; }
        public List<string> Range { get; set; }

        public string GetIconSrc(HtmlNode node)
        {
            String baseUrl = "https://www.mobafire.com";
            Filter filter = new Filter("class", "champ-abilities__item__pic");

            var result = HtmlScraper.SearchNodeForValue(node, filter)
                .Where(x => x.Attributes["data-cfsrc"] != null)
                .SingleOrDefault()
                .Attributes["data-cfsrc"].Value;

            return baseUrl += result;
        }


        public List<string> GetCooldowns(HtmlNode node)
        {
            List<string> cooldownArray = new List<string>();
            Filter filter = new Filter("class", "champ-abilities__item__cooldown");

            List<string> cooldowns = HtmlScraper.SearchNodeForValue(node, filter)
                .Where(x => x.Name.Contains("text"))
                .FirstOrDefault()
                .InnerText.Replace("\n", "").Replace(" / ", " ")
                .Split(" ")
                .ToList();

            return cooldowns;
        }

        public string GetName(HtmlNode node)
        {
            Filter filter = new Filter("class", "champ-abilities__item__name");

            var name = HtmlScraper.SearchNodeForValue(node, filter)
                .Where(x => x.Name.Contains("text"))
                .FirstOrDefault()
                .InnerText.Replace("\n", "");

            return name;
        }

        public string GetKey(HtmlNode node)
        {
            Filter filter = new Filter("class", "champ-abilities__item__letter");
            var test = HtmlScraper.SearchNodeForValue(node, filter);
            var key = HtmlScraper.SearchNodeForValue(node, filter)?
                .Where(x => x.Name.Contains("text"))
                .FirstOrDefault()
                .InnerText.Replace("\n", "");

            if (key == null)
            {
                key = "Passive";
            }

            return key;
        }

        public string GetDescription(HtmlNode node)
        {
            Filter filter = new Filter("class", "champ-abilities__item__desc");

            string description = HtmlScraper.SearchNodeForValue(node, filter)?
                .Where(x => x.Name.Contains("text"))
                .ToArray()
                .Aggregate("", (x, y) => x += y.InnerText);

            return description;
        }

        public List<string> GetCost(HtmlNode node)
        {
            Filter filter = new Filter("class", "champ-abilities__item__cost");

            var cost = HtmlScraper.SearchNodeForValue(node, filter)
                .Where(x => x.Name.Contains("text"))
                .FirstOrDefault()
                .InnerText.Replace("\n", "").Replace(" / ", " ")
                .Split(" ")
                .ToList();
      
            return cost;
        }

        private List<string> GetRange(HtmlNode node)
        {
            List<string> range = new List<string>();

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__range")
                {
                    var rangeString = new string((from i in element.ChildNodes[0].InnerText
                                                 where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                                 select i
                                                ).ToArray()).Replace("\n", " ");
                    var rangeArray = rangeString.Split(" ").Where(x => x.ToString() != "").ToArray();
                    if (rangeArray.Length > 0)
                    {
                        foreach (var rangeElement in rangeArray)
                        {
                            range.Add(rangeElement);
                        }
                    }
                }

            }

            return range;
        }
    }
}
