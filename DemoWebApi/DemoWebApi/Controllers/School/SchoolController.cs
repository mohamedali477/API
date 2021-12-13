using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DemoWebApi.Models;
using DemoWebApi.Models.StudentGuardian;
using Microsoft.Ajax.Utilities;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoWebApi.Controllers.School
{
    [System.Web.Http.RoutePrefix("api/School")]
    public class SchoolController : ApiController
    {
        // GET api/<controller>
        [System.Web.Http.Route("getSchool")]
        public HttpResponseMessage Get(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var mongoCollection = db.GetCollection<BsonDocument>("School");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }
        
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getAllSchoolBranchList")]
        public HttpResponseMessage getAllSchoolBranchList(string schoolId)
        {
            #region---------- Search and Sort Criteria -----------------------
            var builder = Builders<BsonDocument>.Filter;
            FilterDefinition<SchoolModel> filter = new BsonDocumentFilterDefinition<SchoolModel>(new BsonDocument());
            
            #endregion

            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var mongoCollection = db.GetCollection<SchoolModel>("School");

            var data = mongoCollection.Find(filter).ToList();
            var rowCount = mongoCollection.Find(filter).CountDocuments();
            
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getSchoolList")]
        public HttpResponseMessage getSchoolList([FromBody]NoSearchModel schoolSearch)
        {
            #region---------- Search and Sort Criteria -----------------------
            var builder = Builders<BsonDocument>.Filter;
            FilterDefinition<SchoolModel> filter = new BsonDocumentFilterDefinition<SchoolModel>(new BsonDocument());

            var paging = schoolSearch.paging;
            var sorting = schoolSearch.sorting;
            
            var sort = Builders<SchoolModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<SchoolModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var mongoCollection = db.GetCollection<SchoolModel>("School");



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
        [System.Web.Http.Route("saveSchool")]
        public HttpResponseMessage saveSchool([FromBody]SchoolModel school)
        {
            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);

            var mongoCollection = db.GetCollection<BsonDocument>("School");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(school));

                mongoCollection.ReplaceOneAsync(
                filter: new BsonDocument("_id", document.GetValue("_id")),
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);

            var result = new WriteResult("School saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Deactivate")]
        public HttpResponseMessage Deactivate(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var mongoCollection = db.GetCollection<StudentGuardianModel>("School");

            var filter = Builders<StudentGuardianModel>.Filter.Where(x => (ObjectId)x._id == new ObjectId(id));
            var update = Builders<StudentGuardianModel>.Update.Set(x => x.status, 0);

            var result = mongoCollection.UpdateOne(filter, update);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("ddlSchoolBranch")]
        public HttpResponseMessage ddlSchoolBranch()
        {
            var data = masterDataSchoolBranchDdl();
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = data.ToJson((jsonWriterSettings));

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("setupBranchRoles")]
        public HttpResponseMessage setupBranchRoles(string id)
        {
            var collectionName = "SchoolRole";

            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());

            var devDB = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var devDBCollection = devDB.GetCollection<BsonDocument>(collectionName);
            var defaultRoleList = devDBCollection.Find(filter).ToList();

            var db = BaseClass.GetDatabase(id);
            var mongoCollection = db.GetCollection<BsonDocument>(collectionName);

            var msg = "Branch Roles setup successfully.";
            try
            {
                foreach (var role in defaultRoleList)
                {
                    mongoCollection.ReplaceOneAsync(
                        filter: new BsonDocument("_id", role.GetValue("_id")),
                        options: new UpdateOptions {IsUpsert = true},
                        replacement: role);
                }
            }
            catch (Exception ex)
            {
                msg = "Sorry exception occurs while setting up roles.";
            }

            var result = new WriteResult(msg, true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public List<SchoolBranchDdl> masterDataSchoolBranchDdl()
        {
            #region---------- Search and Sort Criteria -----------------------
            FilterDefinition<SchoolBranchDdl> filter = new BsonDocumentFilterDefinition<SchoolBranchDdl>(new BsonDocument());
            var sort = Builders<SchoolBranchDdl>.Sort.Descending("name");

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.DevTeamDB);
            var mongoCollection = db.GetCollection<SchoolBranchDdl>("view_ddlSchoolBranch");

            return mongoCollection.Find(filter).Sort(sort).ToList();
        }

    }
}