using System;
using System.Linq;
using HtmlAgilityPack;
namespace lol_cd_quiz
{
    public class Ability
    {
        public Ability(HtmlNode node)
        {
            Name = GetName(node);
            Key = GetKey(node);
            Cooldown = GetCooldown(node);
            Description = GetDescription(node);
        }

        public string Name { get; set; }
        public string Key { get; set; }
        public string Cooldown { get; set; }
        public string Description { get; set; }

        public String GetCooldown(HtmlNode node)
        {
            var cooldown = "";

            foreach (HtmlNode element in node.ChildNodes)
            {
                if (element.Attributes["class"] != null && element.Attributes["class"].Value == "champ-abilities__item__cooldown")
                {
                    var cooldowns = new string((from i in element.InnerText
                                                where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                                select i
                                                ).ToArray()).Replace("\n", "");
                    var cooldownArray = cooldowns.Split(" ");
                    cooldown = cooldownArray[0];

                    if (cooldown == "")
                    {
                        cooldown = "0";
                    }
                }

            }

            return cooldown;
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
    }
}
