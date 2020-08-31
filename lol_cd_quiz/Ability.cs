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
            Name = HtmlScraper.GetStringFromNode(node, "champ-abilities__item__name");
            Key = GetKey(node);
            Cooldowns = HtmlScraper.GetListFromNode(node, "champ-abilities__item__cooldown");
            Description = HtmlScraper.GetStringArrayFromNode(node, "champ-abilities__item__desc");
            Cost = HtmlScraper.GetListFromNode(node, "champ-abilities__item__cost");
            Range = HtmlScraper.GetListFromNode(node, "champ-abilities__item__range");
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
            string baseUrl = "https://www.mobafire.com";

            var result = HtmlScraper.GetNodeByClass(node, "champ-abilities__item__pic")
                .Where(x => x.Attributes["data-cfsrc"] != null)
                .SingleOrDefault()
                .Attributes["data-cfsrc"].Value;

            return baseUrl += result;
        }

        public string GetKey(HtmlNode node)
        {
            var key = HtmlScraper.GetStringFromNode(node, "champ-abilities__item__letter");

            if (key == null)
            {
                key = "Passive";
            }

            return key;
        }

    }
}
