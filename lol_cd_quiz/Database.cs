using System;
using MongoDB.Driver;
using LanguageExt;
using MongoDB.Driver.Linq;

namespace lol_cd_quiz
{
    public class Database
    {
        public Database()
        {
        }

        public static async void UpdateDatabase()
        {
            IMongoDatabase database = ConnectMongoDb();
            ChampionList championList = new ChampionList();
            IMongoCollection<Champion> championCollection = database.GetCollection<Champion>("champions");

            foreach (var champion in championList.Champions)
            {
                var filter = Builders<Champion>.Filter.Eq(x => x.Name, champion.Name);
                Option<Champion> foundChampion = await championCollection.Find(filter).SingleOrDefaultAsync();
                Console.WriteLine(foundChampion);
                foundChampion
                    .Some(x =>
                    {
                        var championFilter = Builders<Champion>.Filter.Eq("Name", champion.Name);
                        var championUpdate = Builders<Champion>.Update.Set("Abilities", champion.Abilities);
                        championCollection.UpdateOne(championFilter, championUpdate);
                    })
                    .None(() => championCollection.InsertOne(champion));

                championCollection.InsertOne(champion);
            };
        }

        public static IMongoDatabase ConnectMongoDb()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://thines92:Aidran001@cluster0.t6myq.mongodb.net/lol_db?retryWrites=true&w=majority");

            var database = dbClient.GetDatabase("lol_db");

            return database;
        }
    }
}
