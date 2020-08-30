using System;
using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson;

namespace lol_cd_quiz
{
    public class Database
    {
        public Database()
        {
        }

        public static void UpdateDatabase()
        {
            IMongoDatabase database = ConnectMongoDb();
            ChampionList championList = new ChampionList();
            IMongoCollection<Champion> championCollection = database.GetCollection<Champion>("champions");

            foreach (var champion in championList.Champions)
            {
                FilterDefinition<Champion> filter = Builders<Champion>.Filter.Eq(x => x.Name, champion.Name);
                var foundChampion = championCollection.Find(filter).FirstOrDefault();

                if (foundChampion != null)
                {
                    var championFilter = Builders<Champion>.Filter.Eq("Name", champion.Name);
                    var championUpdate = Builders<Champion>.Update.Set("Abilities", champion.Abilities);
                    championCollection.UpdateOne(championFilter, championUpdate);
                } else
                {
                    championCollection.InsertOne(champion);
                }

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
