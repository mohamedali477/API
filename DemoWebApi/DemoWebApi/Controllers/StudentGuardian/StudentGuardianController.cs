using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using DemoWebApi.Models;
using DemoWebApi.Models.StudentGuardian;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DemoWebApi.Controllers.Student
{
    [System.Web.Http.RoutePrefix("api/StudentGuardian")]
    public class StudentGuardianController : ApiController
    {
        // GET api/<controller>
        [System.Web.Http.Route("getStudentGuardian")]
        public HttpResponseMessage Get(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("StudentGuardian");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);
            
            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        // POST api/<controller>
        [System.Web.Http.Route("saveStudentGuardian")]
        public HttpResponseMessage Post([FromBody]StudentGuardianModel value)
        {
            WriteResult result = new WriteResult("Student and Guardian information saved successfully", true);

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("StudentGuardian");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(value));

            try
            {
                mongoCollection.ReplaceOne(
                filter: new BsonDocument("_id", document.GetValue("_id")),
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);
            }
            catch (MongoWriteException ex)
            {
                result.message = "Login Id Already exists";
                result.isSuccess = false;
            }


            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("DeleteStudentGuardian")]
        public HttpResponseMessage DeleteStudentGuardian(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("StudentGuardian");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("Student and Guardian information get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

    }

}