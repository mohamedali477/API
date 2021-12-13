using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using DemoWebApi.Controllers.Holiday;
using DemoWebApi.Models;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Holiday
{
    [System.Web.Http.RoutePrefix("api/holiday")]
    public class HolidayController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getSchoolHolidayList")]
        public HttpResponseMessage getSchoolHolidayList([FromBody]HolidaySearch holidaySearch)
        {
            FilterDefinition<HolidayModel> filter = new BsonDocumentFilterDefinition<HolidayModel>(new BsonDocument());

            #region---------- Search and Sort Criteria -----------------------
            var searchParams = holidaySearch.SearchParameters;
            var paging = holidaySearch.paging;
            var sorting = holidaySearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.holidayName))
                {
                    filter = filter & Builders<HolidayModel>.Filter.Regex(x => x.holidayName, BsonRegularExpression.Create(new Regex(searchParams.holidayName, RegexOptions.IgnoreCase)));
                }

                if (searchParams.fromDate != null)
                {
                    filter = filter & Builders<HolidayModel>.Filter.Where(x => x.holidayDate >= searchParams.fromDate);
                }

                if (searchParams.toDate != null)
                {
                    filter = filter & Builders<HolidayModel>.Filter.Where(x => x.holidayDate <= searchParams.toDate);
                }
            }

            var sort = Builders<HolidayModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<HolidayModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<HolidayModel>("SchoolHoliday");



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
        [System.Web.Http.Route("getSchoolHolidayByDate")]
        public HttpResponseMessage getSchoolHolidayByDate(long holidayDate)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var data = new
            {
                schoolHoliday = getSchoolHolidayByDate(db, holidayDate)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSchoolHolidayByDateRange")]
        public HttpResponseMessage getSchoolHolidayByDateRange(long startDate, long endDate)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var data = new
            {
                schoolHoliday = getSchoolHolidayByDateRange(db, startDate, endDate)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSchoolHoliday")]
        public HttpResponseMessage getSchoolHoliday(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolHoliday");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveSchoolHoliday")]
        public HttpResponseMessage saveSchoolHoliday([FromBody]HolidayModel holidayData)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("SchoolHoliday");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(holidayData));

                mongoCollection.ReplaceOneAsync(
                filter: new BsonDocument("_id", document.GetValue("_id")),
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);

            var result = new WriteResult("School Holiday saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private List<HolidayModel> getSchoolHolidayByDate(IMongoDatabase db, long date)
        {
            FilterDefinition<HolidayModel> filter = new BsonDocumentFilterDefinition<HolidayModel>(new BsonDocument());


            filter = filter & Builders<HolidayModel>.Filter.Where(x =>
                         x.holidayDate >= date);
            filter = filter & Builders<HolidayModel>.Filter.Where(x =>
                    x.holidayDate <= date);

            var collection = db.GetCollection<HolidayModel>("SchoolHoliday");
            var result = collection.Find(filter).ToList();

            return result;
        }

        public List<HolidayModel> getSchoolHolidayByDateRange(IMongoDatabase db, long firstDay, long lastDay)
        {
            FilterDefinition<HolidayModel> filter = new BsonDocumentFilterDefinition<HolidayModel>(new BsonDocument());

            filter = filter & Builders<HolidayModel>.Filter.Where(x =>
                         x.holidayDate >= firstDay);
            filter = filter & Builders<HolidayModel>.Filter.Where(x =>
                x.holidayDate <= lastDay);

            var sort = Builders<HolidayModel>.Sort.Ascending("holidayDate");

            var collection = db.GetCollection<HolidayModel>("SchoolHoliday");
            var result = collection.Find(filter).Sort(sort).ToList();

            return result;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteSchoolHoliday")]
        public HttpResponseMessage deleteSchoolHoliday(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolHoliday");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("School Holiday deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
