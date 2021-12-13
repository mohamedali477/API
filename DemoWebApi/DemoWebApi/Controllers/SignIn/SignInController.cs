using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DemoWebApi.Controllers.Guardian;
using DemoWebApi.Controllers.Student;
using DemoWebApi.Models.Employee;
using DemoWebApi.Models.StudentGuardian;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.SignIn
{
    [System.Web.Http.RoutePrefix("api/SignIn")]
    public class SignInController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("authenticate")]
        public HttpResponseMessage authenticate([FromBody] SignInModel signInValue)
        {
            if (signInValue.userType.Equals("employee"))
            {
                return employee(signInValue);
            }
            else if (signInValue.userType.Equals("student"))
            {
                return student(signInValue);
            }
            else if (signInValue.userType.Equals("guardian"))
            {
                return guardian(signInValue);
            }
            else
            {
                return devTeam(signInValue);
            }
        }

        private HttpResponseMessage employee(SignInModel signInValue)
        {
            FilterDefinition<EmployeeModel>
                filter = new BsonDocumentFilterDefinition<EmployeeModel>(new BsonDocument());

            if (signInValue != null &&
                !(string.IsNullOrEmpty(signInValue.loginId) && string.IsNullOrEmpty(signInValue.password))
            )
            {
                filter = filter & Builders<EmployeeModel>.Filter.Where(x =>
                             x.status > 0);

                filter = filter & Builders<EmployeeModel>.Filter.Where(x =>
                             x.credentialInfo.loginId.ToLower() == signInValue.loginId.ToLower());

                filter = filter & Builders<EmployeeModel>.Filter.Where(x =>
                             x.credentialInfo.password == signInValue.password);
            }

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<EmployeeModel>("Employee");


            var documents = mongoCollection.Find(filter).ToList();

            var jsonWriterSettings = new JsonWriterSettings {OutputMode = JsonOutputMode.Strict};
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private HttpResponseMessage student(SignInModel signInValue)
        {
            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<StudentModel> filter = new BsonDocumentFilterDefinition<StudentModel>(new BsonDocument());

            if (signInValue != null &&
                !(string.IsNullOrEmpty(signInValue.loginId) && string.IsNullOrEmpty(signInValue.password))
            )
            {

                filter = filter & Builders<StudentModel>.Filter.Where(x =>
                             x.status > 0);

                filter = filter & Builders<StudentModel>.Filter.Where(x =>
                             x.credentialInfo.loginId.ToLower() == signInValue.loginId.ToLower());

                filter = filter & Builders<StudentModel>.Filter.Where(x =>
                             x.credentialInfo.password == signInValue.password);
            }

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<StudentModel>("view_students");


            var documents = mongoCollection.Find(filter).ToList();

            var jsonWriterSettings = new JsonWriterSettings {OutputMode = JsonOutputMode.Strict};
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private HttpResponseMessage guardian(SignInModel signInValue)
        {
            var builder = Builders<BsonDocument>.Filter;

            FilterDefinition<GuardianModel>
                filter = new BsonDocumentFilterDefinition<GuardianModel>(new BsonDocument());

            if (signInValue != null &&
                !(string.IsNullOrEmpty(signInValue.loginId) && string.IsNullOrEmpty(signInValue.password))
            )
            {
                filter = filter & Builders<GuardianModel>.Filter.Where(x =>
                             x.status > 0);

                filter = filter & Builders<GuardianModel>.Filter.Where(x =>
                             x.credentialInfo.loginId.ToLower() == signInValue.loginId.ToLower());

                filter = filter & Builders<GuardianModel>.Filter.Where(x =>
                             x.credentialInfo.password == signInValue.password);
            }

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<GuardianModel>("view_guardians");


            var documents = mongoCollection.Find(filter).ToList();

            var jsonWriterSettings = new JsonWriterSettings {OutputMode = JsonOutputMode.Strict};
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private HttpResponseMessage devTeam(SignInModel signInValue)
        {
            FilterDefinition<EmployeeModel>
                filter = new BsonDocumentFilterDefinition<EmployeeModel>(new BsonDocument());

            if (signInValue != null &&
                !(string.IsNullOrEmpty(signInValue.loginId) && string.IsNullOrEmpty(signInValue.password))
            )
            {
                filter = filter & Builders<EmployeeModel>.Filter.Where(x =>
                             x.status > 0);

                filter = filter & Builders<EmployeeModel>.Filter.Where(x =>
                             x.credentialInfo.loginId.ToLower() == signInValue.loginId.ToLower());

                filter = filter & Builders<EmployeeModel>.Filter.Where(x =>
                             x.credentialInfo.password == signInValue.password);
            }

            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var mongoCollection = db.GetCollection<EmployeeModel>("Employee");


            var documents = mongoCollection.Find(filter).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
    }
}
