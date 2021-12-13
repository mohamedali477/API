using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Mvc;
using DemoWebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace DemoWebApi.Controllers.Student
{
    [System.Web.Http.RoutePrefix("api/student")]
    public class StudentController : ApiController
    {
        // GET api/<controller>
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getlist")]
        public HttpResponseMessage getlist([FromBody]StudentSearch studentSearch)
        {
            FilterDefinition<StudentModel> filter = new BsonDocumentFilterDefinition<StudentModel>(new BsonDocument());
            
            #region---------- Search and Sort Criteria -----------------------
            var searchParams = studentSearch.SearchParameters;
            var paging = studentSearch.paging;
            var sorting = studentSearch.sorting;

            if (searchParams != null)
            {
                if (searchParams.classId != null)
                {
                    filter = filter & Builders<StudentModel>.Filter.Where(x => (ObjectId) x.academicInfo.classId == BaseClass.convertStringObjectIdToMongoObjectId(searchParams.classId));
                }

                if (searchParams.sectionId != null)
                {
                    filter = filter & Builders<StudentModel>.Filter.Where(x => x.academicInfo.sectionId == searchParams.sectionId);
                }

                if (!string.IsNullOrEmpty(searchParams.firstName))
                {
                    filter = filter & Builders<StudentModel>.Filter.Regex(x => x.basicInfo.firstName, BsonRegularExpression.Create(new Regex(searchParams.firstName, RegexOptions.IgnoreCase)));
                }

                if (!string.IsNullOrEmpty(searchParams.lastName))
                {
                    filter = filter & Builders<StudentModel>.Filter.Regex(x => x.basicInfo.lastName, BsonRegularExpression.Create(new Regex(searchParams.lastName, RegexOptions.IgnoreCase)));
                }

                if (searchParams.dob != null)
                {
                    filter = filter & Builders<StudentModel>.Filter.Where(x => x.basicInfo.dob == searchParams.dob);
                }

                if (searchParams.castId != null)
                {
                    filter = filter & Builders<StudentModel>.Filter.Where(x => x.basicInfo.castId == searchParams.castId);
                }

                if (searchParams.genderId != null)
                {
                    filter = filter & Builders<StudentModel>.Filter.Where(x => x.basicInfo.genderId == searchParams.genderId);
                }
                if (searchParams.religionId != null)
                {
                    filter = filter & Builders<StudentModel>.Filter.Where(x => x.basicInfo.religionId == searchParams.religionId);
                }
            }

            var sort = Builders<StudentModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<StudentModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<StudentModel>("view_students");

            

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
        [System.Web.Http.Route("Deactivate")]
        public HttpResponseMessage Deactivate(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("StudentGuardian");


            #region---------- Serach Criteria -----------------------
            var filterBuilder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            filter = filterBuilder.Eq("guardians._id", new ObjectId(id));
            #endregion


            var builder = Builders<BsonDocument>.Update;
            UpdateDefinition<BsonDocument> update = builder.Set("guardians.status", 0);

            var result = mongoCollection.UpdateOne(filter, update);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
    

}