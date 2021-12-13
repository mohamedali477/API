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

namespace DemoWebApi.Controllers.Exam
{
    [System.Web.Http.RoutePrefix("api/examType")]
    public class ExamTypeController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getSchoolExamTypeList")]
        public HttpResponseMessage getSchoolExamTypeList([FromBody]ExamTypeSearch examTypeSearch)
        {
            MongoDB.Driver.FilterDefinition<ExamTypeModel> filter = new MongoDB.Driver.BsonDocumentFilterDefinition<ExamTypeModel>(new BsonDocument());

            #region---------- Search and Sort Criteria -----------------------
            var searchParams = examTypeSearch.SearchParameters;
            var paging = examTypeSearch.paging;
            var sorting = examTypeSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.name))
                {
                    filter = filter & Builders<ExamTypeModel>.Filter.Regex(x => x.name, BsonRegularExpression.Create(new Regex(searchParams.name, RegexOptions.IgnoreCase)));
                }

                if (!string.IsNullOrEmpty(searchParams.code))
                {
                    filter = filter & Builders<ExamTypeModel>.Filter.Regex(x => x.code, BsonRegularExpression.Create(new Regex(searchParams.code, RegexOptions.IgnoreCase)));
                }
            }

            var sort = Builders<ExamTypeModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<ExamTypeModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<ExamTypeModel>("SchoolExamType");



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
        [System.Web.Http.Route("getSchoolExamType")]
        public HttpResponseMessage getSchoolExamType(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolExamType");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveSchoolExamType")]
        public HttpResponseMessage saveSchoolExamType([FromBody]ExamTypeModel examTypeData)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("SchoolExamType");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(examTypeData));

            mongoCollection.ReplaceOneAsync(
            filter: new BsonDocument("_id", document.GetValue("_id")),
            options: new UpdateOptions { IsUpsert = true },
            replacement: document);

            var result = new WriteResult("School ExamType saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getExamTypeDdl")]
        public HttpResponseMessage getExamTypeDdl()
        {
            var documents = masterDataExamTypeDdl(this);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteSchoolExamType")]
        public HttpResponseMessage deleteSchoolExamType(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolExamType");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("School ExamType deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public List<ExamTypeDdlModel> masterDataExamTypeDdl(ApiController apiController)
        {
            FilterDefinition<ExamTypeDdlModel> filter = new BsonDocumentFilterDefinition<ExamTypeDdlModel>(new BsonDocument());

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(apiController).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<ExamTypeDdlModel>("view_examType_ddl");

            return mongoCollection.Find(filter).ToList();
        }
    }
}
