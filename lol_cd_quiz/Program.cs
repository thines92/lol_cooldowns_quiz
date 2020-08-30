using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Linq;
using System.Data.SqlClient;
using LanguageExt;
using MongoDB.Bson;
using System.Web;

namespace lol_cd_quiz
{
    class Program
    {
        static void Main(string[] args)
        {
            var championUrls = GetChampionUrls("https://www.mobafire.com/league-of-legends/champions");

            var champions = GetChampions(championUrls);

            UpdateDatabase(champions);
            Console.ReadLine();
        }

        private static async void UpdateDatabase(List<Champion> champions)
        {
            IMongoDatabase database = ConnectMongoDb();
            IMongoCollection<Champion> championCollection = database.GetCollection<Champion>("champions");

            foreach (var champion in champions)
            {
                var filter = Builders<Champion>.Filter.Eq(x => x.Name, champion.Name);
                Option<Champion> foundChampion = await championCollection.Find(filter).SingleOrDefaultAsync();

                foundChampion
                    .Some(x =>
                    {
                        var championFilter = Builders<Champion>.Filter.Eq("Name", champion.Name);
                        var championUpdate = Builders<Champion>.Update.Set("Cooldowns", champion.Abilities);
                        championCollection.UpdateOne(championFilter, championUpdate);
                    })
                    .None(() => championCollection.InsertOne(champion));
            };
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

            //for (var i = 0; i < 6; i++)
            //{
            //    var href = champNodes[i].GetAttributeValue("href", string.Empty);
            //    var championPageUrl = "https://www.mobafire.com" + href;
            //    championUrls.Add(championPageUrl);
            //}

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
            champion.Abilities = new List<Ability>();
            var championAbilityNodes = GetHtmlNodes(url, "//a[contains(@class, 'champ-abilities__item ')]");
            var championCooldownNodes = GetHtmlNodes(url, "//div[contains(@class, 'champ-abilities__item__cooldown')]");

            foreach (HtmlNode node in championAbilityNodes)
            {
                var ability = GetAbilityInfo(node);
                champion.Abilities.Add(ability);

                //champion.Cooldowns.Add(championCooldown);
            }

            return champion;
        }

        private static Ability GetAbilityInfo(HtmlNode node)
        {
            Ability ability = new Ability();
            ability.Cooldown = ability.GetCooldown(node);
            ability.Name = ability.GetName(node);
            ability.Key = ability.GetKey(node);

            return ability;
        }

        private static String GetChampionCooldown(HtmlNode node)
        {
            var nodeText = node.SelectSingleNode("//div[contains(@class, 'champ-abilities__item__cooldown')]").InnerText.Replace("\n", "");
            var cooldowns = new string((from i in nodeText
                                       where char.IsLetterOrDigit(i) || char.IsWhiteSpace(i)
                                       select i
                                       ).ToArray()).Replace(Environment.NewLine, "");
            var cooldownArray = cooldowns.Split(" ");
            var cooldown = cooldownArray[0];

            return cooldown;
        }

        private static IMongoDatabase ConnectMongoDb()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://thines92:Aidran001@cluster0.t6myq.mongodb.net/lol_db?retryWrites=true&w=majority");

            var database = dbClient.GetDatabase("lol_db");

            return database;
        }

        public class Champion
        {
            public ObjectId _id { get; set; }
            public string Name { get; set; }
            public List<Ability> Abilities { get; set; }
        }

        public class Ability
        {
            public string Name { get; set; }
            public string Key { get; set; }
            public string Cooldown { get; set; }

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
        }
    }
}
