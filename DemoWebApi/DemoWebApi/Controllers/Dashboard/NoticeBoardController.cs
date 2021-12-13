using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Dashboard
{
    [System.Web.Http.RoutePrefix("api/dashboard")]
    public class NoticeBoardController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getNoticeBoard")]
        public HttpResponseMessage getNoticeBoard()
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("NoticeBoard");

            var documents = mongoCollection.Find(FilterDefinition<BsonDocument>.Empty).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveNoticeBoard")]
        public HttpResponseMessage saveNoticeBoard([FromBody]NoticeBoardModel noticeData)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            
            db.DropCollection("NoticeBoard");
            var mongoCollection = db.GetCollection<BsonDocument>("NoticeBoard");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(noticeData));

            mongoCollection.InsertOne(document);

            var result = new WriteResult("Notice Board saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
