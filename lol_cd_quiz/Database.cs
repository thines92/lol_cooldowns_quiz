using System;
using System.Collections.Generic;
using MongoDB.Driver.Core;
using MongoDB.Driver;
using LanguageExt;

namespace lol_cd_quiz
{
    public class Database
    {
        public Database()
        {
        }

        public static async void UpdateDatabase(List<Champion> champions)
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

        public static IMongoDatabase ConnectMongoDb()
        {
            MongoClient dbClient = new MongoClient("mongodb+srv://thines92:Aidran001@cluster0.t6myq.mongodb.net/lol_db?retryWrites=true&w=majority");

            var database = dbClient.GetDatabase("lol_db");

            return database;
        }
    }
}
