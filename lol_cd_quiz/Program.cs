using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Linq;
using LanguageExt;
using MongoDB.Bson;

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

            foreach (HtmlNode node in championAbilityNodes)
            {
                var ability = new Ability(node);
                champion.Abilities.Add(ability);
            }

            return champion;
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
    }
}
