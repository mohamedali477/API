using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Event
{
    [System.Web.Http.RoutePrefix("api/event")]
    public class EventController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getSchoolEventList")]
        public HttpResponseMessage getSchoolEventList([FromBody]EventSearch eventSearch)
        {
            FilterDefinition<EventModel> filter = new BsonDocumentFilterDefinition<EventModel>(new BsonDocument());

            #region---------- Search and Sort Criteria -----------------------
            var searchParams = eventSearch.SearchParameters;
            var paging = eventSearch.paging;
            var sorting = eventSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.eventName))
                {
                    filter = filter & Builders<EventModel>.Filter.Regex(x => x.eventName, BsonRegularExpression.Create(new Regex(searchParams.eventName, RegexOptions.IgnoreCase)));
                }

                if (searchParams.fromDate != null)
                {
                    filter = filter & Builders<EventModel>.Filter.Where(x => x.fromDate >= searchParams.fromDate);
                }

                if (searchParams.toDate != null)
                {
                    filter = filter & Builders<EventModel>.Filter.Where(x => x.toDate <= searchParams.toDate);
                }
            }

            var sort = Builders<EventModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<EventModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<EventModel>("SchoolEvent");



            var data = mongoCollection.Find(filter).Sort(sort).Skip(paging.pageSize * paging.pageIndex).Limit(paging.pageSize).ToList();
            var rowCount = mongoCollection.Find(filter).CountDocuments();

            var result = new Result()
            {
                count = rowCount,
                data = data
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(result);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSchoolEventByDate")]
        public HttpResponseMessage getSchoolEventByDate(long agendaDate)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var data = new
            {
                schoolEvent = getSchoolEventByDate(db, agendaDate)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSchoolEventByDateRange")]
        public HttpResponseMessage getSchoolEventByDateRange(long startDate, long endDate)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var data = new
            {
                schoolEvent = getSchoolEventByDateRange(db, startDate, endDate)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSchoolEvent")]
        public HttpResponseMessage getSchoolEvent(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolEvent");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveSchoolEvent")]
        public HttpResponseMessage saveSchoolEvent([FromBody]EventModel eventData)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("SchoolEvent");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(eventData));

                mongoCollection.ReplaceOneAsync(
                filter: new BsonDocument("_id", document.GetValue("_id")),
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);

            var result = new WriteResult("School Event saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteSchoolEvent")]
        public HttpResponseMessage deleteSchoolEvent(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolEvent");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("School Event get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private List<EventModel> getSchoolEventByDate(IMongoDatabase db, long date)
        {
            FilterDefinition<EventModel> filter = new BsonDocumentFilterDefinition<EventModel>(new BsonDocument());


            filter = filter & Builders<EventModel>.Filter.Where(x =>
                         x.toDate >= date);
            filter = filter & Builders<EventModel>.Filter.Where(x =>
                    x.fromDate <= date);

            var collection = db.GetCollection<EventModel>("SchoolEvent");
            var result = collection.Find(filter).ToList();
            
            return result;
        }

        public List<EventModel> getSchoolEventByDateRange(IMongoDatabase db, long firstDay, long lastDay)
        {
            FilterDefinition<EventModel> filter = new BsonDocumentFilterDefinition<EventModel>(new BsonDocument());

            filter = filter & Builders<EventModel>.Filter.Where(x =>
                         x.toDate >= firstDay);
            filter = filter & Builders<EventModel>.Filter.Where(x =>
                x.fromDate <= lastDay);

            var collection = db.GetCollection<EventModel>("SchoolEvent");
            var result = collection.Find(filter).ToList();

            return result;
        }
    }
}
