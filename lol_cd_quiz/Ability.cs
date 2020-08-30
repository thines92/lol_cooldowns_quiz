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
        public List<String> Cooldowns { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
        public List<string> Range { get; set; }

        public string GetIconSrc(HtmlNode node)
        {
            var iconSrc = "https://www.mobafire.com";

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__pic")
                {
                    var iconSrcString = element.ChildNodes[1].Attributes["data-cfsrc"].Value;

                    iconSrc += iconSrcString;
                }

            }

            return iconSrc;
        }

        public List<string> GetCooldowns(HtmlNode node)
        {
            List<string> cooldownArray = new List<string>();

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__cooldown")
                {
                    var cooldowns = new string((from i in element.InnerText
                                                where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                                select i
                                                ).ToArray()).Replace("\n", "");
                    var cooldownsArray = cooldowns.Split(" ").Where(x => x != "").ToArray();

                    if (cooldownsArray.Length > 0)
                    {
                        foreach (var cd in cooldownsArray)
                        {
                            var cdValue = cd;
                            if (cdValue == "")
                            {
                                cdValue = "0";
                            }
                            cooldownArray.Add(cdValue);
                        }
                    } else
                    {
                        cooldownArray.Add("N/A");
                    }
                }

            }

            return cooldownArray;
        }

        public string GetName(HtmlNode node)
        {
            var name = "";

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__name")
                {
                    var nameString = new string((from i in element.ChildNodes[0].InnerText
                                                 where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                                 select i
                                                ).ToArray()).Replace("\n", " ");
                    var nameArray = nameString.Split(" ");
                    name = nameString.Trim();
                }

            }

            return name;
        }

        public string GetKey(HtmlNode node)
        {
            var key = "Passive";

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__letter")
                {
                    var keyString = new string((from i in element.ChildNodes[0].InnerText
                                                where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                                select i
                                                ).ToArray()).Replace("\n", " ");
                    key = keyString.Trim();
                }

            }

            return key;
        }

        public string GetDescription(HtmlNode node)
        {
            var desc = "";

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__desc")
                {
                    var descString = element.ChildNodes.ToList();
                    var newDesc = descString.FindAll(x => x.Name == "#text");

                    foreach (var descElement in newDesc)
                    {
                        desc += descElement.InnerText;
                    }
                }

            }

            return desc;
        }

        public string GetCost(HtmlNode node)
        {
            var cost = "";

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__cost")
                {
                    var costString = new string((from i in element.ChildNodes[0].InnerText
                                                where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                                select i
                                                ).ToArray()).Replace("\n", " ");
                    var costArray = costString.Split(" ").Where(x => x.ToString() != "").ToArray();
                    if (costArray.Length > 0)
                    {
                        cost = costArray[0];
                    }
                }

            }

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
