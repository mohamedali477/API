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

namespace DemoWebApi.Controllers.Role
{
    [System.Web.Http.RoutePrefix("api/role")]
    public class RoleController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getSchoolRoleList")]
        public HttpResponseMessage getSchoolRoleList([FromBody]RoleSearch roleSearch)
        {
            MongoDB.Driver.FilterDefinition<RoleModel> filter = new MongoDB.Driver.BsonDocumentFilterDefinition<RoleModel>(new BsonDocument());

            #region---------- Search and Sort Criteria -----------------------
            var searchParams = roleSearch.SearchParameters;
            var paging = roleSearch.paging;
            var sorting = roleSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.name))
                {
                    filter = filter & Builders<RoleModel>.Filter.Regex(x => x.name, BsonRegularExpression.Create(new Regex(searchParams.name, RegexOptions.IgnoreCase)));
                }

            }

            var sort = Builders<RoleModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<RoleModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<RoleModel>("SchoolRole");



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
        [System.Web.Http.Route("getSchoolRole")]
        public HttpResponseMessage getSchoolRole(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolRole");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveSchoolRole")]
        public HttpResponseMessage saveSchoolRole([FromBody]RoleModel roleData)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("SchoolRole");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(roleData));

            var result = new WriteResult("Role saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            try
            {
                mongoCollection.ReplaceOne(
                    filter: new BsonDocument("_id", document.GetValue("_id")),
                    options: new UpdateOptions { IsUpsert = true },
                    replacement: document);
            }
            catch (MongoWriteException ex)
            {
                result.message = "Role Already exists";
                result.isSuccess = false;
            }

            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getRoleDdl")]
        public HttpResponseMessage getRoleDdl()
        {
            var documents = masterDataRoleDdl(this);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteSchoolRole")]
        public HttpResponseMessage deleteSchoolRole(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("SchoolRole");

            var documents = mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("School Role deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public List<RoleDdlModel> masterDataRoleDdl(ApiController apiController)
        {
            FilterDefinition<RoleDdlModel> filter = new BsonDocumentFilterDefinition<RoleDdlModel>(new BsonDocument());

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(apiController).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<RoleDdlModel>("view_role_ddl");

            List<RoleDdlModel> roles = mongoCollection.Find(filter).ToList();

            for(int idx=0; idx < roles.Count; idx++)
            {
                if ((ObjectId) roles[idx]._id == HardCodedRoles.superAdminRoleId)
                {
                    roles[idx].isHidden = true;
                }
                else if ((ObjectId) roles[idx]._id == HardCodedRoles.guardianRoleId)
                {
                    roles[idx].isHidden = true;
                }
                else if ((ObjectId)roles[idx]._id == HardCodedRoles.studentRoleId)
                {
                    roles[idx].isHidden = true;
                }
            }

            return roles;
        }
    }
}