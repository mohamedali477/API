using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Routing;
using DemoWebApi.Models;
using DemoWebApi.Models.StudentGuardian;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DemoWebApi.Controllers.Transport
{
    [System.Web.Http.RoutePrefix("api/Route")]
    public class RouteController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getRoute")]
        public HttpResponseMessage getRoute(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("Route");

            var documents = mongoCollection.Find(x => x["_id"] == new ObjectId(id)).ToList();

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = documents.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("getRouteList")]
        public HttpResponseMessage getRouteList([FromBody]RouteSearch routeSearch)
        {
            FilterDefinition<RouteModel> filter = new BsonDocumentFilterDefinition<RouteModel>(new BsonDocument());
            
            #region---------- Search and Sort Criteria -----------------------
            var searchParams = routeSearch.SearchParameters;
            var paging = routeSearch.paging;
            var sorting = routeSearch.sorting;

            if (searchParams != null)
            {
                if (!string.IsNullOrEmpty(searchParams.routeName))
                {
                    filter = filter & Builders<RouteModel>.Filter.Regex(x => x.basicInfo.routeName, BsonRegularExpression.Create(new Regex(searchParams.routeName, RegexOptions.IgnoreCase)));
                }

                if (searchParams.routeLength != null)
                {
                    filter = filter & Builders<RouteModel>.Filter.Where(x => x.basicInfo.routeLength == searchParams.routeLength);
                }
            }
            
            var sort = Builders<RouteModel>.Sort.Descending(sorting.fieldName);
            if (sorting.isAsc)
            {
                sort = Builders<RouteModel>.Sort.Ascending(sorting.fieldName);
            }

            #endregion

            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<RouteModel>("view_routes");



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

       
        [System.Web.Http.Route("saveRoute")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post([FromBody]RouteModel value)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);

            var mongoCollection = db.GetCollection<BsonDocument>("Route");
            var document = BsonSerializer.Deserialize<BsonDocument>(Newtonsoft.Json.JsonConvert.SerializeObject(value));

                mongoCollection.ReplaceOneAsync(
                filter: new BsonDocument("_id", document.GetValue("_id")),
                options: new UpdateOptions { IsUpsert = true },
                replacement: document);

            var result = new WriteResult("Route saved successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("deleteRoute")]
        public HttpResponseMessage deleteRoute(string id)
        {
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("Route");

            mongoCollection.DeleteOne(x => x["_id"] == new ObjectId(id));

            var result = new WriteResult("Route get deleted successfully", true);
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var json = result.ToJson(jsonWriterSettings);

            return new HttpResponseMessage()
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("getRouteDdl")]
        public HttpResponseMessage GetRouteDdl()
        {
            FilterDefinition<BsonDocument> filter = new BsonDocumentFilterDefinition<BsonDocument>(new BsonDocument());
            
            var db = BaseClass.GetDatabase(BaseClass.GetRequestHeader(this).currentSchoolBranchId);
            var mongoCollection = db.GetCollection<BsonDocument>("view_route_ddl");

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