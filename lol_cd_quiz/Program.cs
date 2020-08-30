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
            Database.UpdateDatabase(Champion.GetChampions());

            Console.ReadLine();
        }     
    }
}
