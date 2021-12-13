using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DemoWebApi.Controllers.Exam;
using DemoWebApi.Models;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Exam
{
    [System.Web.Http.RoutePrefix("api/exam")]
    public class ExamController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getSchoolExamList")]
        public HttpResponseMessage getSchoolExamList([FromBody]ExamSearch examSearch)
        {
            FilterDefinition<ExamModel> filter = new BsonDocumentFilterDefinition<ExamModel>(new BsonDocument());

            #region---------- Search and Sort Criteria -----------------------
            var searchParams = examSearch.SearchParameters;
            var paging = examSearch.paging;
            var sorting = examSearch.sorting;
            
            if (searchParams != null)
            {
                if (searchParams.academicSessionId != null)
                {
                    filter = filter & Builders<ExamModel>.Filter.Where(x => x.academicSessionId == searchParams.academicSessionId);
                }

                if (searchParams.examTypeId != null)
                {
                    filter = filter & Builders<ExamModel>.Filter.Where(x => (ObjectId)x.examTypeId == BaseClass.convertStringObjectIdToMongoObjectId(searchParams.examTypeId));
                }

                if (searchParams.classId != null)
                {
                    filter = filter & Builders<ExamModel>.Filter.Where(x => (ObjectId) x.classId == BaseClass.convertStringObjectIdToMongoObjectId(searchParams.classId));
                }
            }
            

            var sort = Builders<ExamModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<ExamModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<ExamModel>("SchoolExam");



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
        [System.Web.Http.Route("getSchoolExamByDate")]
        public HttpResponseMessage getSchoolExamByDate(long examDate)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var data = new
            {
                schoolExam = getSchoolExamByDate(db, examDate)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSchoolExamByDateRange")]
        public HttpResponseMessage getSchoolExamByDateRange(long startDate, long endDate)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var data = new
            {
                schoolExam = getSchoolExamByDateRange(db, startDate, endDate)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getSchoolExam")]
        public HttpResponseMessage getSchoolExam(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("view_schoolExam_StuDetails");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getExamResultStudentWise")]
        public HttpResponseMessage getExamResultStudentWise(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("view_examResultStudentWise_StuDetails");

            var documents = mongoCollection.Find(x => x["_id"]["examId"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveSchoolExam")]
        public HttpResponseMessage saveSchoolExam([FromBody]ExamModel examData)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("SchoolExam");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(examData));

            mongoCollection.ReplaceOne(
            filter: new BsonDocument("_id", document.GetValue("_id")),
            options: new UpdateOptions { IsUpsert = true },
            replacement: document);

            var result = new WriteResult("School Exam saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveSpecificSubjectSectionExamResult")]
        public HttpResponseMessage saveSpecificSubjectSectionExamResult([FromBody]sectionResultModel resultData)
        {
           // sectionResultModel resultData = BsonSerializer.Deserialize<sectionResultModel>(r);

           var examId = new ObjectId(resultData.examId.ToString());
           var subjectId = new ObjectId(resultData.subjectId.ToString());

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var documents = db.GetCollection<BsonDocument>("SchoolExam").Find(x => (ObjectId)x["_id"] == examId).ToList();
            var documents1 = db.GetCollection<ExamModel>("SchoolExam").Find(x => (ObjectId)x._id == examId).ToList();
            
            if (documents.Count == 1)
            {
                foreach (var subject in documents[0]["subject"].AsBsonArray)
                {
                    if ((ObjectId) subject["subjectId"] == subjectId)
                    {
                        for (int idx=0; idx < subject["sections"].AsBsonArray.Count; idx++)
                        {
                            var section = (subject["sections"].AsBsonArray)[idx];

                            if (section["sectionId"] == resultData.sectionResult.sectionId)
                            {
                                subject["sections"][idx] = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(resultData.sectionResult));
                                break;
                            }
                        }
                    }
                }
            }
            
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolExam");
            var r = mongoCollection.ReplaceOne(
                filter: new BsonDocument("_id", new BsonObjectId(resultData.examId.ToString())),
                options: new UpdateOptions { IsUpsert = true },
                replacement: documents[0]);

            var result = new WriteResult("School Exam saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private List<ViewSchoolExamModel> getSchoolExamByDate(IMongoDatabase db, long date)
        {
            FilterDefinition<ViewSchoolExamModel> filter = new BsonDocumentFilterDefinition<ViewSchoolExamModel>(new BsonDocument());

            filter = filter & Builders<ViewSchoolExamModel>.Filter.Where(x => x.examDate >= date);
            filter = filter & Builders<ViewSchoolExamModel>.Filter.Where(x => x.examDate <= date);
                    
            var collection = db.GetCollection<ViewSchoolExamModel>("view_school_exam");
            var result = collection.Find(filter).ToList();

            return result;
        }

        public List<ViewSchoolExamModel> getSchoolExamByDateRange(IMongoDatabase db, long firstDay, long lastDay)
        {
            FilterDefinition<ViewSchoolExamModel> filter = new BsonDocumentFilterDefinition<ViewSchoolExamModel>(new BsonDocument());

            filter = filter & Builders<ViewSchoolExamModel>.Filter.Where(x => x.examDate >= firstDay);
            filter = filter & Builders<ViewSchoolExamModel>.Filter.Where(x => x.examDate <= lastDay);
         
            var sort = Builders<ViewSchoolExamModel>.Sort.Ascending("examDate");

            var collection = db.GetCollection<ViewSchoolExamModel>("view_school_exam");
            var result = collection.Find(filter).Sort(sort).ToList();

            return result;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteSchoolExam")]
        public HttpResponseMessage deleteSchoolExam(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolExam");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("School Exam deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
