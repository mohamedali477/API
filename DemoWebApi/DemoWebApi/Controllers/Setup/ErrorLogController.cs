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

namespace DemoWebApi.Controllers.Setup
{
    [System.Web.Http.RoutePrefix("api/errorLog")]
    public class ErrorLogController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("logAngularError")]
        public HttpResponseMessage logAngularError([FromBody] ErrorLogModel errorObject)
        {
            try
            {
                var l = BaseClass.GetRequestHeader(this).currentSchoolId;
                errorObject.schoolId = new MongoDB.Bson.ObjectId(l);
                errorObject.schoolBranchId = new ObjectId(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
                errorObject.userId = new ObjectId(BaseClass.GetRequestHeader(this).currentUserId);
                errorObject.userRoleId = new ObjectId(BaseClass.GetRequestHeader(this).currentUserRoleId);
            }
            catch (Exception) { }

            WriteResult result = new WriteResult("Student and Guardian information saved successfully", true);

            var db = BaseClass.GetDatabase("StuG");
            var mongoCollection = db.GetCollection<BsonDocument>("AngularError");

            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(errorObject));

            var filter = new BsonDocument("message", document.GetValue("message"));
            filter = filter.Add("functionName", document.GetValue("functionName"));

            try
            {
                mongoCollection.ReplaceOne(
                    filter: filter,
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
    }
}
