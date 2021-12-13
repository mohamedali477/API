using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Http;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers
{
    public static class BaseClass
    {
        private const string mongoAtlas = @"Provide your mongo atlas or your mongo db connection string";
        private const string local = @"mongodb://localhost";


        static IMongoClient client;
        private static string connectionString;

        public static string DevTeamDB
        {
            get { return "DevTeamDB"; }
        }
        public static DateTime IndianDateTime
        {
            get
            {
                DateTime utcTime = DateTime.UtcNow;

                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // convert from utc to Indian time zone

                return localTime;
            }
        }

        static BaseClass()
        {
            var dbServer = ConfigurationManager.AppSettings["dbServer"];

            switch (dbServer)
            {
                case "mongoAtlas":
                {
                    connectionString = mongoAtlas;
                    break;
                }
                default:
                {

                    connectionString = local;
                    break;
                }
            }

            client = new MongoClient(connectionString);
        }

        public static List<string> GetAllDatabaseFullInfo(int includeDefault)
        {
            string[] defaultDbs = new[] {"admin", "config", "local"};

            List<string> databaseList = new List<string>();

            var client = new MongoClient(connectionString);

            using (IAsyncCursor<BsonDocument> cursor = client.ListDatabases())
            {
                while (cursor.MoveNext())
                {
                    foreach (var db in cursor.Current)
                    {
                        if (includeDefault == 0 && defaultDbs.Contains(db["name"].ToString()))
                        {
                            continue;
                        }

                        var jsonWriterSettings = new JsonWriterSettings {OutputMode = JsonOutputMode.Strict};
                        var json = db.ToJson(jsonWriterSettings);

                        databaseList.Add(json);
                    }
                }
            }

            return databaseList;
        }

        public static List<string> GetAllDatabaseNames(int includeDefault = 0)
        {
            string[] defaultDbs = new[] {"admin", "config", "local"};

            List<string> databaseList = new List<string>();

            var client = new MongoClient(connectionString);

            using (IAsyncCursor<string> cursor = client.ListDatabaseNames())
            {
                while (cursor.MoveNext())
                {
                    foreach (var db in cursor.Current)
                    {
                        if (includeDefault == 0 && defaultDbs.Contains(db))
                        {
                            continue;
                        }

                        databaseList.Add(db);
                    }
                }
            }

            return databaseList;
        }

        public static IMongoDatabase GetDatabase(string schoolBranchId)
        {
            return client.GetDatabase(schoolBranchId);
        }

        public static AuthorizationHeader GetRequestHeader(ApiController apiController)
        {
            var authHeader = apiController.Request.Headers.GetValues("Authorization");

            string authorization = "";

            foreach (var auth in authHeader)
            {
                authorization = auth;
            }

            var session = HttpContext.Current.Session;
            if (session != null)
            {
                if (session["authorization"] == null)
                {
                    session["authorization"] = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationHeader>(authorization);
                }
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationHeader>(authorization);
        }

        public static long convertMSTicksToJavascriptTics(long ticks)
        {
            ticks = ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks;
            return (ticks / 10000) - 19800000; // 19800000 is +5.30 hours
        }

        public static long computeRatingDate()
        {
            var today = IndianDateTime;
            var year = today.Year;
            var month = today.Month;

            if (today.Day <= 10)
            {
                return convertMSTicksToJavascriptTics(new DateTime(year, month, 1, 0,0,0).Ticks);
            }
            else if (today.Day <= 20)
            {
                return convertMSTicksToJavascriptTics(new DateTime(year, month, 11,0,0,0).Ticks);
            }
            else
            {
                return convertMSTicksToJavascriptTics(new DateTime(year, month, 21,0,0,0).Ticks);
            }
        }

        public static long computeToDayTicks()
        {
            var today = IndianDateTime;
            var year = today.Year;
            var month = today.Month;
            var day = today.Day;

            var msTodayTics = new DateTime(year, month, day, 0, 0, 0).Ticks;

            return convertMSTicksToJavascriptTics(msTodayTics);
        }

        public static ObjectId? convertStringObjectIdToMongoObjectId(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            string mongoStringId = Convert.ToString(obj);
            mongoStringId = mongoStringId.Replace("{\r\n  \"$oid\": \"", "");
            mongoStringId = mongoStringId.Replace("\"\r\n}", "");
            return new ObjectId(mongoStringId);
        }

        public static bool compare<T>(T searchArgument, T databaseField)
        {
            if (searchArgument is string)
            {
                return string.IsNullOrEmpty(Convert.ToString(searchArgument)) || string.Equals(
                           Convert.ToString(searchArgument), Convert.ToString(databaseField),
                           StringComparison.OrdinalIgnoreCase);
            }
            else if (searchArgument is int)
            {
                return Convert.ToInt32(searchArgument) == Convert.ToInt32(databaseField);
            }
            else if (searchArgument is double)
            {
                return Convert.ToDouble(searchArgument) == Convert.ToDouble(databaseField);
            }
            else if (searchArgument is long)
            {
                return Convert.ToInt64(searchArgument) == Convert.ToInt64(databaseField);
            }
            else
            {
                return true;
            }

        }

        public static List<BsonDocument> ExecuteQuery(IMongoCollection<BsonDocument> mongoCollection,
            FilterDefinition<BsonDocument> filter, Paging paging, Sorting sorting)
        {
            if (sorting.isAsc)
            {
                return mongoCollection.Find(filter)
                    .Skip(paging.pageIndex * paging.pageSize)
                    .SortBy(bosn => bosn[sorting.fieldName])
                    .Limit(paging.pageSize).ToList();
            }

            return mongoCollection.Find(filter)
                .Skip(paging.pageIndex * paging.pageSize)
                .SortByDescending(bosn => bosn[sorting.fieldName])
                .Limit(paging.pageSize).ToList();
        }
    }
}