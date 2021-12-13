using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using DemoWebApi.Models;
using DemoWebApi.Models.Employee;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Employee
{
    [System.Web.Http.RoutePrefix("api/Employee")]
    public class EmployeeController : ApiController
    {
        [System.Web.Http.Route("getdevSuperAdmin")]
        public HttpResponseMessage getdevSuperAdmin(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var result = this.getEmployeeFromDB(id, db);
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.Route("getEmployee")]
        public HttpResponseMessage getEmployee(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var result = this.getEmployeeFromDB(id, db);
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json")
            };
        }

        private string getEmployeeFromDB(string id, IMongoDatabase db)
        {
            var mongoCollection = db.GetCollection<BsonDocument>("Employee");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            return documents.ToJson(jsonWriterSettings);
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveDevSuperAdmin")]
        public HttpResponseMessage saveDevSuperAdmin([FromBody] EmployeeModel value)
        {
            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var result = saveEmpolyeeToDB(value, db);
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveEmployee")]
        public HttpResponseMessage saveEmployee([FromBody]EmployeeModel value)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var result = saveEmpolyeeToDB(value, db);
            return new HttpResponseMessage()
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json")
            };
        }

        private string saveEmpolyeeToDB(EmployeeModel emp, IMongoDatabase db)
        {
            WriteResult result = new WriteResult("Employee information saved successfully", true);

            var mongoCollection = db.GetCollection<BsonDocument>("Employee");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(emp));

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
            return result.ToJson(jsonWriterSettings);
        }



        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("DeleteEmployee")]
        public HttpResponseMessage DeleteEmployee(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("Employee");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("Employee information get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getlist")]
        public HttpResponseMessage getlist([FromBody]EmployeeSearch employeeSearch)
        {
            FilterDefinition<EmployeeModel> filter = new BsonDocumentFilterDefinition<EmployeeModel>(new BsonDocument());
            
            #region---------- Search and Sort Criteria -----------------------
            var searchParams = employeeSearch.SearchParameters;
            var paging = employeeSearch.paging;
            var sorting = employeeSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.firstName))
                {
                    filter = filter & Builders<EmployeeModel>.Filter.Regex(x => x.basicInfo.firstName, BsonRegularExpression.Create(new Regex(searchParams.firstName, RegexOptions.IgnoreCase)));
                }

                if (!string.IsNullOrEmpty(searchParams.lastName))
                {
                    filter = filter & Builders<EmployeeModel>.Filter.Regex(x => x.basicInfo.lastName, BsonRegularExpression.Create(new Regex(searchParams.lastName, RegexOptions.IgnoreCase)));
                }

                if (searchParams.dob != null)
                {
                    filter = filter & Builders<EmployeeModel>.Filter.Where(x => x.basicInfo.dob == searchParams.dob);
                }

                if (searchParams.castId != null)
                {
                    filter = filter & Builders<EmployeeModel>.Filter.Where(x => x.basicInfo.castId == searchParams.castId);
                }

                if (searchParams.genderId != null)
                {
                    filter = filter & Builders<EmployeeModel>.Filter.Where(x => x.basicInfo.genderId == searchParams.genderId);
                }
                if (searchParams.religionId != null)
                {
                    filter = filter & Builders<EmployeeModel>.Filter.Where(x => x.basicInfo.religionId == searchParams.religionId);
                }
            }

            var sort = Builders<EmployeeModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<EmployeeModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<EmployeeModel>("view_employees");



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
        [System.Web.Http.Route("employeeDdl")]
        public HttpResponseMessage employeeDdl([FromBody]ObjectId empRoleId)
        {
            var data = masterDataEmployeeDdl(empRoleId, this);
            var jsonWriterSettings = new JsonWriterSettings {OutputMode = JsonOutputMode.Strict};
            var json = data.ToJson((jsonWriterSettings));

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public List<EmployeeDdl> masterDataEmployeeDdl(ObjectId empRoleId, ApiController apiController)
        {
            FilterDefinition<EmployeeDdl> filter = new BsonDocumentFilterDefinition<EmployeeDdl>(new BsonDocument());

            #region---------- Search and Sort Criteria -----------------------

            filter = filter & Builders<EmployeeDdl>.Filter.Where(x => (ObjectId) x.roleId == empRoleId);
            var sort = Builders<EmployeeDdl>.Sort.Descending("name");

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(apiController).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<EmployeeDdl>("view_employeeDdl");

            return mongoCollection.Find(filter).Sort(sort).ToList();
        }
    }
}
