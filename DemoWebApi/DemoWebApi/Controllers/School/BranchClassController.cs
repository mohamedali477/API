using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DemoWebApi.Models;
using DemoWebApi.Models.StudentGuardian;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DemoWebApi.Controllers.School
{
    [System.Web.Http.RoutePrefix("api/class")]
    public class BranchClassController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getClass")]
        public HttpResponseMessage getClass(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("Class");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getClassList")]
        public HttpResponseMessage getClassList([FromBody]NoSearchModel classSearch)
        {
            #region---------- Search and Sort Criteria -----------------------
            var builder = Builders<BsonDocument>.Filter;
            FilterDefinition<BranchClassModel> filter = new BsonDocumentFilterDefinition<BranchClassModel>(new BsonDocument());

            var paging = classSearch.paging;
            var sorting = classSearch.sorting;

            var sort = Builders<BranchClassModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<BranchClassModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BranchClassModel>("Class");



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
        
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveClass")]
        public HttpResponseMessage saveClass([FromBody]BranchClassModel branchClass)
        {
            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());
            filter = filter & Builders<BsonDocument>.Filter.Where(x => (ObjectId)x["_id"] == BaseClass.convertStringObjectIdToMongoObjectId(branchClass._id));

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("Class");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(branchClass));
            /*
            var OldDocument = mongoCollection.Find(filter).ToList();
            var newDocument = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(branchClass));

            if (OldDocument.Count > 0)
            {
                foreach (var doc in OldDocument[0]["subject"].AsBsonArray)
                {
                    if (!branchClass.subject.Exists(x => x.sectionId == doc["sectionId"] &&
                        BaseClass.convertStringObjectIdToMongoObjectId(x.subjectId) == (ObjectId)doc["subjectId"]))
                    {
                        doc["status"] = 0;
                        newDocument["subject"].AsBsonArray.Add(doc);
                    }
                }
            }
            */

            mongoCollection.ReplaceOne(
                filter: filter,
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);


            var result = new WriteResult("Class saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteClass")]
        public HttpResponseMessage deleteClass(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("Class");

            mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("Class get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getClassSubjectDdl")]
        public HttpResponseMessage getClassSubjectDdl()
        {
            var documents = masterDataClassSubjectDdl(this);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("ClassSectionWiseStudents")]
        public HttpResponseMessage ClassSectionWiseStudents()
        {
            var documents = masterClassSectionWiseStudents(this);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public List<ClassSubjectDdlModel> masterDataClassSubjectDdl(ApiController apiController)
        {
            FilterDefinition<ClassSubjectDdlModel> filter = new BsonDocumentFilterDefinition<ClassSubjectDdlModel>(new BsonDocument());

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(apiController).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<ClassSubjectDdlModel>("view_class_subject_ddl");

            return mongoCollection.Find(filter).ToList();
        }

        public List<CJClassSectionStudentModel> masterClassSectionWiseStudents(ApiController apiController)
        {
            FilterDefinition<CJClassSectionStudentModel> filter = new BsonDocumentFilterDefinition<CJClassSectionStudentModel>(new BsonDocument());

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(apiController).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<CJClassSectionStudentModel>("view_cj_classSectionStudents");

            return mongoCollection.Find(filter).ToList();
        }
    }
}
