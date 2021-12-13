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
    [System.Web.Http.RoutePrefix("api/userSettings")]
    public class UserSettingsController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("saveUserSettings")]
        public HttpResponseMessage saveUserSettings([FromBody]UserSettingsModel settingsData)
        {
            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());
            filter = filter & Builders<BsonDocument>.Filter.Where(x => (ObjectId)x["_id"] == BaseClass.convertStringObjectIdToMongoObjectId(settingsData._id));

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("UserSettings");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(settingsData));
         
            mongoCollection.ReplaceOne(
                filter: filter,
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);


            var result = new WriteResult("User settings saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("restoreDefaultSettings")]
        public HttpResponseMessage restoreDefaultSettings(string id)
        {
            FilterDefinition<UserSettingsModel> filter = new BsonDocumentFilterDefinition<UserSettingsModel>(new BsonDocument());
            filter = filter & Builders<UserSettingsModel>.Filter.Where(x => (ObjectId)x._id == BaseClass.convertStringObjectIdToMongoObjectId(id));
            
            var settingsData = getDefaultSettings(id);

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<UserSettingsModel>("UserSettings");

            mongoCollection.ReplaceOne(
                filter: filter,
                options: new UpdateOptions { IsUpsert = true },
                replacement: settingsData);

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = settingsData.ToJson((jsonWriterSettings));

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public UserSettingsModel masterDataUserSettings(ApiController apiController)
        {
            var userId = BaseClass.GetRequestHeader(apiController).currentUserId;

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(apiController).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("UserSettings");

            var list = mongoCollection.Find(x => (ObjectId) x["_id"] == BaseClass.convertStringObjectIdToMongoObjectId(userId)).ToList();
            
            if (list.Count > 0)
            {
                var usersetting = list.First();
                var document = BsonSerializer.Deserialize<UserSettingsModel>(usersetting);
                return document;
            }
            else
            {
                return this.getDefaultSettings(userId);
            }
        }

        private UserSettingsModel getDefaultSettings(string userId)
        {
            return new UserSettingsModel()
            {
                _id = BaseClass.convertStringObjectIdToMongoObjectId(userId),
                showErrorMessages = true,
                outlineFields = true,
                expendMultiple = false,
                multipleMenuOpen = false,
                formAnimation = true,
                lineChartBgColor = "#f3f3f3",
                websiteMainColor = "#970097",
                lineChartPointRadius = 8,
                websiteBgImage = ""
            };
        }
    }
}
