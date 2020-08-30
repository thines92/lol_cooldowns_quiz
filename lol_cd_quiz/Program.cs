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
            var championUrls = Champion.GetChampionUrls("https://www.mobafire.com/league-of-legends/champions");

            var champions = Champion.GetChampions(championUrls);

            Database.UpdateDatabase(champions);
            Console.ReadLine();
        }     
    }
}
