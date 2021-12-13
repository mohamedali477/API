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

namespace DemoWebApi.Controllers.Subject
{
    [System.Web.Http.RoutePrefix("api/subject")]
    public class SubjectController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getSchoolSubjectList")]
        public HttpResponseMessage getSchoolSubjectList([FromBody]SubjectSearch subjectSearch)
        {
            MongoDB.Driver.FilterDefinition<SubjectModel> filter = new MongoDB.Driver.BsonDocumentFilterDefinition<SubjectModel>(new BsonDocument());

            #region---------- Search and Sort Criteria -----------------------
            var searchParams = subjectSearch.SearchParameters;
            var paging = subjectSearch.paging;
            var sorting = subjectSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.name))
                {
                    filter = filter & Builders<SubjectModel>.Filter.Regex(x => x.name, BsonRegularExpression.Create(new Regex(searchParams.name, RegexOptions.IgnoreCase)));
                }

                if (!string.IsNullOrEmpty(searchParams.code))
                {
                    filter = filter & Builders<SubjectModel>.Filter.Regex(x => x.code, BsonRegularExpression.Create(new Regex(searchParams.code, RegexOptions.IgnoreCase)));
                }
            }

            var sort = Builders<SubjectModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<SubjectModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<SubjectModel>("SchoolSubject");



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
        [System.Web.Http.Route("getSchoolSubject")]
        public HttpResponseMessage getSchoolSubject(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolSubject");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveSchoolSubject")]
        public HttpResponseMessage saveSchoolSubject([FromBody]SubjectModel subjectData)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("SchoolSubject");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(subjectData));

            mongoCollection.ReplaceOneAsync(
            filter: new BsonDocument("_id", document.GetValue("_id")),
            options: new UpdateOptions { IsUpsert = true },
            replacement: document);

            var result = new WriteResult("School Subject saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSubjectDdl")]
        public HttpResponseMessage getSubjectDdl()
        {
            var documents = masterDataSubjectDdl(this);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteSchoolSubject")]
        public HttpResponseMessage deleteSchoolSubject(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolSubject");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("School Subject deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public List<SubjectDdlModel> masterDataSubjectDdl(ApiController apiController)
        {
            FilterDefinition<SubjectDdlModel> filter = new BsonDocumentFilterDefinition<SubjectDdlModel>(new BsonDocument());

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(apiController).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<SubjectDdlModel>("view_subject_ddl");

            return mongoCollection.Find(filter).ToList();
        }
    }
}